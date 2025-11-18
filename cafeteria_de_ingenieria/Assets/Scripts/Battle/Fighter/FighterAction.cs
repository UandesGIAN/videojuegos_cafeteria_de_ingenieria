using UnityEngine;

public class FighterAction : MonoBehaviour
{
    // clase usada para seleccionar entre ataque melee y habilidades, tanto para el prota como enemigos 

    private GameObject player;
    private GameObject enemy;

    [SerializeField]
    private GameObject meleePrefab;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Player.ToString());
        enemy = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Enemy.ToString());
    }

    public void SelectOption(string option_name)
    {
        // recibe el nombre de la opcion elegida en el ActionMenu desde los metodos llamados por BattleManager.ActivateOption()
        GameObject target = gameObject.CompareTag(BattleConstants.CharacterRole.Player.ToString()) ? enemy : player;
        if (target == null)
        {
            Debug.LogWarning("Target ya no existe, no se puede atacar.");
            return; // No hay objetivo
        }

        Debug.Log("Current Fighter: " + gameObject.tag);
        Debug.Log("Target is " + target.GetComponent<FighterStats>().fightername + ":");

        // ataque melee (normal)
        if (option_name.CompareTo(BattleConstants.MenuAttackOptions.Melee.ToString()) == 0)
            meleePrefab.GetComponent<AttackScript>().Attack(target);
        
        // skill (por ahora randomamente)
        else if (option_name.CompareTo(BattleConstants.MenuAttackOptions.Skill.ToString()) == 0)
            meleePrefab.GetComponent<AttackScript>().UseSkillRandomly(target);
    }

    public GameObject GetEnemy()
    {
        return this.enemy;
    }

    public void SetEnemy(GameObject enemy)
    {
        this.enemy = enemy;
    }
}

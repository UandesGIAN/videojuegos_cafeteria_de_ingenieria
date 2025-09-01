using UnityEngine;

public class FighterAction : MonoBehaviour
{
    // clase usada para seleccionar entre ataque melee y habilidades, tanto para el prota como enemigos 

    private GameObject player;
    private GameObject enemy;

    [SerializeField]
    private GameObject meleePrefab;

    [SerializeField]
    private GameObject abilityPrefab; //para posible futuro uso de habilidades

    private GameObject currentAttack;
    
    // propiedades de abilidades (estoy asumiendo weas asi q cambia lo q creas necesario)
    private GameObject abilityType;
    private GameObject abilityAttack;
    private GameObject abilityModifier; //e.g., para una habilidad q sube el ataque

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Player.ToString());
        enemy = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Enemy.ToString());
    }

    public void SelectOption(string option_name)
    {
        // recibe el nombre de la opcion elegida en el ActionMenu desde los metodos llamados por BattleManager.ActivateOption()

        GameObject target = CompareTag(BattleConstants.CharacterRole.Player.ToString()) ? enemy : player;
        Debug.Log("Target is " + target.tag);

        // ataque melee (normal)
        if (option_name.CompareTo(BattleConstants.MenuAttackOptions.Melee.ToString()) == 0)
        {
            meleePrefab.GetComponent<AttackScript>().Attack(target);
            Debug.Log(BattleConstants.MenuAttackOptions.Melee.ToString() + " attack made to " + target.tag);
        } //else if (###quizas logica de habilidades si te parece!! aunque hay que equilibrarlo con lo q hizo el gian de habilidades en BattleManager
    }
}

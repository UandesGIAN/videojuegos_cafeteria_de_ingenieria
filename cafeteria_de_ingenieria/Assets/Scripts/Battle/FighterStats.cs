using System.Threading;
using UnityEngine;

public class FighterStats : MonoBehaviour
{
    // estadisticas de player y enemigos

    [SerializeField]
    private GameObject currentHealthObject;

    [SerializeField]
    private GameObject currentMagicObject;

    [Header("Stats")]
    public float health;
    public float magic;
    public float attack;
    public float defense;

    public float experience;

    private float startHealth;
    private float startMagic;

    protected Skill[] skills;

    public Skill[] GetSkills()
    {
        return skills;
    }

    [Header("UI")]
    public BattleManager battleManager;

    private bool dead = false;

    // para modificacion del tamaño de barras de health y magic
    private Vector2 healthBarDimensions;
    private Vector2 magicBarDimensions;

    private float healthBarNewHorizontalValue;
    private float magicBarNewHorizontalValue;


    private void Awake()
    {
        // currentHealthObject y Magic son establecidos en el editor xd
        healthBarDimensions = currentHealthObject.GetComponent<RectTransform>().transform.localScale;
        magicBarDimensions = currentMagicObject.GetComponent<RectTransform>().transform.localScale;
        startHealth = health;
        startMagic = magic;
        skills = this.GetComponentsInChildren<Skill>();
        battleManager = FindObjectOfType<BattleManager>();
        battleManager.SetUpSkillButtons();
       for (int i = 0; i < skills.Length; i++)
        {
            battleManager.ConfigureSkillButtons(i, skills[i].skillName);
        }
    }

    public void ReceiveDamage(float damage)
    {
        if (damage <= 0) return; // si no hizo daño, no actualizar nada

        health -= damage;
        Debug.Log("\t\tCurrent Health: " + health);
        //TODO: reproducir animacion de recibimiento de daño en unos meses xd

        if (health <= 0)
        {
            dead = true;
            gameObject.tag = BattleConstants.CharacterRole.Dead.ToString();
            Destroy(currentHealthObject);
            Destroy(currentMagicObject);
            Destroy(gameObject);
            // destruir otras cosas del target como su nombre y barritas
            return;
        }

        // si health no bajo de 0, actualizar la barrita de vida
        healthBarNewHorizontalValue = healthBarDimensions.x * (health / startHealth);
        currentHealthObject.transform.localScale = new Vector2(healthBarNewHorizontalValue, healthBarDimensions.y);

        if (gameObject.CompareTag(BattleConstants.CharacterRole.Player.ToString()))
        {
            // si player recibe daño, se espera un segundo para q no pase tan rapido el ataque xd
            Invoke(nameof(ContinueGame), 1);
        }
        else ContinueGame(); // se sigue al turno del enemigo de inmediato para evitar que jugador spamee botones
    }
    public void Heal(float healAmount)
    {
        if (healAmount <= 0) return; // si no hizo nada, no actualizar nada

        health += healAmount;
        if (health > startHealth) health = startHealth; // no puede curarse mas alla de su vida inicial
        Debug.Log("\t\tCurrent Health: " + health);

        // actualizar la barrita de vida
        healthBarNewHorizontalValue = healthBarDimensions.x * (health / startHealth);
        currentHealthObject.transform.localScale = new Vector2(healthBarNewHorizontalValue, healthBarDimensions.y);

        if (gameObject.CompareTag(BattleConstants.CharacterRole.Player.ToString()))
        {
            // si player se cura, se espera un segundo para q no pase tan rapido el ataque xd
            Invoke(nameof(ContinueGame), 1);
        }
        else ContinueGame(); // se sigue al turno del enemigo de inmediato para evitar que jugador spamee botones
    }

    public bool IsDead()
    {
        return dead;
    }

    private void ContinueGame()
    {
        GameObject.Find("TurnControllerObject").GetComponent<TurnController>().NextTurn();
    }

    // falta funcion para updatear magic bar pero eso solo pasa luego de usar una habilidad!
    // ademas quizas sea mejor aislar la logica de receivedamage y updatemagicbar a un nuevo script pq quizas no tienen mucho q ver aqui
}

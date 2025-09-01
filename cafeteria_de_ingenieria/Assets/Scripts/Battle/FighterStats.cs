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
    //private bool dead;

    // para modificacion del tamaño de barras de health y magic
    private Transform healthTransform;
    private Transform magicTransform;
    private Vector2 healthBarDimensions;
    private Vector2 magicBarDimensions;

    private float healthBarNewHorizontalValue;
    private float magicBarNewHorizontalValue;


    private void Start()
    {
        //currenthealtobject y magic son establecidos en el editor xd
        healthTransform = currentHealthObject.GetComponent<RectTransform>();
        magicTransform = currentMagicObject.GetComponent<RectTransform>();

        healthBarDimensions = currentHealthObject.transform.localScale;
        //magicBarDimensions = currentMagicObject.transform.localScale; //no se modifica todavia asi q esta comentado

        startHealth = health;
        startMagic = magic;
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        //TODO: reproducir animacion de recibimiento de daño en unos meses xd

        if (health <= 0)
        {
            //dead = true;
            gameObject.tag = BattleConstants.CharacterRole.Dead.ToString();
            Destroy(currentHealthObject);
            Destroy(gameObject);
            // destruir otras cosas del target como su nombre y barritas
        }
        else
        {
            healthBarNewHorizontalValue = healthBarDimensions.x * (health / startHealth);
            currentHealthObject.transform.localScale = new Vector2(healthBarNewHorizontalValue, healthBarDimensions.y);
        }
    }

    //falta funcion para updatear magic bar pero eso solo pasa luego de usar una habilidad!
    // ademas quizas sea mejor aislar la logica de receivedamage y updatemagicbar a un nuevo script pq quizas no tienen mucho q ver aqui
}

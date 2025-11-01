using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    // define ataque actual y modificadores del player

    public GameObject owner;

    //[SerializeField]
    //private bool magicAttack; //debug para cachar si el magic attack funca .. deberia ser activado por default en habilidades!

    //[SerializeField]
    //private float magicCost;

    /*
    [SerializeField]
    private float minAttackMult = 0.8f;

    [SerializeField]
    private float maxAttackMult = 1.2f;

    [SerializeField]
    private float minDefenseMult = 0.9f;

    [SerializeField]
    private float maxDefenseMult = 1.1f;
    */

    private FighterStats attackerStats;
    private FighterStats targetStats;
    //private float damage = 0.0f;
    //private float magicBarNewHorizontalValue;
    //private Vector2 magicBar; // representa dimensiones en 2D de la barra de magia

    void Start()
    {
        //magicBar = GameObject.Find("PlayerMagicBar").GetComponent<RectTransform>().localScale;
    }

    public void Attack(GameObject target)
    {
    	// obtener estadisticas de atacante y target desde el componente fighterstats de cada uno
    	attackerStats = owner.GetComponent<FighterStats>();
    	targetStats = target.GetComponent<FighterStats>();
    
    	// NUEVO SISTEMA - usar el cálculo de daño con tipos
    	targetStats.ReceiveDamageWithType(attackerStats, false); // false = ataque físico
    }
}

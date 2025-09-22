using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public enum ElementType
{
    AGUA,
    CEMENTO,
    ELECTRONICO,
    FUEGO,
    NORMAL,
    PLANTA
}

public static class TypeEffectiveness
{
    private static readonly Dictionary<ElementType, Dictionary<ElementType, float>> effectivenessTable = 
        new Dictionary<ElementType, Dictionary<ElementType, float>>
        {
            {
                ElementType.ELECTRONICO, new Dictionary<ElementType, float>
                {
                    { ElementType.ELECTRONICO, 0.5f },
                    { ElementType.PLANTA, 1f },
                    { ElementType.CEMENTO, 0.5f },
                    { ElementType.NORMAL, 1f },
                    { ElementType.AGUA, 2f },
                    { ElementType.FUEGO, 1f }
                }
            },
            {
                ElementType.PLANTA, new Dictionary<ElementType, float>
                {
                    { ElementType.ELECTRONICO, 1f },
                    { ElementType.PLANTA, 0.5f },
                    { ElementType.CEMENTO, 2f },
                    { ElementType.NORMAL, 0.5f },
                    { ElementType.AGUA, 2f },
                    { ElementType.FUEGO, 0.5f }
                }
            },
            {
                ElementType.CEMENTO, new Dictionary<ElementType, float>
                {
                    { ElementType.ELECTRONICO, 2f },
                    { ElementType.PLANTA, 0.5f },
                    { ElementType.CEMENTO, 2f },
                    { ElementType.NORMAL, 2f },
                    { ElementType.AGUA, 0.5f },
                    { ElementType.FUEGO, 2f }
                }
            },
            {
                ElementType.NORMAL, new Dictionary<ElementType, float>
                {
                    { ElementType.ELECTRONICO, 0.5f },
                    { ElementType.PLANTA, 1f },
                    { ElementType.CEMENTO, 1f },
                    { ElementType.NORMAL, 1f },
                    { ElementType.AGUA, 2f },
                    { ElementType.FUEGO, 1f }
                }
            },
            {
                ElementType.AGUA, new Dictionary<ElementType, float>
                {
                    { ElementType.ELECTRONICO, 2f },
                    { ElementType.PLANTA, 0.5f },
                    { ElementType.CEMENTO, 2f },
                    { ElementType.NORMAL, 0.5f },
                    { ElementType.AGUA, 1f },
                    { ElementType.FUEGO, 2f }
                }
            },
            {
                ElementType.FUEGO, new Dictionary<ElementType, float>
                {
                    { ElementType.ELECTRONICO, 1f },
                    { ElementType.PLANTA, 2f },
                    { ElementType.CEMENTO, 0.5f },
                    { ElementType.NORMAL, 2f },
                    { ElementType.AGUA, 0.5f },
                    { ElementType.FUEGO, 0.5f }
                }
            }
        };


    public static float GetTypeMultiplier(ElementType attackerType, ElementType defenderType)
    {
        if (effectivenessTable.ContainsKey(attackerType) && 
            effectivenessTable[attackerType].ContainsKey(defenderType))
        {
            return effectivenessTable[attackerType][defenderType];
        }
        return 1f; 
    }


    public static string GetEffectivenessMessage(float multiplier)
    {
        if (multiplier >= 2f)
            return "¡Es súper efectivo!";
        else if (multiplier <= 0.5f)
            return "No es muy efectivo...";
        else
            return "";
    }
}

public class FighterStats : MonoBehaviour
{
    // estadisticas de player y enemigos

    [SerializeField]
    private GameObject currentHealthObject;

    [SerializeField]
    private GameObject currentMagicObject;

    [Header("Stats del Luchador")]
    public ElementType elementType = ElementType.NORMAL;
    public float health;
    public float magic;
    public float attack;
    public float physicalArmor = 1f;
    public float magicalArmor = 1f;
    public float experience;
    public int level = 1;

    [HideInInspector]
    public float defense;

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

        // Asegurar valores mínimos de armadura
        if (physicalArmor <= 0) physicalArmor = 1f;
        if (magicalArmor <= 0) magicalArmor = 1f;
    }

    public void ReceiveDamage(float damage)
    {
        if (damage <= 0) return; // si no hizo daño, no actualizar nada

        health -= damage;
        Debug.Log($"\t\t{gameObject.name} recibió {damage} de daño. HP actual: {health}");

        if (health <= 0)
        {
            dead = true;
            gameObject.tag = "Dead";
            Destroy(currentHealthObject);
            Destroy(currentMagicObject);
            Destroy(gameObject);
            // destruir otras cosas del target como su nombre y barritas
            return;
        }

        // si health no bajo de 0, actualizar la barrita de vida
        healthBarNewHorizontalValue = healthBarDimensions.x * (health / startHealth);
        currentHealthObject.transform.localScale = new Vector2(healthBarNewHorizontalValue, healthBarDimensions.y);

        if (gameObject.CompareTag("Player"))
        {
            // si player recibe daño, se espera un segundo para q no pase tan rapido el ataque xd
            Invoke(nameof(ContinueGame), 1);
        }
        else ContinueGame(); // se sigue al turno del enemigo de inmediato para evitar que jugador spamee botones
    }

    public void ReceiveDamageWithType(FighterStats attacker, bool isMagical = false)
    {
        float damage = CalculateDamage(attacker, this, isMagical);
        ReceiveDamage(damage); 
    }

    public float CalculateDamage(FighterStats attacker, FighterStats defender, bool isMagical = false)
    {
        // Daño base del ataque (por ahora estándar = 1)
        float baseDamage = 1f;
        
        // Multiplicador = attack o magic del atacante
        float multiplier = isMagical ? attacker.magic : attacker.attack;
        
        // Armadura del defensor
        float armor = isMagical ? defender.magicalArmor : defender.physicalArmor;
        
        // Asegurar que armadura mínima sea 1 para evitar división por 0
        if (armor <= 0) armor = 1;
        
        // Obtener multiplicador de tipo
        float typeMultiplier = TypeEffectiveness.GetTypeMultiplier(attacker.elementType, defender.elementType);
        
        // Aplicar tu fórmula: (Base × (multiplicador ÷ armadura)) × Tipo
        float finalDamage = (baseDamage * (multiplier / armor)) * typeMultiplier;
        
        // Log para debug y mostrar efectividad
        string effectiveness = TypeEffectiveness.GetEffectivenessMessage(typeMultiplier);
        string attackTypeText = isMagical ? "MÁGICO" : "FÍSICO";
        
        Debug.Log($"=== CÁLCULO DE DAÑO ===");
        Debug.Log($"Atacante: {attacker.name} ({attacker.elementType}) - {attackTypeText}");
        Debug.Log($"Base: {baseDamage}, Multiplicador: {multiplier}, Armadura: {armor}");
        Debug.Log($"Multiplicador de tipo: {typeMultiplier}x");
        Debug.Log($"Fórmula: ({baseDamage} × ({multiplier} ÷ {armor})) × {typeMultiplier} = {finalDamage}");
        
        if (!string.IsNullOrEmpty(effectiveness))
            Debug.Log($">>> {effectiveness} <<<");
            
        return Mathf.Max(1f, finalDamage); // Mínimo 1 de daño
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

        if (gameObject.CompareTag("Player"))
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
}

using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using System;

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
    [Header("Stats del Luchador")]
    public ElementType elementType = ElementType.NORMAL;
    public float health;
    public float IQ;
    
    public float startHealth;
    public float startIQ;

    public float attack;
    public float physicalArmor = 1f;
    public float IQArmor = 1f;
    public float experience;
    public int level = 1;
    public Sprite img;
    public string fightername;
    public event System.Action<float, float> OnHealthChanged;
    public event System.Action<float, float> OnIQChanged;

    [Header("UI")]
    public BattleUI battleUI;
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private GameObject IQBarObject;

    [HideInInspector]
    public float defense;

    protected Skill[] skills;
    protected Item[] items;

    // para modificacion del tamaño de barras de health y IQ
    private Vector2 healthBarScale;
    private Vector2 IQBarScale;

    public event Action<FighterStats> OnDeath;

    void Awake()
    {
        startHealth = health;
        startIQ = IQ;

        skills = GetComponentsInChildren<Skill>();
        items = GetComponentsInChildren<Item>();

        if (physicalArmor <= 0) physicalArmor = 1f;
        if (IQArmor <= 0) IQArmor = 1f;

        if (healthBarObject != null)
            healthBarScale = healthBarObject.GetComponent<RectTransform>().localScale;

        if (IQBarObject != null)
            IQBarScale = IQBarObject.GetComponent<RectTransform>().localScale;
    }
    
    public Skill[] GetSkills()
    {
        return skills;
    }

    public Item[] GetItems()
    {
        return items;
    }

    public void SetupUI()
    {
        if (battleUI == null) return;

        battleUI.SetupSkillButtons(this);
        battleUI.SetupItemList(this);
    }

    public void ReceiveDamage(float damage)
    {
        if (damage <= 0) return;

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            OnDeath?.Invoke(this); // aviso de muerte
        }

        OnHealthChanged?.Invoke(health, startHealth);

        UpdateHealthBar();
    }

    public void ReceiveDamageWithType(FighterStats attacker, bool isIQal = false)
    {
        float damage = CalculateDamage(attacker, this, isIQal);
        ReceiveDamage(damage);
    }

    public float CalculateDamage(FighterStats attacker, FighterStats defender, bool isIQal = false)
    {
        float baseDamage = 1f;
        float multiplier = isIQal ? attacker.IQ : attacker.attack;
        float armor = isIQal ? defender.IQArmor : defender.physicalArmor;

        if (armor <= 0) armor = 1f;

        float typeMultiplier = TypeEffectiveness.GetTypeMultiplier(attacker.elementType, defender.elementType);
        float finalDamage = (baseDamage * (multiplier / armor)) * typeMultiplier;

        string effectiveness = TypeEffectiveness.GetEffectivenessMessage(typeMultiplier);
        string attackTypeText = isIQal ? "MÁGICO" : "FÍSICO";

        Debug.Log($"=== CÁLCULO DE DAÑO ===\nAtacante: {attacker.name} ({attacker.elementType}) - {attackTypeText}\nBase: {baseDamage}, Multiplicador: {multiplier}, Armadura: {armor}\nTipo: {typeMultiplier}x\nFórmula: ({baseDamage} × ({multiplier} ÷ {armor})) × {typeMultiplier} = {finalDamage}\n{effectiveness}");

        return Mathf.Max(1f, finalDamage);
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        health += amount;
        if (health > startHealth) health = startHealth;
        OnHealthChanged?.Invoke(health, startHealth);

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarObject != null)
        {
            float newX = healthBarScale.x * (health / startHealth);
            healthBarObject.transform.localScale = new Vector2(newX, healthBarScale.y);
        }
    }

    private void UpdateIQBar()
    {
        if (IQBarObject != null)
        {
            float newX = IQBarScale.x * (IQ / startIQ);
            IQBarObject.transform.localScale = new Vector2(newX, IQBarScale.y);
        }
        OnIQChanged?.Invoke(IQ, startIQ);
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    // para copiar valores desde otra instancia de fighterStats
    public void CopyFrom(FighterStats source)
    {
        // estadísticas principales
        this.elementType = source.elementType;
        this.health = source.health;
        this.IQ = source.IQ;
        this.startHealth = source.startHealth;
        this.startIQ = source.startIQ;
        this.attack = source.attack;
        this.physicalArmor = source.physicalArmor;
        this.IQArmor = source.IQArmor;
        this.experience = source.experience;
        this.level = source.level;
        this.img = source.img;
        this.fightername = source.fightername;

        
        this.battleUI = source.battleUI; 
        
        // copiar el estado de las barras de salud y IQ
        if (source.healthBarObject != null)
            this.healthBarObject = source.healthBarObject;
        if (source.IQBarObject != null)
            this.IQBarObject = source.IQBarObject;

        // resetear eventos
        OnHealthChanged = null;
        OnIQChanged = null;
        OnDeath = null;
    }
}

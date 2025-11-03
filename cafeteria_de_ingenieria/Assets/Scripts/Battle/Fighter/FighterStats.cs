using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.VisualScripting;

public enum FighterHierarchy // MODIFICADO!!
{
    Player,
    Boss
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
    public float IQattack;
    public float physicalArmor = 1f;
    public float IQArmor = 1f;
    public float experience;
    public int level = 1;
    public FighterHierarchy fighterHierarchy = FighterHierarchy.Player; // MODIFICADO!!: Para definir cual es el tipo del Fighter, normal se refiere a "enemigo comun", 
                                               // tambien puede ser "player" o "boss"
    
    [Header("Multiplicadores de Combate (Preguntas)")]
    [Tooltip("Multiplicadores que se aplican durante el combate actual por responder preguntas")]
    public float attackMultiplier = 1f;  // Multiplicador para attack
    public float iqAttackMultiplier = 1f; // Multiplicador para IQattack
    public Sprite img;
    public string fightername;

    [Header("Diálogos del Jefe")]
    public string dialogueOnBattleStart;
    public string dialogueOnMidHealth;
    public string dialogueOnDefeat;
    
    [HideInInspector]
    public bool hasSaidMidHealthDialogue = false;

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnIQChanged;

    [Header("UI")]
    public BattleUI battleUI;
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private GameObject IQBarObject;

    public Skill[] skills;
    public List<Item> itemList = new List<Item>();

    // para modificacion del tamaño de barras de health y IQ
    private Vector2 healthBarScale;
    private Vector2 IQBarScale;

    public event Action<FighterStats> OnDeath;

    public void Awake()
    {
        // Evitar errores por casos borde
        if (startHealth <= 0) startHealth = Mathf.Max(1f, health);
        if (startIQ <= 0) startIQ = Mathf.Max(1f, IQ);
        health = Mathf.Clamp(health, 0f, startHealth);
        IQ = Mathf.Clamp(IQ, 0f, startIQ);

        skills = GetComponentsInChildren<Skill>();

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
        return itemList.ToArray(); ;
    }

    public int GetItemCount(string itemName)
    {
        return itemList.Count(i => i.itemName == itemName);
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }

    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
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

    public void RefreshSkills()
    {
        skills = GetComponentsInChildren<Skill>(true);
    }

    public void ReceiveDamageWithType(FighterStats attacker, bool isIQal = false)
    {
        float damage = CalculateDamage(attacker, this, isIQal);
        ReceiveDamage(damage);
    }

    public float CalculateDamage(FighterStats attacker, FighterStats defender, bool isIQal = false)
    {
        float baseDamage = 1f;
        // Usar IQattack para ataques especiales y attack para ataques físicos, ambos con sus multiplicadores
        float multiplier = isIQal ? attacker.GetEffectiveIQAttack() : attacker.GetEffectiveAttack();
        float armor = isIQal ? defender.IQArmor : defender.physicalArmor;

        if (armor <= 0) armor = 1f;

        float typeMultiplier = TypeEffectiveness.GetTypeMultiplier(attacker.elementType, defender.elementType);
        float finalDamage = (baseDamage * (multiplier / armor)) * typeMultiplier;

        string effectiveness = TypeEffectiveness.GetEffectivenessMessage(typeMultiplier);
        string attackTypeText = isIQal ? "ESPECIAL" : "FÍSICO";
        
        // Mostrar valores para debug
        if (isIQal)
        {
            float baseIQAttack = attacker.IQattack;
            Debug.Log($"=== CÁLCULO DE DAÑO ===\nAtacante: {attacker.name} ({attacker.elementType}) - {attackTypeText}\nBase: {baseDamage}, IQAttack Base: {baseIQAttack}, Multiplicador: x{attacker.iqAttackMultiplier:F2}, IQAttack Efectivo: {multiplier:F1}, Armadura: {armor}\nTipo: {typeMultiplier}x\nFórmula: ({baseDamage} × ({multiplier:F1} ÷ {armor})) × {typeMultiplier} = {finalDamage:F1}\n{effectiveness}");
        }
        else
        {
            float baseAttack = attacker.attack;
            Debug.Log($"=== CÁLCULO DE DAÑO ===\nAtacante: {attacker.name} ({attacker.elementType}) - {attackTypeText}\nBase: {baseDamage}, Attack Base: {baseAttack}, Multiplicador: x{attacker.attackMultiplier:F2}, Attack Efectivo: {multiplier:F1}, Armadura: {armor}\nTipo: {typeMultiplier}x\nFórmula: ({baseDamage} × ({multiplier:F1} ÷ {armor})) × {typeMultiplier} = {finalDamage:F1}\n{effectiveness}");
        }

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

    public void ModifyIQ(float amount)
    {
        IQ += amount;
        if (IQ > startIQ) IQ = startIQ;
        if (IQ < 0) IQ = 0;

        OnIQChanged?.Invoke(IQ, startIQ);

        UpdateIQBar();
    }

    // Aplica multiplicadores de combate por responder preguntas correcta/incorrectamente
    public void ApplyQuestionMultiplier(bool isCorrect)
    {
        if (isCorrect)
        {
            // Respuesta correcta: +10% de ataque físico y especial
            attackMultiplier *= 1.1f;
            iqAttackMultiplier *= 1.1f;
            Debug.Log($"{fightername}: ¡Respuesta correcta! Attack x{attackMultiplier:F2}, IQAttack x{iqAttackMultiplier:F2}");
        }
        else
        {
            // Respuesta incorrecta: -10% de ataque físico y especial
            attackMultiplier *= 0.9f;
            iqAttackMultiplier *= 0.9f;
            Debug.Log($"{fightername}: Respuesta incorrecta. Attack x{attackMultiplier:F2}, IQAttack x{iqAttackMultiplier:F2}");
        }
    }

    //Resetea los multiplicadores de combate a valores normales
    public void ResetCombatMultipliers()
    {
        attackMultiplier = 1f;
        iqAttackMultiplier = 1f;
        Debug.Log($"{fightername}: Multiplicadores de combate reseteados");
    }

    //Obtiene el valor efectivo de ataque con multiplicadores aplicados
    public float GetEffectiveAttack()
    {
        return attack * attackMultiplier;
    }

    //Obtiene el valor efectivo de IQattack con multiplicadores aplicados
    public float GetEffectiveIQAttack()
    {
        return IQattack * iqAttackMultiplier;
    }

    public void UpdateHealthBar(bool start = false)
    {
        if (healthBarObject == null || startHealth <= 0) return;
        if (healthBarObject != null && !healthBarObject.activeSelf) healthBarObject.SetActive(true);

        float ratio = Mathf.Clamp01(health / startHealth);
        Vector3 newScale = new Vector3(ratio, healthBarScale.y, 1f);
        if (start) newScale = new Vector3(1f, healthBarScale.y, 1f);

        healthBarObject.transform.localScale = newScale;
    }

    public void UpdateIQBar()
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

    public void PrintStats()
    {
        Debug.Log($"FighterStats of {fightername}: (Abrir dropdown!!)\n" +
                  $"- Element Type: {elementType}\n" +
                  $"- Health: {health}/{startHealth}\n" +
                  $"- IQ: {IQ}/{startIQ}\n" +
                  $"- Attack: {attack} (Efectivo: {GetEffectiveAttack():F1})\n" +
                  $"- IQAttack: {IQattack} (Efectivo: {GetEffectiveIQAttack():F1})\n" +
                  $"- Physical Armor: {physicalArmor}\n" +
                  $"- IQ Armor: {IQArmor}\n" +
                  $"- Experience: {experience}\n" +
                  $"- Level: {level}\n" +
                  $"- Multiplicadores: Attack x{attackMultiplier:F2}, IQAttack x{iqAttackMultiplier:F2}");
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
        this.IQattack = source.IQattack;
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

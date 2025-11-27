using UnityEngine;

//Todos los tipos de escalados o calculos que se tienen para la modificacion de salud
public enum HealthModType
{
    Flat, Percentage
}

// Tipo de ataque para determinar qué stats usar
public enum AttackCategory
{
    Physical,   // Usa attack vs physicalArmor
    Special     // Usa IQ vs IQArmor
}

public class HealthModSkill : Skill
{
    [Header("Health Mod Skill")]
    public float amount;
    public float cost;

    public HealthModType modType;
    
    [Header("Advanced Damage Calculation")]
    [Tooltip("Tipo de ataque: Físico usa attack vs armor, Especial usa IQ vs IQArmor")]
    public AttackCategory attackCategory = AttackCategory.Physical;
    
    [Tooltip("Tipo elemental de esta habilidad para cálculo de efectividades")]
    public ElementType skillElementType = ElementType.NORMAL;
    
    [Tooltip("Habilitar variación aleatoria en el daño (85-100%)")]
    public bool useRandomVariation = true;
    
    [Tooltip("Multiplicador de daño base adicional para balanceo")]
    public float damageMultiplier = 1f;

    public override void onRun()
    {
        // si no se tiene IQ, entonces no se realiza el calculo de amount
        if (this.userStats.IQ < this.cost)
        {
            Debug.Log("No hay IQ suficiente para usar la skill: " + this.skillName);
            return;
        }

        float amount = this.GetModification();
        Debug.Log($"onRun ejecutado | Amount calculado: {amount:F1} | User: {userStats.fightername} | Target: {targetStats.fightername}");

        // si se tiene IQ, se descuenta el costo de la habilidad a la barra de IQ del usuario
        this.userStats.ModifyIQ(-this.cost);
            
        if(this.selfinflicted)
        {
            Debug.Log($"   HEAL aplicado a {targetStats.fightername}: +{amount:F1} HP");
            this.targetStats.Heal(amount);
        }
        else
        {
            Debug.Log($"   DAÑO aplicado a {targetStats.fightername}: {amount:F1} HP");
            this.targetStats.ReceiveDamage(amount);
        }
        
    }

    public float GetModification()
    {
        // Aplicar fórmula avanzada automáticamente si:
        // 1. NO es self-inflicted (no es heal)
        // 2. Es daño flat (no percentage)
        if (!this.selfinflicted && this.modType == HealthModType.Flat)
        {
            return CalculatePokemonStyleDamage();
        }
        
        // Usar cálculo original para heals y porcentajes
        switch (this.modType)
        {
            case HealthModType.Flat:
                return this.amount;
            case HealthModType.Percentage:
                return this.targetStats.health * (this.amount / 100);
            default:
                return 0;
        }
    }
    
    // Calcula el daño usando la fórmula estilo Pokémon
    // Daño = 0.01 × B × E × V × ((0.2 × N + 1) × A × P / (25 × D) + 2)
    private float CalculatePokemonStyleDamage()
    {
        Debug.Log($"Iniciando cálculo Pokémon para {skillName}");
        Debug.Log($"STATS ANTES DEL CÁLCULO:");
        Debug.Log($"\t\tUser: {userStats.fightername} | Attack: {userStats.attack} | IQattack: {userStats.IQattack} | AttackMult: {userStats.attackMultiplier:F2} | IQAttackMult: {userStats.iqAttackMultiplier:F2}");
        Debug.Log($"\t\tTarget: {targetStats.fightername} | PhysArmor: {targetStats.physicalArmor} | IQArmor: {targetStats.IQArmor}");
        Debug.Log($"\t\tSkill: AttackCategory={attackCategory}, SkillElement={skillElementType}, Amount={amount}");
        
        // N = Nivel del atacante
        float N = this.userStats.level;
        
        // A = Ataque (físico usa attack con multiplicador, especial usa IQattack con multiplicador)
        float A = (attackCategory == AttackCategory.Physical) ? 
            this.userStats.GetEffectiveAttack() : this.userStats.GetEffectiveIQAttack();
        
        // P = Poder de la habilidad
        float P = this.amount;
        
        // D = Defensa del objetivo (física o especial según attackCategory)
        float D = (attackCategory == AttackCategory.Physical) ? 
            this.targetStats.physicalArmor : this.targetStats.IQArmor;
        
        // Asegurar que la defensa no sea 0 para evitar división por cero
        D = Mathf.Max(D, 1f);
        
        // B = Bonificación STAB (Same Type Attack Bonus)
        // 1.5x si el tipo del usuario coincide con el tipo de la habilidad
        float B = (this.userStats.elementType == this.skillElementType) ? 1.5f : 1.0f;
        
        // E = Efectividad de tipo
        float E = TypeEffectiveness.GetTypeMultiplier(this.skillElementType, this.targetStats.elementType);
        
        // V = Variación aleatoria (85-100%)
        float V = useRandomVariation ? Random.Range(85, 101) / 100f : 1.0f;
        
        Debug.Log($"\t\tValores: N={N}, A={A:F1}, P={P}, D={D}, B={B}, E={E}, V={V:F2}");
        
        // Aplicar fórmula Pokémon (sin el 0.01 para que funcione mejor con stats bajos)
        float innerCalc = (0.2f * N + 1) * A * P / (25f * D) + 2;
        Debug.Log($"\t\tCálculo interno: ((0.2×{N}+1) × {A:F1} × {P} / (25×{D}) + 2) = {innerCalc:F2}");
        
        float baseDamage = B * E * V * innerCalc;
        Debug.Log($"\t\tDaño base: {B} × {E} × {V:F2} × {innerCalc:F2} = {baseDamage:F2}");
        
        // Aplicar multiplicador adicional para balanceo
        float finalDamage = baseDamage * damageMultiplier;
        
        Debug.Log($"\t\tDAÑO FINAL (con multiplicador {damageMultiplier}): {finalDamage:F1}");
        
        // Asegurar que el daño mínimo sea 1
        return Mathf.Max(finalDamage, 1f);
    }
}

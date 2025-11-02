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
    [Tooltip("Usar cálculo avanzado tipo Pokémon (solo para ataques, no auto-heal)")]
    public bool useAdvancedDamageFormula = false;
    
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
        float amount = this.GetModification();

        if (this.userStats.IQ >= this.cost)
        {
            this.userStats.ModifyIQ(-this.cost);
            
            if(this.selfinflicted)
            {
                this.targetStats.Heal(amount);
            }
            else
            {
                this.targetStats.ReceiveDamage(amount);
            }
        }
        else
        {
            Debug.Log("No hay IQ suficiente para usar la skill: " + this.skillName);
        }
    }

    public float GetModification()
    {
        // Aplicar fórmula avanzada solo si:
        // 1. Está activada la fórmula avanzada
        // 2. NO es self-inflicted (no es heal)
        // 3. Es daño flat (no percentage)
        if (useAdvancedDamageFormula && !this.selfinflicted && this.modType == HealthModType.Flat)
        {
            return CalculatePokemonStyleDamage();
        }
        
        // Usar cálculo original para percentage
        switch (this.modType)
        {
            case HealthModType.Percentage:
                return this.targetStats.health * (this.amount / 100);
            default:
                return 0;
        }
    }
    
    /// Calcula el daño usando la fórmula estilo Pokémon
    /// Daño = 0.01 × B × E × V × ((0.2 × N + 1) × A × P / (25 × D) + 2)
    private float CalculatePokemonStyleDamage()
    {
        // N = Nivel del atacante
        float N = this.userStats.level;
        
        // A = Ataque (físico o especial según attackCategory)
        float A = (attackCategory == AttackCategory.Physical) ? 
            this.userStats.attack : this.userStats.IQ;
        
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
        
        // Aplicar fórmula Pokémon
        float baseDamage = 0.01f * B * E * V * ((0.2f * N + 1) * A * P / (25f * D) + 2);
        
        // Aplicar multiplicador adicional para balanceo
        float finalDamage = baseDamage * damageMultiplier;
        
        // Asegurar que el daño mínimo sea 1
        return Mathf.Max(finalDamage, 1f);
    }
}

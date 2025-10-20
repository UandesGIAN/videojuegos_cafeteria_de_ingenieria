using UnityEngine;
public enum StatsModType
{
    Flat,
    Percentage
}

public enum StatsType
{
    Health,
    IQ,
    Attack,
    PhysicalArmor,
    IQArmor
}

public class StatsModItems : Item
{
    [Header("Configuración del Objeto")]
    [Tooltip("stat a modificar")]
    public StatsType statType;
    [Tooltip("Cantidad de modificación")]
    public float modificationAmount;
    [Tooltip("De que manera se aplica la modificación")]
    public StatsModType modType;
    
    public override void onRun()
    {
        FighterStats userStats = GetComponentInParent<FighterStats>();

        if (userStats == null)
        {
            Debug.LogError("No se encontro un fighterstat");
            return;
        }

        ApplyStatModification(userStats);
    }

    private void ApplyStatModification(FighterStats stats)
    {
        switch (statType)
        {
            case StatsType.Health:
                if (modificationAmount > 0)
                    stats.Heal(modificationAmount);
                else
                    stats.ReceiveDamage(-modificationAmount);
                break;

            case StatsType.IQ:
                stats.ModifyIQ(modificationAmount);
                break;
                
        }
        
        Debug.Log($"{stats.fightername} usó {itemName}, modificando {statType} en {modificationAmount} ({modType}).");
    }

    private float CalculateNewValue(float currentValue)
    {
        return modType switch
        {
            StatsModType.Flat => currentValue + modificationAmount,
            StatsModType.Percentage => currentValue * (1 + modificationAmount / 100f),
            _ => currentValue
        };
    }
}
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
        // Usar userStats (establecido con SetUser) en lugar de currentPlayer
        if (userStats == null)
        {
            Debug.LogError($"StatsModItems: userStats es null para item {itemName}. Asegúrate de llamar SetUser() antes de Run().");
            return;
        }

        ApplyStatModification();
    }

    private void ApplyStatModification()
    {
        switch (statType)
        {
            case StatsType.Health:
                if (modificationAmount > 0)
                    userStats.Heal(modificationAmount);
                break;

            case StatsType.IQ:
                userStats.ModifyIQ(modificationAmount);
                break;

            case StatsType.Attack:
                //to do
                break;

            case StatsType.PhysicalArmor:
                //to do
                break;

            case StatsType.IQArmor:
                //to do
                break;
                
        }
        
        Debug.Log($"{userStats.fightername} usó {itemName}, modificando {statType} en {modificationAmount} ({modType}).");
    }
}
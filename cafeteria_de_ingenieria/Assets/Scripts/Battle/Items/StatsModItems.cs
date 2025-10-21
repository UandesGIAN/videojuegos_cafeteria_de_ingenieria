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

    public FighterStats currentPlayer;
    
    public override void onRun()
    {
        if (currentPlayer == null)
        {
            Debug.LogError("No se encontro un fighterstat");
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
                    currentPlayer.Heal(modificationAmount);
                break;

            case StatsType.IQ:
                currentPlayer.ModifyIQ(modificationAmount);
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
        
        Debug.Log($"{currentPlayer.fightername} usó {itemName}, modificando {statType} en {modificationAmount} ({modType}).");
    }
}
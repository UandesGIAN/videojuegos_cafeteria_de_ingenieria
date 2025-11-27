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
        float amount = modificationAmount;

        switch (statType)
        {
            case StatsType.Health:
                if (modificationAmount > 0)
                {
                    if (modType == StatsModType.Flat)
                        userStats.Heal(amount);
                    else if (modType == StatsModType.Percentage)
                    {
                        float healValue = userStats.startHealth * (amount / 100f);
                        userStats.Heal(healValue);
                    }
                }
                break;

            case StatsType.IQ:

                if (modType == StatsModType.Flat)
                {
                    userStats.ModifyIQ(amount);
                }
                else if (modType == StatsModType.Percentage)
                {
                    float iqValue = userStats.startIQ * (amount / 100f);
                    userStats.ModifyIQ(iqValue);
                }
                break;

            case StatsType.Attack:

                if (modType == StatsModType.Flat)
                {
                    userStats.attack += amount;
                }
                else if (modType == StatsModType.Percentage)
                {
                    float atkValue = userStats.attack * (amount / 100f);
                    userStats.attack += atkValue;
                }
                break;

            case StatsType.PhysicalArmor:

                if (modType == StatsModType.Flat)
                {
                    userStats.physicalArmor += amount;
                }
                else if (modType == StatsModType.Percentage)
                {
                    float paValue = userStats.physicalArmor * (amount / 100f);
                    userStats.physicalArmor += paValue;
                }
                break;

            case StatsType.IQArmor:

                if (modType == StatsModType.Flat)
                {
                    userStats.IQArmor += amount;
                }
                else if (modType == StatsModType.Percentage)
                {
                    float iqAValue = userStats.IQArmor * (amount / 100f);
                    userStats.IQArmor += iqAValue;
                }
                break;
        }

        Debug.Log($"{userStats.fightername} usó {itemName}, modificando {statType} en {modificationAmount} ({modType}).");
    }
}
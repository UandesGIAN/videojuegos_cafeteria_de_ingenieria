using UnityEngine;
using System;
using System.Reflection;

public enum StatsModType
{
    Flat, Percentage
}

public enum StatsType
{
    Attack, Defense, Speed, Health, Mana
}

// Versión alternativa usando Reflection (más automática)
public class StatsModItems : Item
{
    public StatsType statType;
    public float modificationAmount;
    public StatsModType modType;
    protected FighterStats userStats;

    public override void onRun()
    {
        ApplyStatModification(modificationAmount);
    }

    private void ApplyStatModification(float modificationAmount)
    {
        if (userStats == null) return;

        string fieldName = statType.ToString().ToLower();
        FieldInfo field = typeof(FighterStats).GetField(fieldName);
        
        if (field != null && field.FieldType == typeof(float))
        {
            float currentValue = (float)field.GetValue(userStats);
            float newValue = CalculateNewValue(currentValue, modificationAmount);
            field.SetValue(userStats, newValue);
        }
    }

    private float CalculateNewValue(float currentValue, float modificationAmount)
    {
        return modType switch
        {
            StatsModType.Flat => currentValue + modificationAmount,
            StatsModType.Percentage => currentValue * (1 + modificationAmount / 100),
            _ => currentValue
        };
    }
}

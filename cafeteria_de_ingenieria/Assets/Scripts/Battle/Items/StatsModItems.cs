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

        switch (statType)
        {
            case StatsType.Attack:
                userStats.attack = CalculateNewValue(userStats.attack, modificationAmount);
                break;
            case StatsType.Defense:
                userStats.defense = CalculateNewValue(userStats.defense, modificationAmount);
                break;
            case StatsType.Health:
                userStats.health = CalculateNewValue(userStats.health, modificationAmount);
                break;
            case StatsType.Mana:
                userStats.magic = CalculateNewValue(userStats.magic, modificationAmount);
                break;
            default:
                Debug.LogWarning($"Stat type {statType} not handled.");
                break;
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

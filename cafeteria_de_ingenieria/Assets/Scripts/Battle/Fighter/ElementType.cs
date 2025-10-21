using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum ElementType
{
    AGUA,
    CEMENTO,
    ELECTRONICO,
    FUEGO,
    NORMAL,
    PLANTA,
    COMPUTACION
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
                    { ElementType.FUEGO, 1f },
                    { ElementType.COMPUTACION, 0.5f }
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
                    { ElementType.FUEGO, 0.5f },
                    { ElementType.COMPUTACION, 1f }
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
                    { ElementType.FUEGO, 2f },
                    { ElementType.COMPUTACION, 2f }
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
                    { ElementType.FUEGO, 1f },
                    { ElementType.COMPUTACION, 0.5f }
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
                    { ElementType.FUEGO, 2f },
                    { ElementType.COMPUTACION, 2f }
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
                    { ElementType.FUEGO, 0.5f },
                    { ElementType.COMPUTACION, 1f }
                }
            },
            {
                ElementType.COMPUTACION, new Dictionary<ElementType, float>
                {
                    { ElementType.ELECTRONICO, 2f },
                    { ElementType.PLANTA, 1f },
                    { ElementType.CEMENTO, 0.5f },
                    { ElementType.NORMAL, 2f },
                    { ElementType.AGUA, 0.5f },
                    { ElementType.FUEGO, 1.5f },
                    { ElementType.COMPUTACION, 0.5f }
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
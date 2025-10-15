using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("Info básica")]
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Efecto de mejora")]
    public UpgradeType type;
    public float value;

    public void Apply(FighterStats playerStats)
    {
        switch (type)
        {
            case UpgradeType.MaxHP:
                //playerStats.maxHP += value;
                //playerStats.currentHP = Mathf.Min(playerStats.currentHP + value, playerStats.maxHP);
                break;

            case UpgradeType.Attack:
                playerStats.attack += value;
                break;

            case UpgradeType.IQ:
                //playerStats.IQ += value;
                break;

            case UpgradeType.Skill:
                // Ejemplo de mejora especial (desbloquear habilidad)
                //playerStats.UnlockRandomSkill();
                break;

            case UpgradeType.Item:
                // Ejemplo de mejora especial (obtener objeto)
                //playerStats.UnlockRandomSkill();
                break;

            default:
                Debug.LogWarning($"UpgradeType {type} no manejado aún.");
                break;
        }

        Debug.Log($"Upgrade aplicada: {upgradeName} (+{value} {type})");
    }
}

public enum UpgradeType
{
    MaxHP,
    Attack,
    IQ,
    Skill,
    Item
}

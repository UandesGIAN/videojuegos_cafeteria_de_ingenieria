using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public string upgradeName;
    public string description;
    public Sprite icon;

    public enum UpgradeType
    {
        Health,
        IQ,
        Attack,
        Defense,
        IQArmor,
        PhysicalArmor
    }

    public UpgradeType upgradeType;
    public float bonusAmount;

    /// <summary>
    /// Aplica el upgrade al jugador
    /// </summary>
    public void Apply(FighterStats player)
    {
        if (player == null)
        {
            Debug.LogError("Upgrade: No se puede aplicar, player es null");
            return;
        }

        switch (upgradeType)
        {
            case UpgradeType.Health:
                player.startHealth += bonusAmount;
                player.health += bonusAmount;
                player.UpdateHealthBar();
                Debug.Log($"Upgrade aplicado: +{bonusAmount} salud máxima");
                break;

            case UpgradeType.IQ:
                player.startIQ += bonusAmount;
                player.IQ += bonusAmount;
                player.UpdateIQBar();
                Debug.Log($"Upgrade aplicado: +{bonusAmount} IQ máximo");
                break;

            case UpgradeType.Attack:
                player.attack += bonusAmount;
                Debug.Log($"Upgrade aplicado: +{bonusAmount} ataque");
                break;

            case UpgradeType.Defense:
                player.defense += bonusAmount;
                Debug.Log($"Upgrade aplicado: +{bonusAmount} defensa");
                break;

            case UpgradeType.IQArmor:
                player.IQArmor += bonusAmount;
                Debug.Log($"Upgrade aplicado: +{bonusAmount} armadura IQ");
                break;

            case UpgradeType.PhysicalArmor:
                player.physicalArmor += bonusAmount;
                Debug.Log($"Upgrade aplicado: +{bonusAmount} armadura física");
                break;
        }

        player.PrintStats();
    }
}
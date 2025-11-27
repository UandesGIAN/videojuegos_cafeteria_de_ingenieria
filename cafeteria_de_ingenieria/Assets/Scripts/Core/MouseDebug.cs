using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;


public class MouseDebug : MonoBehaviour
{
    public UpgradeSelection upgradeSelection;

    void Awake()
    {
        if (upgradeSelection == null)
        {
            upgradeSelection = FindObjectOfType<UpgradeSelection>();
            if (upgradeSelection == null)
                Debug.LogWarning("⚠ No se encontró UpgradeSelection en la escena.");
        }
    }
    
    void OnGUI()
    {
        // Crea un estilo de texto grande para verlo bien en el build
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.red;

        string message = "Mouse Debug:\n";
        
        // 1. Posición del mouse
        message += $"Pos: {Input.mousePosition}\n";

        // 2. ¿El sistema detecta el mouse sobre la UI?
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        message += $"IsPointerOverGameObject: {isOverUI}\n";

        // 3. ¿Qué objeto específico está debajo?
        if (isOverUI)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                message += $"Hit: {results[0].gameObject.name}\n";
                message += $"Layer: {LayerMask.LayerToName(results[0].gameObject.layer)}";
            }
        }
        else
        {
            message += "Hit: NADA (o Raycast bloqueado)";
        }

        message += GetRewardsDebug();

        GUI.Label(new Rect(20, 20, 500, 500), message, style);
    }

    string GetRewardsDebug()
    {
        if (upgradeSelection == null)
            return "\n(UpgradeSelection no asignado)\n";

        var all = upgradeSelection.GetAllPossibleRewards().ToList();

        int skillCount = all.Count(r => r.type == UpgradeSelection.Reward.RewardType.Skill);
        int itemCount = all.Count(r => r.type == UpgradeSelection.Reward.RewardType.Item);
        int upgradeCount = all.Count(r => r.type == UpgradeSelection.Reward.RewardType.Upgrade);

        return
            "\n--- Rewards Disponibles ---\n" +
            $"Skills: {skillCount}\n" +
            $"Items: {itemCount}\n" +
            $"Upgrades: {upgradeCount}\n" +
            $"Total: {all.Count}\n";
    }
}
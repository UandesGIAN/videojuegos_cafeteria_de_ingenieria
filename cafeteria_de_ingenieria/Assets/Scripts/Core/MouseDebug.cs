using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDebug : MonoBehaviour
{
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

        GUI.Label(new Rect(20, 20, 500, 500), message, style);
    }
}
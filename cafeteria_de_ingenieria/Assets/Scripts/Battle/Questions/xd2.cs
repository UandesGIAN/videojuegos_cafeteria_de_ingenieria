using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RecreateQuestionUI : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Recreate Question UI in Scene")]
    public static void RecreateUI()
    {
        // Buscar BattleHUD en la escena
        GameObject battleHUD = GameObject.Find("BattleHUD");
        
        if (battleHUD == null)
        {
            Debug.LogError("No se encontró BattleHUD en la escena. Asegúrate de estar en la escena correcta.");
            return;
        }

        // Verificar si ya existe QuestionUI y eliminarlo
        Transform existingQuestionUI = battleHUD.transform.Find("QuestionUI");
        if (existingQuestionUI != null)
        {
            Debug.Log("Se encontró un QuestionUI existente, eliminándolo...");
            DestroyImmediate(existingQuestionUI.gameObject);
        }

        // Crear nuevo QuestionUI
        GameObject questionUIObj = new GameObject("QuestionUI", typeof(RectTransform));
        questionUIObj.transform.SetParent(battleHUD.transform, false);
        questionUIObj.transform.localScale = Vector3.one;

        // Configurar RectTransform del QuestionUI
        RectTransform questionUIRect = questionUIObj.GetComponent<RectTransform>();
        questionUIRect.anchorMin = Vector2.zero;
        questionUIRect.anchorMax = Vector2.one;
        questionUIRect.offsetMin = Vector2.zero;
        questionUIRect.offsetMax = Vector2.zero;

        // Agregar componentes de Canvas al QuestionUI
        Canvas canvas = questionUIObj.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 10;
        
        CanvasScaler scaler = questionUIObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1;
        scaler.referencePixelsPerUnit = 100;
        
        questionUIObj.AddComponent<GraphicRaycaster>();

        // Agregar componente QuestionUI
        QuestionUI questionUI = questionUIObj.AddComponent<QuestionUI>();

        // Crear QuestionPanel
        GameObject questionPanel = CreatePanel("QuestionPanel", questionUIObj.transform);
        RectTransform panelRect = questionPanel.GetComponent<RectTransform>();
        
        // Configurar QuestionPanel centrado
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(1000, 700);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.localScale = Vector3.one;
        
        // Fondo semi-transparente oscuro
        Image panelImage = questionPanel.GetComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        // Agregar Vertical Layout Group al panel
        VerticalLayoutGroup vlg = questionPanel.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(60, 60, 40, 40);
        vlg.spacing = 15;
        vlg.childAlignment = TextAnchor.UpperCenter;

        // 1. Category Text
        GameObject categoryTextObj = CreateTextMeshPro("CategoryText", questionPanel.transform);
        TextMeshProUGUI categoryText = categoryTextObj.GetComponent<TextMeshProUGUI>();
        categoryText.text = "Categoría: Ingeniería";
        categoryText.fontSize = 22;
        categoryText.enableAutoSizing = false;
        categoryText.alignment = TextAlignmentOptions.Center;
        categoryText.color = Color.yellow;
        
        LayoutElement categoryLayout = categoryTextObj.AddComponent<LayoutElement>();
        categoryLayout.preferredHeight = 35;
        categoryLayout.minHeight = 35;

        // 2. Question Text
        GameObject questionTextObj = CreateTextMeshPro("QuestionText", questionPanel.transform);
        TextMeshProUGUI questionText = questionTextObj.GetComponent<TextMeshProUGUI>();
        questionText.text = "¿Aquí va la pregunta?";
        questionText.fontSize = 26;
        questionText.enableAutoSizing = false;
        questionText.alignment = TextAlignmentOptions.Center;
        questionText.enableWordWrapping = true;
        questionText.color = Color.white;
        
        LayoutElement questionLayout = questionTextObj.AddComponent<LayoutElement>();
        questionLayout.preferredHeight = 100;
        questionLayout.minHeight = 80;
        questionLayout.flexibleHeight = 1;

        // 3. Answer Buttons
        Button[] answerButtons = new Button[4];
        TextMeshProUGUI[] answerTexts = new TextMeshProUGUI[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject buttonObj = CreateButton($"AnswerButton{i + 1}", questionPanel.transform);
            answerButtons[i] = buttonObj.GetComponent<Button>();
            
            LayoutElement buttonLayout = buttonObj.AddComponent<LayoutElement>();
            buttonLayout.preferredHeight = 60;
            buttonLayout.minHeight = 60;

            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = $"Respuesta {i + 1}";
            buttonText.fontSize = 20;
            buttonText.enableAutoSizing = false;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.enableWordWrapping = true;
            buttonText.color = Color.white;

            RectTransform textRect = buttonText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(15, 10);
            textRect.offsetMax = new Vector2(-15, -10);

            answerTexts[i] = buttonText;

            ColorBlock colors = answerButtons[i].colors;
            colors.normalColor = new Color(0.3f, 0.3f, 0.5f, 1f);
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.6f, 1f);
            colors.pressedColor = new Color(0.2f, 0.6f, 0.2f, 1f);
            answerButtons[i].colors = colors;
        }

        // 4. Feedback Panel
        GameObject feedbackPanel = CreatePanel("FeedbackPanel", questionPanel.transform);
        Image feedbackImage = feedbackPanel.GetComponent<Image>();
        feedbackImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
        
        LayoutElement feedbackLayout = feedbackPanel.AddComponent<LayoutElement>();
        feedbackLayout.preferredHeight = 90;
        feedbackLayout.minHeight = 90;

        GameObject feedbackTextObj = CreateTextMeshPro("FeedbackText", feedbackPanel.transform);
        TextMeshProUGUI feedbackText = feedbackTextObj.GetComponent<TextMeshProUGUI>();
        feedbackText.text = "Feedback aquí";
        feedbackText.fontSize = 22;
        feedbackText.enableAutoSizing = false;
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.enableWordWrapping = true;
        feedbackText.color = Color.green;

        RectTransform feedbackTextRect = feedbackTextObj.GetComponent<RectTransform>();
        feedbackTextRect.anchorMin = Vector2.zero;
        feedbackTextRect.anchorMax = Vector2.one;
        feedbackTextRect.offsetMin = new Vector2(20, 10);
        feedbackTextRect.offsetMax = new Vector2(-20, -10);

        feedbackPanel.SetActive(false);

        // Asignar todas las referencias al QuestionUI
        questionUI.questionPanel = questionPanel;
        questionUI.categoryText = categoryText;
        questionUI.questionText = questionText;
        questionUI.answerButtons = answerButtons;
        questionUI.answerTexts = answerTexts;
        questionUI.feedbackPanel = feedbackPanel;
        questionUI.feedbackText = feedbackText;

        // Desactivar el panel completo por defecto
        questionPanel.SetActive(false);

        // Buscar el QuestionPool existente
        string[] guids = AssetDatabase.FindAssets("t:QuestionPool");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            QuestionPool pool = AssetDatabase.LoadAssetAtPath<QuestionPool>(path);
            questionUI.questionPool = pool;
            Debug.Log($"QuestionPool encontrado y asignado: {path}");
        }
        else
        {
            Debug.LogWarning("No se encontró QuestionPool. Asígnalo manualmente en el Inspector.");
        }

        // Buscar BattleManager y asignar QuestionUI
        BattleManager battleManager = FindObjectOfType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.questionUI = questionUI;
            EditorUtility.SetDirty(battleManager);
            Debug.Log("QuestionUI asignado a BattleManager automáticamente.");
        }

        EditorUtility.SetDirty(questionUI);
        EditorUtility.SetDirty(questionUIObj);
        EditorUtility.SetDirty(battleHUD);

        Selection.activeGameObject = questionUIObj;

        Debug.Log("✅ QuestionUI recreado exitosamente en la escena!");
    }

    private static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        return panel;
    }

    private static GameObject CreateTextMeshPro(string name, Transform parent)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(parent, false);
        textObj.transform.localScale = Vector3.one;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);

        return textObj;
    }

    private static GameObject CreateButton(string name, Transform parent)
    {
        GameObject buttonObj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObj.transform.SetParent(parent, false);
        buttonObj.transform.localScale = Vector3.one;

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(160, 30);

        GameObject textObj = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(buttonObj.transform, false);
        textObj.transform.localScale = Vector3.one;

        return buttonObj;
    }
#endif
}

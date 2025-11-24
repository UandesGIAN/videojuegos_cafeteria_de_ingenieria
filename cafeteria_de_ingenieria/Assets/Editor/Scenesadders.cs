using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
public class QuestionUISceneCreator : EditorWindow
{
    [MenuItem("Tools/Create 6 QuestionUI in Scene")]
    public static void ShowWindow()
    {
        CreateAllQuestionUIInScene();
    }

    private static void CreateAllQuestionUIInScene()
    {
        string[] categories = new string[] 
        { 
            "Ambiental", 
            "Civil", 
            "Computacion", 
            "Electrica", 
            "Obras", 
            "PlanComun" 
        };

        foreach (string category in categories)
        {
            CreateQuestionUIInScene(category);
        }

        Debug.Log("¡Los 6 QuestionUI han sido creados en la escena!");
    }

    private static void CreateQuestionUIInScene(string category)
    {
        // Crear GameObject principal
        GameObject questionUIObj = new GameObject("QuestionUI_" + category);
        
        // Agregar Canvas
        Canvas canvas = questionUIObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        
        CanvasScaler scaler = questionUIObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        questionUIObj.AddComponent<GraphicRaycaster>();

        // Crear QuestionPanel
        GameObject questionPanel = new GameObject("QuestionPanel");
        questionPanel.transform.SetParent(questionUIObj.transform, false);
        RectTransform panelRect = questionPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        Image panelImage = questionPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);

        // Crear CategoryText
        GameObject categoryTextObj = CreateTextObject("CategoryText", questionPanel.transform);
        RectTransform categoryRect = categoryTextObj.GetComponent<RectTransform>();
        categoryRect.anchoredPosition = new Vector2(0, 400);
        categoryRect.sizeDelta = new Vector2(800, 60);
        TextMeshProUGUI categoryTMP = categoryTextObj.GetComponent<TextMeshProUGUI>();
        categoryTMP.fontSize = 40;
        categoryTMP.fontStyle = FontStyles.Bold;

        // Crear QuestionText
        GameObject questionTextObj = CreateTextObject("QuestionText", questionPanel.transform);
        RectTransform questionRect = questionTextObj.GetComponent<RectTransform>();
        questionRect.anchoredPosition = new Vector2(0, 200);
        questionRect.sizeDelta = new Vector2(1400, 250);
        TextMeshProUGUI questionTMP = questionTextObj.GetComponent<TextMeshProUGUI>();
        questionTMP.fontSize = 32;

        // Crear 4 Answer Buttons
        Button[] buttons = new Button[4];
        TextMeshProUGUI[] answerTexts = new TextMeshProUGUI[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject btnObj = CreateButtonObject("AnswerButton" + (i + 1), questionPanel.transform);
            RectTransform btnRect = btnObj.GetComponent<RectTransform>();
            btnRect.anchoredPosition = new Vector2(0, 20 - (i * 110));
            btnRect.sizeDelta = new Vector2(1200, 90);

            buttons[i] = btnObj.GetComponent<Button>();
            
            GameObject textObj = CreateTextObject("Text", btnObj.transform);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = new Vector2(-20, -20);
            textRect.anchoredPosition = Vector2.zero;
            
            answerTexts[i] = textObj.GetComponent<TextMeshProUGUI>();
            answerTexts[i].fontSize = 28;
        }

        // Crear FeedbackPanel
        GameObject feedbackPanel = new GameObject("FeedbackPanel");
        feedbackPanel.transform.SetParent(questionUIObj.transform, false);
        RectTransform feedbackRect = feedbackPanel.AddComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.5f, 0.5f);
        feedbackRect.anchorMax = new Vector2(0.5f, 0.5f);
        feedbackRect.anchoredPosition = Vector2.zero;
        feedbackRect.sizeDelta = new Vector2(900, 400);
        Image feedbackImage = feedbackPanel.AddComponent<Image>();
        feedbackImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        // Crear FeedbackText
        GameObject feedbackTextObj = CreateTextObject("FeedbackText", feedbackPanel.transform);
        RectTransform feedbackTextRect = feedbackTextObj.GetComponent<RectTransform>();
        feedbackTextRect.anchorMin = Vector2.zero;
        feedbackTextRect.anchorMax = Vector2.one;
        feedbackTextRect.sizeDelta = new Vector2(-40, -40);
        feedbackTextRect.anchoredPosition = Vector2.zero;
        TextMeshProUGUI feedbackTMP = feedbackTextObj.GetComponent<TextMeshProUGUI>();
        feedbackTMP.fontSize = 36;
        feedbackTMP.fontStyle = FontStyles.Bold;

        // Agregar el script correspondiente según la categoría
        MonoBehaviour script = null;
        
        switch (category)
        {
            case "Ambiental":
                script = questionUIObj.AddComponent<QuestionUI_Ambiental>();
                break;
            case "Civil":
                script = questionUIObj.AddComponent<QuestionUI_Civil>();
                break;
            case "Computacion":
                script = questionUIObj.AddComponent<QuestionUI_Computacion>();
                break;
            case "Electrica":
                script = questionUIObj.AddComponent<QuestionUI_Electrica>();
                break;
            case "Obras":
                script = questionUIObj.AddComponent<QuestionUI_Obras>();
                break;
            case "PlanComun":
                script = questionUIObj.AddComponent<QuestionUI_PlanComun>();
                break;
        }

        // Asignar referencias al script
        if (script != null)
        {
            SerializedObject so = new SerializedObject(script);
            so.FindProperty("questionPanel").objectReferenceValue = questionPanel;
            so.FindProperty("categoryText").objectReferenceValue = categoryTMP;
            so.FindProperty("questionText").objectReferenceValue = questionTMP;
            
            SerializedProperty answerButtonsProp = so.FindProperty("answerButtons");
            answerButtonsProp.arraySize = 4;
            for (int i = 0; i < 4; i++)
                answerButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
            
            SerializedProperty answerTextsProp = so.FindProperty("answerTexts");
            answerTextsProp.arraySize = 4;
            for (int i = 0; i < 4; i++)
                answerTextsProp.GetArrayElementAtIndex(i).objectReferenceValue = answerTexts[i];
            
            so.FindProperty("feedbackText").objectReferenceValue = feedbackTMP;
            so.FindProperty("feedbackPanel").objectReferenceValue = feedbackPanel;
            so.FindProperty("feedbackDuration").floatValue = 2.5f;
            
            so.ApplyModifiedProperties();
        }

        // Desactivar los paneles por defecto
        questionPanel.SetActive(false);
        feedbackPanel.SetActive(false);

        Debug.Log("QuestionUI_" + category + " creado en la escena");
    }

    private static GameObject CreateTextObject(string name, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        
        return textObj;
    }

    private static GameObject CreateButtonObject(string name, Transform parent)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rectTransform = btnObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        
        Image image = btnObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.6f, 1f);
        
        Button button = btnObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.4f, 0.6f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.5f, 0.7f, 1f);
        colors.pressedColor = new Color(0.15f, 0.3f, 0.5f, 1f);
        button.colors = colors;
        
        return btnObj;
    }
}
#endif

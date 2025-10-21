using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FixQuestionUILayout : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Fix Question UI Layout")]
    public static void FixLayout()
    {
        QuestionUI questionUI = FindObjectOfType<QuestionUI>();
        
        if (questionUI == null)
        {
            Debug.LogError("No se encontró QuestionUI en la escena.");
            return;
        }

        GameObject questionUIObj = questionUI.gameObject;
        
        // Asegurar escala correcta del QuestionUI
        questionUIObj.transform.localScale = Vector3.one;

        // Buscar o crear el QuestionPanel
        Transform panelTransform = questionUIObj.transform.Find("QuestionPanel");
        if (panelTransform == null)
        {
            Debug.LogError("No se encontró QuestionPanel");
            return;
        }

        GameObject questionPanel = panelTransform.gameObject;
        RectTransform panelRect = questionPanel.GetComponent<RectTransform>();

        // LIMPIAR TODOS LOS TEXTMESHPRO SUELTOS QUE NO ESTÉN EN LA ESTRUCTURA CORRECTA
        TextMeshProUGUI[] allTexts = questionPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in allTexts)
        {
            // Si el texto no es parte de un botón y no es uno de los textos principales, eliminarlo
            string parentName = text.transform.parent.name;
            if (parentName == "QuestionPanel" && 
                text.name != "CategoryText" && 
                text.name != "QuestionText")
            {
                // Es un texto suelto no deseado
                DestroyImmediate(text.gameObject);
            }
        }

        // Configurar panel centrado
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(1000, 700);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.localScale = Vector3.one;

        // Configurar o agregar Vertical Layout Group
        VerticalLayoutGroup vlg = questionPanel.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = questionPanel.AddComponent<VerticalLayoutGroup>();
        
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(50, 50, 30, 30);
        vlg.spacing = 15;
        vlg.childAlignment = TextAnchor.UpperCenter;

        // Arreglar CategoryText
        FixText(questionPanel, "CategoryText", 22, 35, TextAlignmentOptions.Center, Color.yellow);

        // Arreglar QuestionText
        FixText(questionPanel, "QuestionText", 26, 100, TextAlignmentOptions.Center, Color.white, flexibleHeight: true);

        // Arreglar Answer Buttons
        for (int i = 1; i <= 4; i++)
        {
            FixButton(questionPanel, $"AnswerButton{i}", 20, 60);
        }

        // Arreglar FeedbackPanel
        Transform feedbackTransform = questionPanel.transform.Find("FeedbackPanel");
        if (feedbackTransform != null)
        {
            GameObject feedbackPanel = feedbackTransform.gameObject;
            RectTransform feedbackRect = feedbackPanel.GetComponent<RectTransform>();
            
            LayoutElement feedbackLayout = feedbackPanel.GetComponent<LayoutElement>();
            if (feedbackLayout == null) feedbackLayout = feedbackPanel.AddComponent<LayoutElement>();
            feedbackLayout.preferredHeight = 90;
            feedbackLayout.minHeight = 90;

            // Arreglar FeedbackText
            Transform feedbackTextTransform = feedbackPanel.transform.Find("FeedbackText");
            if (feedbackTextTransform != null)
            {
                TextMeshProUGUI feedbackText = feedbackTextTransform.GetComponent<TextMeshProUGUI>();
                if (feedbackText != null)
                {
                    feedbackText.fontSize = 22;
                    feedbackText.enableAutoSizing = false;
                    feedbackText.alignment = TextAlignmentOptions.Center;
                    feedbackText.enableWordWrapping = true;

                    RectTransform textRect = feedbackText.GetComponent<RectTransform>();
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.offsetMin = new Vector2(15, 10);
                    textRect.offsetMax = new Vector2(-15, -10);
                    textRect.localScale = Vector3.one;

                    EditorUtility.SetDirty(feedbackText);
                }
            }

            feedbackPanel.SetActive(false);
            EditorUtility.SetDirty(feedbackPanel);
        }

        // Desactivar el panel completo por defecto
        questionPanel.SetActive(false);

        EditorUtility.SetDirty(questionUI);
        EditorUtility.SetDirty(questionPanel);

        Debug.Log("✅ Layout de Question UI arreglado correctamente!");
    }

    private static void FixText(GameObject parent, string textName, float fontSize, float preferredHeight, 
                                TextAlignmentOptions alignment, Color color, bool flexibleHeight = false)
    {
        Transform textTransform = parent.transform.Find(textName);
        if (textTransform == null)
        {
            Debug.LogWarning($"No se encontró {textName}");
            return;
        }

        TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.fontSize = fontSize;
            text.enableAutoSizing = false;
            text.alignment = alignment;
            text.enableWordWrapping = true;
            text.color = color;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.localScale = Vector3.one;

            LayoutElement layout = textTransform.GetComponent<LayoutElement>();
            if (layout == null) layout = textTransform.gameObject.AddComponent<LayoutElement>();
            
            layout.preferredHeight = preferredHeight;
            layout.minHeight = preferredHeight;
            
            if (flexibleHeight)
            {
                layout.flexibleHeight = 1;
            }

            EditorUtility.SetDirty(text);
            EditorUtility.SetDirty(textTransform.gameObject);
        }
    }

    private static void FixButton(GameObject parent, string buttonName, float fontSize, float buttonHeight)
    {
        Transform buttonTransform = parent.transform.Find(buttonName);
        if (buttonTransform == null)
        {
            Debug.LogWarning($"No se encontró {buttonName}");
            return;
        }

        GameObject buttonObj = buttonTransform.gameObject;
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.localScale = Vector3.one;

        LayoutElement layout = buttonObj.GetComponent<LayoutElement>();
        if (layout == null) layout = buttonObj.AddComponent<LayoutElement>();
        
        layout.preferredHeight = buttonHeight;
        layout.minHeight = buttonHeight;

        // Arreglar texto del botón
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.fontSize = fontSize;
            buttonText.enableAutoSizing = false;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.enableWordWrapping = true;

            RectTransform textRect = buttonText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);
            textRect.localScale = Vector3.one;

            EditorUtility.SetDirty(buttonText);
        }

        EditorUtility.SetDirty(buttonObj);
    }
#endif
}

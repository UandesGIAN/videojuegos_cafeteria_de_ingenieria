using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class QuestionUI : MonoBehaviour
{
    [Header("Question Panel")]
    public GameObject questionPanel;
    public TextMeshProUGUI categoryText;
    public TextMeshProUGUI questionText;
    
    [Header("Answer Buttons")]
    public Button[] answerButtons; 
    public TextMeshProUGUI[] answerTexts; 
    
    [Header("Feedback")]
    public TextMeshProUGUI feedbackText;
    public GameObject feedbackPanel;
    public float feedbackDuration = 2.5f;
    
    [Header("Question Pool")]
    public QuestionPool questionPool;
    
    // Eventos
    public Action<bool> OnQuestionAnswered; 
    
    private Question currentQuestion;
    private int correctAnswer;
    private bool isAnswered = false;
    
    void Awake()
    {

        if (questionPanel != null) questionPanel.SetActive(false);
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
        

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; 
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(index));
        }
    }
    
    public void ShowQuestion()
    {
        if (questionPool == null)
        {
            Debug.LogError("QuestionPool no está asignado!");
            OnQuestionAnswered?.Invoke(true); 
            return;
        }
        
        // Obtener pregunta aleatoria
        currentQuestion = questionPool.GetRandomQuestion();
        
        if (currentQuestion == null)
        {
            Debug.LogError("No se pudo obtener una pregunta!");
            OnQuestionAnswered?.Invoke(true);
            return;
        }
        
        isAnswered = false;
        
        // Mostrar panel
        questionPanel.SetActive(true);
        
        // Configurar UI
        if (categoryText != null) categoryText.text = "Categoría: " + currentQuestion.category;
        questionText.text = currentQuestion.questionText;
        correctAnswer = currentQuestion.correctAnswerIndex;
        
        // Configurar respuestas
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = currentQuestion.answers[i];
                answerButtons[i].interactable = true;
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
        
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
    }
    
    private void OnAnswerButtonClicked(int selectedIndex)
    {
        if (isAnswered) return; 
        isAnswered = true;
        
        // Deshabilitar todos los botones
        foreach (var btn in answerButtons)
        {
            btn.interactable = false;
        }
        
        bool isCorrect = (selectedIndex == correctAnswer);
        
        // Mostrar feedback y continuar
        StartCoroutine(ShowFeedbackAndContinue(isCorrect, selectedIndex));
    }
    
    private IEnumerator ShowFeedbackAndContinue(bool correct, int selectedIndex)
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(true);
            
            if (correct)
            {
                feedbackText.text = "¡CORRECTO!\n+10% Ataque Físico y Especial para esta batalla";
                feedbackText.color = Color.green;
            }
            else
            {
                feedbackText.text = "INCORRECTO\n-10% Ataque Físico y Especial para esta batalla";
                feedbackText.color = Color.red;
            }
        }
        
        yield return new WaitForSeconds(feedbackDuration);
        
        HideQuestion();
        
        // Notificar resultado
        OnQuestionAnswered?.Invoke(correct);
    }
    
    public void HideQuestion()
    {
        if (questionPanel != null) questionPanel.SetActive(false);
        if (feedbackPanel != null) feedbackPanel.SetActive(false);
        isAnswered = false;
    }
}

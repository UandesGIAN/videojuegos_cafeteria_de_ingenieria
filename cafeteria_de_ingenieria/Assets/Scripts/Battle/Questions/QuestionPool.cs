using UnityEngine;

[CreateAssetMenu(fileName = "QuestionPool", menuName = "Battle/Question Pool")]
public class QuestionPool : ScriptableObject
{
    public Question[] allQuestions;

    public Question GetRandomQuestion()
    {
        if (allQuestions == null || allQuestions.Length == 0)
        {
            Debug.LogError("No hay preguntas en el pool!");
            return null;
        }
        
        int randomIndex = Random.Range(0, allQuestions.Length);
        return allQuestions[randomIndex];
    }

    public Question GetRandomQuestionByCategory(string category)
    {
        Question[] filtered = System.Array.FindAll(allQuestions, q => q.category == category);
        
        if (filtered.Length == 0)
        {
            Debug.LogWarning($"No hay preguntas para la categoría {category}, devolviendo pregunta aleatoria");
            return GetRandomQuestion();
        }
        
        return filtered[Random.Range(0, filtered.Length)];
    }
}

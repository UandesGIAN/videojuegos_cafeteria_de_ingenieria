using UnityEngine;

[CreateAssetMenu(fileName = "QuestionPool", menuName = "Battle/Question Pool")]
public class QuestionPool : ScriptableObject
{
    [Header("Todas las Preguntas")]
    public Question[] allQuestions;
    
    [Header("Preguntas por Categoría")]
    [Tooltip("Preguntas de Ingeniería Civil")]
    public Question[] civilQuestions;
    
    [Tooltip("Preguntas de Computación")]
    public Question[] computacionQuestions;
    
    [Tooltip("Preguntas de Ingeniería Ambiental")]
    public Question[] ambientalQuestions;
    
    [Tooltip("Preguntas de Obras")]
    public Question[] obrasQuestions;
    
    [Tooltip("Preguntas de Ingeniería Eléctrica")]
    public Question[] electricaQuestions;
    
    [Tooltip("Preguntas de Plan Común")]
    public Question[] planComunQuestions;
    
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
        Question[] questionsToUse = null;
        
        switch (category.ToLower())
        {
            case "civil":
                questionsToUse = civilQuestions;
                break;
            case "computacion":
            case "computación":
                questionsToUse = computacionQuestions;
                break;
            case "ambiental":
                questionsToUse = ambientalQuestions;
                break;
            case "obras":
                questionsToUse = obrasQuestions;
                break;
            case "electrica":
            case "eléctrica":
                questionsToUse = electricaQuestions;
                break;
            case "plancomun":
            case "plan comun":
            case "plan común":
                questionsToUse = planComunQuestions;
                break;
            default:
                Debug.LogWarning($"Categoría '{category}' no reconocida. Buscando en todas las preguntas...");
                Question[] filtered = System.Array.FindAll(allQuestions, q => q.category.ToLower() == category.ToLower());
                
                if (filtered.Length > 0)
                {
                    return filtered[Random.Range(0, filtered.Length)];
                }
                
                Debug.LogWarning($"No hay preguntas para la categoría {category}, devolviendo pregunta aleatoria");
                return GetRandomQuestion();
        }
        
        if (questionsToUse == null || questionsToUse.Length == 0)
        {
            Debug.LogWarning($"No hay preguntas específicas para {category}, buscando en todas las preguntas...");
            Question[] filtered = System.Array.FindAll(allQuestions, q => q.category.ToLower() == category.ToLower());
            
            if (filtered.Length > 0)
            {
                return filtered[Random.Range(0, filtered.Length)];
            }
            
            Debug.LogWarning($"No hay preguntas para la categoría {category}, devolviendo pregunta aleatoria");
            return GetRandomQuestion();
        }
        
        return questionsToUse[Random.Range(0, questionsToUse.Length)];
    }
    
    public string[] GetAvailableCategories()
    {
        return new string[] { "Civil", "Computacion", "Ambiental", "Obras", "Electrica", "PlanComun" };
    }
    
    public bool HasQuestionsForCategory(string category)
    {
        Question[] questions = GetQuestionsByCategory(category);
        return questions != null && questions.Length > 0;
    }
    
    private Question[] GetQuestionsByCategory(string category)
    {
        switch (category.ToLower())
        {
            case "civil":
                return civilQuestions;
            case "computacion":
            case "computación":
                return computacionQuestions;
            case "ambiental":
                return ambientalQuestions;
            case "obras":
                return obrasQuestions;
            case "electrica":
            case "eléctrica":
                return electricaQuestions;
            case "plancomun":
            case "plan comun":
            case "plan común":
                return planComunQuestions;
            default:
                return allQuestions;
        }
    }
}

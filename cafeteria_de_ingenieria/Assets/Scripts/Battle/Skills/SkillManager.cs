using UnityEngine;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    [Header("Skill Pool")]
    public GameObject skillPoolContainer;
    
    [Header("Player Configuration")]
    public FighterStats player;

    public string[] playerSkillNames;

    [Header("Battle UI Reference")]
    public BattleUI battleUI;
    void Start()
    {
        AssignSkillsToPlayer();
    }
    
    public void AssignSkillsToPlayer()
    {
        if (player == null)
        {
            Debug.LogError("SkillManager: No se ha asignado un Player");
            return;
        }
        
        if (skillPoolContainer == null)
        {
            Debug.LogError("SkillManager: No se ha asignado un Skill Pool Container");
            return;
        }
        
        Transform playerSkillsContainer = player.transform.Find("Skills");
        
        if (playerSkillsContainer == null)
        {
            Debug.LogError($"SkillManager: No se encontró el GameObject 'Skills' en {player.name}");
            return;
        }
        
        foreach (Transform child in playerSkillsContainer)
        {
            Destroy(child.gameObject);
        }
        
        Skill[] allSkills = skillPoolContainer.GetComponentsInChildren<Skill>(true);
        
        if (allSkills.Length == 0)
        {
            Debug.LogWarning("SkillManager: No se encontraron skills en el Skill Pool");
            return;
        }
        
        Debug.Log($"SkillManager: Se encontraron {allSkills.Length} skills en el pool");
        
        int assignedCount = 0;
        foreach (string skillName in playerSkillNames)
        {
            // Buscar la skill en el pool
            Skill skillTemplate = allSkills.FirstOrDefault(s => s.skillName == skillName);
            
            if (skillTemplate != null)
            {
                // Crear una copia de la skill
                GameObject skillCopy = Instantiate(skillTemplate.gameObject, playerSkillsContainer);
                skillCopy.name = skillName; 
                skillCopy.SetActive(true);

                assignedCount++;

                player.RefreshSkills();
                battleUI.SetupSkillButtons(player);

                Debug.Log($"Skill '{skillName}' instanciada para {player.fightername}");
            }
            else
            {
                Debug.LogWarning($"Skill '{skillName}' no encontrada en el pool. Skills disponibles: {string.Join(", ", allSkills.Select(s => s.skillName))}");
            }
        }
        
        Debug.Log($"SkillManager: {assignedCount} de {playerSkillNames.Length} skills asignadas a {player.fightername}");
    }
    
    public void AddSkillToPlayer(string skillName)
    {
        Transform playerSkillsContainer = player.transform.Find("Skills");
        if (playerSkillsContainer == null) return;

        Skill[] allSkills = skillPoolContainer.GetComponentsInChildren<Skill>(true);
        Skill[] playerSkills = player.GetSkills();
        Skill skillTemplate = allSkills.FirstOrDefault(s => s.skillName == skillName);

        if (skillTemplate != null && !playerSkills.Any(s => s.skillName == skillName))
        {
            GameObject skillCopy = Instantiate(skillTemplate.gameObject, playerSkillsContainer);
            skillCopy.name = skillName;
            skillCopy.SetActive(true);

            player.RefreshSkills();
            battleUI.SetupSkillButtons(player);
            
            Debug.Log($"Skill '{skillName}' agregada a {player.fightername}");
        }
        else
        {
            Debug.LogWarning($"No se pudo agregar la skill '{skillName}' - no existe en el pool o ya está asignada");
        }
    }
    
    public void RemoveSkillFromPlayer(string skillName)
    {
        Transform playerSkillsContainer = player.transform.Find("Skills");
        if (playerSkillsContainer == null) return;
        
        foreach (Transform child in playerSkillsContainer)
        {
            Skill skill = child.GetComponent<Skill>();
            if (skill != null && skill.skillName == skillName)
            {
                Destroy(child.gameObject);

                player.RefreshSkills();
                battleUI.SetupSkillButtons(player);

                Debug.Log($"Skill '{skillName}' removida de {player.fightername}");
                return;
            }
        }
        
        Debug.LogWarning($"Skill '{skillName}' no encontrada en {player.fightername}");
    }
}

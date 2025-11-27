using UnityEngine;
using System.Linq;
using System;
using System.Collections;

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
    
    void Update()
    {
        // Cheat: CTRL + S + D
        if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKey(KeyCode.S) &&
            Input.GetKeyDown(KeyCode.D))
        {
            GiveRandomSkills(4);
        }
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
    
    public void AddSkillToPlayer(string skillName, UpgradeSelection caller = null)
    {
        Transform playerSkillsContainer = player.transform.Find("Skills");
        if (playerSkillsContainer == null) return;

        Skill[] allSkills = skillPoolContainer.GetComponentsInChildren<Skill>(true);
        player.RefreshSkills();
        Skill[] playerSkills = player.GetSkills();
        Skill skillTemplate = allSkills.FirstOrDefault(s => s.skillName == skillName);

        if (skillTemplate == null)
        {
            Debug.LogWarning($"No existe la skill '{skillName}' en el pool.");
            return;
        }

        // Si ya la tiene, no agregarla
        if (playerSkills.Any(s => s.skillName == skillName))
        {
            Debug.Log($"El jugador ya tiene la skill '{skillName}', no se agrega.");
            return;
        }

        // Pedir reemplazo si ya tiene 4 habilidades
        if (playerSkills.Length >= 4)
        {
            Debug.Log("Jugador ya tiene 4 skills. Iniciando proceso de reemplazo...");
            battleUI.gameObject.SetActive(true);
            RequestSkillReplacement(skillName, caller);
            return;
        }

        // Si tiene espacio, agregar normalmente
        GameObject skillCopy = Instantiate(skillTemplate.gameObject, playerSkillsContainer);
        skillCopy.name = skillName;
        skillCopy.SetActive(true);

        player.RefreshSkills();
        battleUI.SetupSkillButtons(player);

        Debug.Log($"Skill '{skillName}' agregada a {player.fightername}");
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

    // CHEAT PARA TENER 4 HABILIDADES
    public void GiveRandomSkills(int amount = 4)
    {
        Skill[] allSkills = skillPoolContainer.GetComponentsInChildren<Skill>(true);
        if (allSkills.Length == 0)
        {
            Debug.LogError("SkillManager: No hay skills en el pool para elegir.");
            return;
        }

        // Limpiar skills actuales
        Transform playerSkillsContainer = player.transform.Find("Skills");
        foreach (Transform child in playerSkillsContainer)
            Destroy(child.gameObject);

        // Seleccionar random sin repetir
        var selected = allSkills
            .OrderBy(x => UnityEngine.Random.value)
            .Take(amount)
            .ToArray();

        foreach (Skill template in selected)
        {
            GameObject copy = Instantiate(template.gameObject, playerSkillsContainer);
            copy.name = template.skillName;
            copy.SetActive(true);
        }

        player.RefreshSkills();
        battleUI.SetupSkillButtons(player);

        Debug.Log("Cheat: se asignaron " + amount + " habilidades aleatorias al jugador.");
    }

    public void RequestSkillReplacement(string newSkillName, UpgradeSelection caller)
    {
        StartCoroutine(ReplacementRoutine(newSkillName, caller));
    }

    private IEnumerator ReplacementRoutine(string newSkillName, UpgradeSelection caller)
    {
        player.RefreshSkills();
        Skill[] currentSkills = player.GetSkills();

        // Mostrar popup
        battleUI.ShowSkillReplacePopup(currentSkills);

        // Esperar a que se seleccione una skill
        while (battleUI.selectedReplaceIndex == -1)
            yield return null;

        int index = battleUI.selectedReplaceIndex;

        // Reiniciar índice para la próxima vez
        battleUI.selectedReplaceIndex = -1;

        // Reemplazar skill
        ReplaceSkill(index, newSkillName, caller);

        // Asegurarse de que el popup esté cerrado
        battleUI.skillReplacementPopup.SetActive(false);
        battleUI.skillReplacementText.gameObject.SetActive(false);
    }
    
    private void ReplaceSkill(int index, string newSkillName, UpgradeSelection caller)
    {
        player.RefreshSkills();
        Skill[] currentSkills = player.GetSkills();

        if (index < 0 || index >= currentSkills.Length)
        {
            Debug.LogError("Índice de reemplazo inválido");
            return;
        }

        string oldName = currentSkills[index].skillName;

        // Remover skill antigua primero
        RemoveSkillFromPlayer(oldName);

        // Ahora sí agregar la nueva SIN pedir reemplazo
        Transform playerSkillsContainer = player.transform.Find("Skills");
        Skill[] allSkills = skillPoolContainer.GetComponentsInChildren<Skill>(true);
        Skill skillTemplate = allSkills.FirstOrDefault(s => s.skillName == newSkillName);

        if (skillTemplate == null)
        {
            Debug.LogError($"Skill '{newSkillName}' no encontrada en el pool.");
            return;
        }

        GameObject skillCopy = Instantiate(skillTemplate.gameObject, playerSkillsContainer);
        skillCopy.name = newSkillName;
        skillCopy.SetActive(true);

        player.RefreshSkills();
        battleUI.SetupSkillButtons(player);

        Debug.Log($"Skill '{oldName}' reemplazada por '{newSkillName}'");

        caller?.OnSkillReplaced();
    }
}

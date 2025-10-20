using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UpgradeSelection : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject[] upgradeButtons;
    public TMP_Text[] upgradeNames;
    public TMP_Text[] upgradeDescriptions;
    public Image[] upgradeIcons;
    public GameObject SkillPoolContainer;
    public GameObject UpgradePoolContainer;
    public RoomController currentRoom;
    
    [Header("Player Reference")]
    public FighterStats player;
    
    [Header("Skill Manager")]
    public SkillManager skillManager;

    [Header("Configuración")]
    public int numberOfRewardsToShow = 3;

    private List<Reward> selectedRewards = new List<Reward>();

    public class Reward
    {
        public enum RewardType { Skill, Upgrade }
        
        public RewardType type;
        public Skill skill;
        public Upgrade upgrade;
        
        public string GetName()
        {
            return type == RewardType.Skill ? skill.skillName : upgrade.upgradeName;
        }
        
        public string GetDescription()
        {
            return type == RewardType.Skill ? skill.description : upgrade.description;
        }
        
        public Sprite GetIcon()
        {
            return type == RewardType.Upgrade ? upgrade.icon : null;
        }
    }

    public void ShowUpgrades()
    {
        gameObject.SetActive(true);
        GenerateRandomRewards();
        DisplayRewards();
        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        // Configurar los listeners de los botones
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            Button button = upgradeButtons[i].GetComponent<Button>();
            if (button != null)
            {
                //remover los listeners previos para evitar duplicados
                button.onClick.RemoveAllListeners();

                int index = i;

                // Agregar el listener al boton de index i
                button.onClick.AddListener(() => SelectUpgrade(index));
            }
            else
            {
                Debug.LogWarning($"UpgradeSelection: upgradeButtons[{i}] no tiene componente Button");
            }
        }
    }

    private void GenerateRandomRewards()
    {
        selectedRewards.Clear();
        
        // Obtener todas las skills y upgrades disponibles
        List<Reward> allPossibleRewards = GetAllPossibleRewards();
        
        // Filtrar las que el jugador ya tiene (para skills)
        List<Reward> availableRewards = FilterAvailableRewards(allPossibleRewards);
        
        // Seleccionar aleatoriamente
        int rewardsToSelect = Mathf.Min(numberOfRewardsToShow, availableRewards.Count);
        
        for (int i = 0; i < rewardsToSelect; i++)
        {
            if (availableRewards.Count > 0)
            {
                int randomIndex = Random.Range(0, availableRewards.Count);
                selectedRewards.Add(availableRewards[randomIndex]);
                availableRewards.RemoveAt(randomIndex); // Evitar duplicados
            }
        }
    }

    private List<Reward> GetAllPossibleRewards()
    {
        List<Reward> allRewards = new List<Reward>();
        
        // Obtener todas las skills del pool
        if (SkillPoolContainer != null)
        {
            Skill[] allSkills = SkillPoolContainer.GetComponentsInChildren<Skill>(true);
            foreach (Skill skill in allSkills)
            {
                allRewards.Add(new Reward 
                { 
                    type = Reward.RewardType.Skill, 
                    skill = skill 
                });
            }
        }
        
        // Obtener todos los upgrades del pool
        if (UpgradePoolContainer != null)
        {
            Upgrade[] allUpgrades = UpgradePoolContainer.GetComponentsInChildren<Upgrade>(true);
            foreach (Upgrade upgrade in allUpgrades)
            {
                allRewards.Add(new Reward 
                { 
                    type = Reward.RewardType.Upgrade, 
                    upgrade = upgrade 
                });
            }
        }
        
        return allRewards;
    }

    private List<Reward> FilterAvailableRewards(List<Reward> allRewards)
    {
        if (player == null)
        {
            Debug.LogWarning("UpgradeSelection: Player no asignado, no se puede filtrar skills");
            return allRewards;
        }
        
        // Obtener skills actuales del jugador
        Transform playerSkillsContainer = player.transform.Find("Skills");
        Skill[] playerSkills = playerSkillsContainer != null 
            ? playerSkillsContainer.GetComponentsInChildren<Skill>() 
            : new Skill[0];
        
        // Filtrar skills que el jugador ya tiene
        List<Reward> availableRewards = allRewards.Where(reward =>
        {
            if (reward.type == Reward.RewardType.Skill)
            {
                // Solo incluir si el jugador NO tiene esta skill
                return !playerSkills.Any(ps => ps.skillName == reward.skill.skillName);
            }
            // Los upgrades siempre están disponibles
            return true;
        }).ToList();
        
        return availableRewards;
    }

    private void DisplayRewards()
    {
        // Mostrar las recompensas en la UI
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < selectedRewards.Count)
            {
                upgradeButtons[i].SetActive(true);
                
                Reward reward = selectedRewards[i];
                upgradeNames[i].text = reward.GetName();
                upgradeDescriptions[i].text = reward.GetDescription();
                
                // Mostrar ícono si existe
                if (upgradeIcons[i] != null)
                {
                    Sprite icon = reward.GetIcon();
                    if (icon != null)
                    {
                        upgradeIcons[i].sprite = icon;
                        upgradeIcons[i].enabled = true;
                    }
                    else
                    {
                        upgradeIcons[i].enabled = false;
                    }
                }
                
                // Agregar indicador de tipo
                string typeIndicator = reward.type == Reward.RewardType.Skill ? "[SKILL]" : "[UPGRADE]";
                upgradeNames[i].text = $"{typeIndicator} {reward.GetName()}";
            }
            else
            {
                upgradeButtons[i].SetActive(false);
            }
        }
    }

    public void SelectUpgrade(int index)
    {
        Debug.Log($"*** SelectUpgrade llamado con índice: {index} ***");
        
        if (index < 0 || index >= selectedRewards.Count)
        {
            return;
        }
        
        Reward chosenReward = selectedRewards[index];
        Debug.Log($"UpgradeSelection: Recompensa seleccionada - Tipo: {chosenReward.type}, Nombre: {chosenReward.GetName()}");

        // Aplicar la recompensa según su tipo
        if (chosenReward.type == Reward.RewardType.Skill)
        {
            ApplySkillReward(chosenReward.skill);
        }
        else if (chosenReward.type == Reward.RewardType.Upgrade)
        {
            ApplyUpgradeReward(chosenReward.upgrade);
        }
        
        // Ocultar el panel de selección
        gameObject.SetActive(false);
        
        if (currentRoom != null)
        {
            currentRoom.OnUpgradeSelected();
        }
        else
        {
            Debug.LogError("UpgradeSelection: currentRoom es null, no se puede continuar");
        }
    }

    private void ApplySkillReward(Skill skill)
    {
        if (skillManager != null)
        {
            skillManager.AddSkillToPlayer(skill.skillName);
        }
        else
        {
            Debug.LogError("UpgradeSelection: SkillManager no asignado, no se puede agregar skill");
        }
    }

    private void ApplyUpgradeReward(Upgrade upgrade)
    {      
        if (player != null)
        {
            upgrade.Apply(player);
        }
        else
        {
            Debug.LogError("UpgradeSelection: Player no asignado, no se puede aplicar upgrade");
        }
    }
}


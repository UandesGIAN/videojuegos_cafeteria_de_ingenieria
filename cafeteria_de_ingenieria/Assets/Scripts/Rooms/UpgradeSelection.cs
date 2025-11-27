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
    public GameObject ItemsPoolContainer;
    public GameObject UpgradePoolContainer;
    public RoomController currentRoom;
    
    [Header("Player Reference")]
    public FighterStats player;

    [Header("Skill Manager")]
    public SkillManager skillManager;

    [Header("Items Manager")]
    public ItemManager ItemsManager;

    [Header("Configuración")]
    public int numberOfRewardsToShow = 3;
    private bool waitingForSkillReplace = false;

    private List<Reward> selectedRewards = new List<Reward>();

    public class Reward
    {
        public enum RewardType { Skill, Item, Upgrade }
        
        public RewardType type;
        public Skill skill;
        public Item item;
        public Upgrade upgrade;

        
         public string GetName()
        {
            switch (type)
            {
                case RewardType.Skill:
                    return skill != null ? skill.skillName : "Unknown Skill";
                case RewardType.Item:
                    return item != null ? item.itemName : "Unknown Item";
                case RewardType.Upgrade:
                    return upgrade != null ? upgrade.upgradeName : "Unknown Upgrade";
                default:
                    return "Unknown Reward";
            }
        }

        public string GetDescription()
        {
            switch (type)
            {
                case RewardType.Skill:
                    return skill != null ? skill.description : "";
                case RewardType.Item:
                    return item != null ? item.description : "";
                case RewardType.Upgrade:
                    return upgrade != null ? upgrade.description : "";
                default:
                    return "";
            }
        }

        public Sprite GetIcon()
        {
            switch (type)
            {
                case RewardType.Skill:
                    return skill != null ? skill.icon : null;
                case RewardType.Item:
                    return item != null ? item.icon : null;
                case RewardType.Upgrade:
                    return upgrade != null ? upgrade.icon : null;
                default:
                    return null;
            }
        }
    }

    private void Update()
    {
        // Si estamos esperando reemplazo de skill bloquear interacción con los botones
        if (waitingForSkillReplace)
        {
            foreach (var btn in upgradeButtons)
            {
                if (btn != null)
                {
                    var buttonComp = btn.GetComponent<Button>();
                    if (buttonComp != null)
                        buttonComp.interactable = false;
                }
            }

            // Cancelar con ESC o click derecho
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                Debug.Log("UpgradeSelection: reemplazo cancelado por ");
                waitingForSkillReplace = false;

                // Habilitar botones de nuevo
                foreach (var btn in upgradeButtons)
                {
                    if (btn != null)
                    {
                        var buttonComp = btn.GetComponent<Button>();
                        if (buttonComp != null)
                            buttonComp.interactable = true;
                    }
                }

                // Cerrar popup de reemplazo si existe
                if (skillManager != null && skillManager.battleUI != null)
                    skillManager.battleUI.skillReplacementPopup.SetActive(false);
            }
        }
        else
        {
            // Botones interactuables
            foreach (var btn in upgradeButtons)
            {
                if (btn != null)
                {
                    var buttonComp = btn.GetComponent<Button>();
                    if (buttonComp != null)
                        buttonComp.interactable = true;
                }
            }
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
                // Agregar componente de sonido si no existe
                if (button.GetComponent<UIButtonSound>() == null)
                {
                    UIButtonSound buttonSound = button.gameObject.AddComponent<UIButtonSound>();
                    buttonSound.soundType = UIButtonSound.ButtonSoundType.Confirm;
                }
                
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
        
        // Obtener todas las skills, objetos y upgrades disponibles
        List<Reward> allPossibleRewards = GetAllPossibleRewards();
        
        // Filtrar las que el jugador ya tiene (para skills)
        List<Reward> availableRewards = FilterAvailableRewards(allPossibleRewards);
        
        // Separar por tipo
        List<Reward> skillRewards = availableRewards.Where(r => r.type == Reward.RewardType.Skill).ToList();
        List<Reward> nonSkillRewards = availableRewards.Where(r => r.type != Reward.RewardType.Skill).ToList();
        
        // Seleccionar máximo 1 skill
        int skillsToAdd = Mathf.Min(1, skillRewards.Count);
        for (int i = 0; i < skillsToAdd; i++)
        {
            if (skillRewards.Count > 0)
            {
                int randomIndex = Random.Range(0, skillRewards.Count);
                selectedRewards.Add(skillRewards[randomIndex]);
                skillRewards.RemoveAt(randomIndex);
            }
        }
        
        // Completar con items/upgrades hasta llegar a numberOfRewardsToShow
        int remainingSlots = numberOfRewardsToShow - selectedRewards.Count;
        for (int i = 0; i < remainingSlots; i++)
        {
            if (nonSkillRewards.Count > 0)
            {
                int randomIndex = Random.Range(0, nonSkillRewards.Count);
                selectedRewards.Add(nonSkillRewards[randomIndex]);
                nonSkillRewards.RemoveAt(randomIndex);
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
        
        // Obtener todos los items del pool
        if (ItemsPoolContainer != null)
        {
            Item[] allItems = ItemsPoolContainer.GetComponentsInChildren<Item>(true);
            foreach (Item it in allItems)
            {
                allRewards.Add(new Reward 
                { 
                    type = Reward.RewardType.Item, 
                    item = it
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
            // Los upgrades y objetos siempre están disponibles
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
                string typeIndicator = reward.type == Reward.RewardType.Skill ? "[SKILL]" :
                       reward.type == Reward.RewardType.Item ? "[ITEM]" :
                       "[UPGRADE]";
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
            waitingForSkillReplace = false;
            ApplySkillReward(chosenReward.skill);

            if (waitingForSkillReplace)
            {
                // IMPORTANTE: NO cerrar UI TODAVÍA
                Debug.Log("UpgradeSelection: esperando reemplazo, NO se cierra la UI ni se avanza de sala.");
                return;
            }
        }
        else if (chosenReward.type == Reward.RewardType.Item)
        {
            ApplyItemReward(chosenReward.item);
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
        if (skillManager == null)
        {
            Debug.LogError("UpgradeSelection: SkillManager no asignado, no se puede agregar skill");
            return;
        }

        bool needsReplacement = player.GetSkills().Length >= 4;

        skillManager.AddSkillToPlayer(skill.skillName, this);

        if (needsReplacement)
        {
            Debug.Log("UpgradeSelection: Skill requiere reemplazo, esperando elección del jugador.");
            waitingForSkillReplace = true;
        }
    }

    private void ApplyItemReward(Item item)
    {
        if (ItemsManager != null)
        {
            ItemsManager.AddItemToPlayer(item.itemName);
        }
        else
        {
            Debug.LogError("UpgradeSelection: ItemManager no asignado, no se puede agregar item");
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

    public void OnSkillReplaced()
    {
        Debug.Log("UpgradeSelection: Reemplazo de skill completo, continuando flujo.");

        // Ahora sí cerramos el panel de upgrades
        gameObject.SetActive(false);

        if (currentRoom != null)
            currentRoom.OnUpgradeSelected();
    }
}


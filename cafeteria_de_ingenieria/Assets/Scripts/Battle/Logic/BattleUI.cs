using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Diagnostics.Tracing;

public class BattleUI : MonoBehaviour
{
    [Header("Action Menu")]
    public TextMeshProUGUI[] actionOptions;
    public event Action<int> OnOptionClicked;
    public event Action<int> OnOptionHovered;
    int selectedOption = 0;

    [Header("Popups")]
    public GameObject skillPopup;
    public GameObject itemPopup;
    public GameObject skillReplacementPopup;
    public TMP_Text skillReplacementText;

    public GameObject itemListContainer;
    public GameObject itemButtonPrefab;

    // Se llenan automáticamente desde los popups
    [HideInInspector] public Button[] skillButtons;
    [HideInInspector] public TextMeshProUGUI[] skillButtonLabels;
    [HideInInspector] public Button[] skillReplacementButtons;
    [HideInInspector] public TextMeshProUGUI[] skillReplacementButtonLabels;

    [Header("HUD del jugador")]
    public Image playerSprite;
    public TMP_Text playerName;
    public Slider playerHP;
    public Slider playerIQ;

    [Header("HUD del enemigo")]
    public Image enemySprite;
    public TMP_Text enemyName;
    public Slider enemyHP;
    public Slider enemyIQ;

    [Header("Mensajes")]
    public TMP_Text message;

    // Jugador
    public FighterStats currentPlayer;

    // Evento cuando se selecciona una skill
    public Action<int> OnSkillSelected;

    // Evento cuando se selecciona un item
    public Action<Item> OnItemSelected;

    // Enemigo actual
    private FighterStats currentEnemy;
    
    public int selectedReplaceIndex = -1;

    void Awake()
    {
        // Asocia clic y hover a todos los botones automáticamente
        for (int i = 0; i < actionOptions.Length; i++)
        {
            int index = i;
            var option = actionOptions[i];

            // Asegura que tenga un EventTrigger
            EventTrigger trigger = option.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = option.gameObject.AddComponent<EventTrigger>();

            // Limpia triggers anteriores
            trigger.triggers.Clear();

            // Hover
            var entryHover = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryHover.callback.AddListener((_) => OnOptionHovered?.Invoke(index));
            trigger.triggers.Add(entryHover);

            // Click
            var entryClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entryClick.callback.AddListener((_) => OnOptionClicked?.Invoke(index));
            trigger.triggers.Add(entryClick);
        }

        // Buscar dinámicamente todos los botones y labels de skills
        skillButtons = skillPopup.GetComponentsInChildren<Button>(true);
        skillButtonLabels = new TextMeshProUGUI[skillButtons.Length];

        for (int i = 0; i < skillButtons.Length; i++)
        {
            var label = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) skillButtonLabels[i] = label;
        }

        skillReplacementButtons = skillReplacementPopup.GetComponentsInChildren<Button>(true);
        skillReplacementButtonLabels = new TextMeshProUGUI[skillReplacementButtons.Length];

        for (int i = 0; i < skillReplacementButtons.Length; i++)
        {
            var label = skillReplacementButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) skillReplacementButtonLabels[i] = label;
        }

        if (skillReplacementPopup != null)
        skillReplacementPopup.SetActive(false);

        if (skillReplacementText != null)
            skillReplacementText.gameObject.SetActive(false);

        // Para items, buscamos el contenedor donde pondremos la lista
        itemListContainer = itemPopup.transform.Find("ItemListContainer")?.gameObject;

        // Para el jugador
        playerSprite.sprite = currentPlayer.img;
        playerName.text = currentPlayer.fightername;
        playerHP.maxValue = currentPlayer.startHealth;
        playerHP.value = currentPlayer.health;

        playerIQ.maxValue = currentPlayer.startIQ;
        playerIQ.value = currentPlayer.IQ;

        // Suscribirse a los eventos de cambio de salud e IQ
        currentPlayer.OnHealthChanged += UpdatePlayerHealth;
        currentPlayer.OnIQChanged += UpdatePlayerIQ;
    }
    
    // HUD
    public void SetSelectedOption(int index)
    {
        selectedOption = index;
        UpdateHighlight();
    }

    public void UpdateHighlight()
    {
        if (actionOptions == null || actionOptions.Length == 0) return;

        for (int i = 0; i < actionOptions.Length; i++)
        {
            if (actionOptions[i] == null) continue;
            actionOptions[i].color = (i == selectedOption)
                ? new Color(0.5f, 1f, 1f)
                : Color.white;
        }
    }

    // Pop up habilidades para reemplazar
    public void ShowSkillReplacePopup(Skill[] currentSkills)
    {
        skillReplacementPopup.SetActive(true);
        skillReplacementText.gameObject.SetActive(true);
        selectedReplaceIndex = -1;

        for (int i = 0; i < skillReplacementButtons.Length; i++)
        {
            if (i < currentSkills.Length)
            {
                skillReplacementButtons[i].gameObject.SetActive(true);
                skillReplacementButtonLabels[i].text = currentSkills[i].skillName;

                int index = i;
                skillReplacementButtons[i].onClick.RemoveAllListeners();
                skillReplacementButtons[i].onClick.AddListener(() =>
                {
                    selectedReplaceIndex = index;
                    skillReplacementPopup.SetActive(false);
                });
            }
            else
            {
                skillReplacementButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Habilidades
    public void SetupSkillButtons(FighterStats fighter)
    {
        fighter.RefreshSkills();
        Skill[] skills = fighter.GetSkills();
        for (int i = 0; i < skillButtons.Length; i++)
        {
            // Agregar componente de sonido a botones de habilidad si no existe
            if (skillButtons[i].GetComponent<UIButtonSound>() == null)
            {
                UIButtonSound buttonSound = skillButtons[i].gameObject.AddComponent<UIButtonSound>();
                buttonSound.soundType = UIButtonSound.ButtonSoundType.Skill;
            }
            
            if (i < skills.Length)
            {
                skillButtons[i].gameObject.SetActive(true);
                skillButtonLabels[i].text = skills[i].skillName;
                if (skills[i] is HealthModSkill healthSkill)
                {
                    skillButtonLabels[i].text = $"{skills[i].skillName}. {healthSkill.amount}DAÑO/{healthSkill.cost}IQ.";
                }

                int capturedIndex = i;
                skillButtons[i].onClick.RemoveAllListeners();
                skillButtons[i].onClick.AddListener(() => OnSkillSelected?.Invoke(capturedIndex));
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // items
    public void SetupItemList(FighterStats fighter)
    {
        if (itemListContainer == null || fighter == null) return;

        // clean list
        foreach (Transform t in itemListContainer.transform)
            Destroy(t.gameObject);

        Debug.Log($"[BattleUI] Generando lista de items para: {fighter.fightername}");

        Item[] items = fighter.GetItems();
        if (items == null || items.Length == 0) return;

        Debug.Log($"[BattleUI] Se encontraron {items.Length} ítems.");

        foreach (var item in items)
        {
            Debug.Log($"[BattleUI] Creando botón para item: {item.itemName}");

            // Uso del prefab para hacer la lista
            GameObject itemButton = Instantiate(itemButtonPrefab, itemListContainer.transform);
            
            // Agregar componente de sonido al botón de item
            Button button = itemButton.GetComponent<Button>();
            if (button != null && button.GetComponent<UIButtonSound>() == null)
            {
                UIButtonSound buttonSound = button.gameObject.AddComponent<UIButtonSound>();
                buttonSound.soundType = UIButtonSound.ButtonSoundType.Normal;
            }

            TextMeshProUGUI nameText = itemButton.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI amountText = itemButton.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            if (nameText == null || amountText == null || button == null)
            {
                Debug.LogError($"[BattleUI] El prefab '{itemButton.name}' no tiene los componentes esperados (Name, Amount, Button).");
                continue;
            }
        
            // Asignar datos
            nameText.text = item.itemName;
            amountText.text = "x" + fighter.GetItemCount(item.itemName);
            Debug.Log($"[BattleUI] Item '{item.itemName}' agregado al HUD.");

            // Asignar evento
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Debug.Log($"[BattleUI] Usando item: {item.itemName}");
                OnItemSelected?.Invoke(item);
            });
        }
    }

    // Asignar enemigo
    public void SetupEnemy(FighterStats enemyStats)
    {
        Debug.Log("Setting up enemy in BattleUI");
        currentEnemy = enemyStats;
        Debug.Log("New enemy assigned: " + currentEnemy.fightername);

        enemySprite.sprite = enemyStats.img;
        //AssignEnemyNewComponents(enemyStats);

        enemyName.text = enemyStats.fightername;
        enemyHP.maxValue = enemyStats.health;
        enemyHP.value = enemyStats.health;
        
        if (enemyStats.IQ > 0)
        {
            enemyIQ.gameObject.SetActive(true);

            enemyIQ.maxValue = enemyStats.IQ;
            enemyIQ.value = enemyStats.IQ;
        }
        else
        {
            enemyIQ.gameObject.SetActive(false);
        }

        // Suscribirse a cambios de vida
        currentEnemy.OnHealthChanged += UpdateEnemyHealth;

        Debug.Log("Enemy setup complete in BattleUI:\n");
        currentEnemy.PrintStats();
    }

    private void UpdateEnemyHealth(float current, float max)
    {
        enemyHP.maxValue = max;
        enemyHP.value = current;
    }

    public void ClearEnemy()
    {
        if (currentEnemy != null)
        {
            currentEnemy.OnHealthChanged -= UpdateEnemyHealth;
            currentEnemy = null;
        }
    }

    // Metodos para el jugador
    private void UpdatePlayerHealth(float current, float max)
    {
        playerHP.maxValue = max;
        playerHP.value = current;
    }

    private void UpdatePlayerIQ(float current, float max)
    {
        playerIQ.maxValue = max;
        playerIQ.value = current;
    }

    public void ClearPlayer()
    {
        if (currentPlayer != null)
        {
            currentPlayer.OnHealthChanged -= UpdatePlayerHealth;
            currentPlayer.OnIQChanged -= UpdatePlayerIQ;
            currentPlayer = null;
        }
    }

    public void ResetUI(FighterStats playerStats, FighterStats enemyStats)
    {
        gameObject.SetActive(true);

        // Jugador
        currentPlayer = playerStats;
        playerSprite.sprite = playerStats.img;
        playerName.text = playerStats.fightername;
        playerHP.maxValue = playerStats.startHealth;
        playerHP.value = playerStats.health;
        playerIQ.maxValue = playerStats.startIQ;
        playerIQ.value = playerStats.IQ;

        currentPlayer.OnHealthChanged -= UpdatePlayerHealth;
        currentPlayer.OnIQChanged -= UpdatePlayerIQ;
        currentPlayer.OnHealthChanged += UpdatePlayerHealth;
        currentPlayer.OnIQChanged += UpdatePlayerIQ;

        // Enemigo
        currentEnemy = enemyStats;
        enemySprite.sprite = enemyStats.img;
        enemyName.text = enemyStats.fightername;
        enemyHP.maxValue = enemyStats.startHealth;
        enemyHP.value = enemyStats.health;
        enemyIQ.maxValue = enemyStats.startIQ;
        enemyIQ.value = enemyStats.IQ;

        currentEnemy.OnHealthChanged -= UpdateEnemyHealth;
        currentEnemy.OnHealthChanged += UpdateEnemyHealth;

        // Popups
        skillPopup.SetActive(false);
        itemPopup.SetActive(false);

        // Skills y items
        SetupSkillButtons(currentPlayer);
        SetupItemList(currentPlayer);
    }
}

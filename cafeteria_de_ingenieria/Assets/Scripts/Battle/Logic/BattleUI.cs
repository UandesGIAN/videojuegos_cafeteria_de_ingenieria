using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BattleUI : MonoBehaviour
{
    [Header("Action Menu")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI skillText;
    public TextMeshProUGUI itemText;

    [Header("Popups")]
    public GameObject skillPopup;
    public GameObject itemPopup;

    // Se llenan automáticamente desde los popups
    [HideInInspector] public Button[] skillButtons;
    [HideInInspector] public TextMeshProUGUI[] skillButtonLabels;
    [HideInInspector] public GameObject itemListContainer; // Para la lista de items

    [Header("HUD del jugador")]
    public Image playerSprite;
    public TMP_Text playerName;
    public Slider playerHP;
    public Slider playerIQ;

    [Header("HUD del enemigo")]
    public Image enemySprite;
    public TMP_Text enemyName;
    public Slider enemyHP;

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

    void Awake()
    {
        // Buscar dinámicamente todos los botones y labels de skills
        skillButtons = skillPopup.GetComponentsInChildren<Button>(true);
        skillButtonLabels = new TextMeshProUGUI[skillButtons.Length];

        for (int i = 0; i < skillButtons.Length; i++)
        {
            var label = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) skillButtonLabels[i] = label;
        }

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

    // Habilidades
    public void SetupSkillButtons(FighterStats fighter)
    {
        Skill[] skills = fighter.GetSkills();
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < skills.Length)
            {
                skillButtons[i].gameObject.SetActive(true);
                skillButtonLabels[i].text = skills[i].skillName;

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

    // Items
    public void SetupItemList(FighterStats fighter)
    {
        if (itemListContainer == null) return;

        // Limpiar hijos previos
        foreach (Transform t in itemListContainer.transform)
        {
            Destroy(t.gameObject);
        }

        Item[] items = fighter.GetItems();

        foreach (var item in items)
        {
            GameObject go = new GameObject(item.itemName, typeof(RectTransform));
            go.transform.SetParent(itemListContainer.transform);
            go.AddComponent<HorizontalLayoutGroup>();

            // Nombre
            TextMeshProUGUI nameText = new GameObject("Name", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            nameText.text = item.itemName;
            nameText.transform.SetParent(go.transform);

            // Cantidad
            TextMeshProUGUI amountText = new GameObject("Amount", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
            amountText.text = item.amount.ToString();
            amountText.transform.SetParent(go.transform);

            // Botón para usar item
            Button button = go.AddComponent<Button>();
            button.onClick.AddListener(() => OnItemSelected?.Invoke(item));
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

        currentEnemy.OnHealthChanged -= UpdateEnemyHealth;
        currentEnemy.OnHealthChanged += UpdateEnemyHealth;

        // Popups
        skillPopup.SetActive(false);
        itemPopup.SetActive(false);

        // Skills y items
        SetupSkillButtons(currentPlayer);
        SetupItemList(currentPlayer);
    }

    // asigna nuevos componentes desde enemigo de una room hacia el enemigo "interfaz" de battlehud
    // me di cuenta que es inutil tener esto pero lo dejo escrito porsiacaso
    private void AssignEnemyNewComponents(FighterStats enemyStats)
    {
        // obtener viejos componentes
        FighterAction oldEnemyAction = enemySprite.GetComponent<FighterAction>();
        FighterStats oldEnemyStats = enemySprite.GetComponent<FighterStats>();

        // destruir originales si es que existen
        if (oldEnemyAction != null) Destroy(oldEnemyAction);
        if (oldEnemyStats != null) Destroy(oldEnemyStats);

        // asignar nuevos componentes!!
        FighterStats newEnemyStats = enemySprite.AddComponent<FighterStats>();
        newEnemyStats.CopyFrom(enemyStats);
    }
}

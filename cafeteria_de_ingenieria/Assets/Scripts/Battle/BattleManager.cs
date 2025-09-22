using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BattleManager : MonoBehaviour
{
    private GameObject player;
    private GameObject enemy;
    
    // Mapeo simple: qué item del array corresponde a cada botón
    private int[] buttonToItemIndex;

    [Header("Action Menu")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI skillText;
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI talkText;

    [Header("Popups")]
    public GameObject skillPopup;
    public GameObject itemPopup;

    [Header("Skill Buttons")]
    public GameObject[] skillButtons;
    public TextMeshProUGUI[] skillButtonLabels;

    [Header("Item Buttons")]
    public GameObject[] itemButtons;
    public TextMeshProUGUI[] itemButtonLabels;

    [Header("Player & Enemy Panels")]
    public Image playerSprite;
    public TMP_Text playerName;
    public Slider playerHP;
    public Slider playerIQ;

    public Image enemySprite;
    public TMP_Text enemyName;
    public Slider enemyHP;

    [Header("Message Log")]
    public TMP_Text messageLog;

    private int selectedOption = 0;
    private TextMeshProUGUI[] actionOptions;

    // Flag para saber si un popup está activo
    private bool isPopupActive => skillPopup.activeSelf || itemPopup.activeSelf;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Player.ToString());
        actionOptions = new TextMeshProUGUI[] { attackText, skillText, itemText, talkText };
    }
    
    public void SetUpSkillButtons()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].SetActive(false);
        }
    }

    public void SetUpItemButtons()
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].SetActive(false);
        }
    }

    public void ConfigureSkillButtons(int index, string skillName)
    {
        this.skillButtons[index].SetActive(true);
        this.skillButtonLabels[index].text = skillName;
    }

    public void ConfigureItemButtons(int index, string itemName)
    {
        this.itemButtons[index].SetActive(true);
        this.itemButtonLabels[index].text = itemName;
    }
    
    public void ConfigureItemButtons(int buttonIndex, string itemName, int realItemIndex)
    {
        this.itemButtons[buttonIndex].SetActive(true);
        this.itemButtonLabels[buttonIndex].text = itemName;
        
        // Inicializar el mapeo si no existe
        if (buttonToItemIndex == null) buttonToItemIndex = new int[itemButtons.Length];
        buttonToItemIndex[buttonIndex] = realItemIndex;
    }

    void Update()
    {
        // Cerrar popup con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (skillPopup.activeSelf)
                skillPopup.SetActive(false);
            if (itemPopup.activeSelf)
                itemPopup.SetActive(false);
        }

        // Solo navegar si no hay popup abierto
        if (!isPopupActive)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedOption = (selectedOption - 1 + actionOptions.Length) % actionOptions.Length;
                UpdateHighlight();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedOption = (selectedOption + 1) % actionOptions.Length;
                UpdateHighlight();
            }

            // Confirmar con Enter o Space
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ActivateOption(selectedOption);
            }
        }
    }

    // Manejo del mouse
    public void SetSelectedOption(int index)
    {
        if (isPopupActive) return;
        selectedOption = index;
        UpdateHighlight();
    }

    public void OnClickOption(int index)
    {
        if (isPopupActive && (index == 0 || index == 3)) return;
        ActivateOption(index);
    }


    void UpdateHighlight()
    {
        for (int i = 0; i < actionOptions.Length; i++)
        {
            actionOptions[i].color = (i == selectedOption) ? new Color(0.5f, 1f, 1f) : Color.white;
        }
    }


    void ActivateOption(int option)
    {
        switch (option)
        {
            case 0: Attack(); break;
            case 1: ShowSkills(); break;
            case 2: ShowItems(); break;
            case 3: Talk(); break;
        }
    }

    void Attack()
    {
        //Debug.Log("Jugador ataca al enemigo");
        player.GetComponent<FighterAction>().SelectOption(BattleConstants.MenuAttackOptions.Melee.ToString());
        // PlayerController
    }

    void ExecuteSkill(int index)
    {
        if (index < 0 || index >= skillButtons.Length) return;
        Debug.Log("Ejecutar habilidad: " + skillButtonLabels[index].text); 
        enemy = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Enemy.ToString());

        // Obtener las habilidades del jugador
        FighterStats playerStats = player.GetComponent<FighterStats>();
        Skill[] playerSkills = playerStats.GetSkills();

        if (index < playerSkills.Length)
        {
            Skill skillSelected = playerSkills[index];
            skillSelected.SetTargetanduser(playerStats, enemy.GetComponent<FighterStats>());
            skillSelected.Run();
            // Cerrar el popup después de usar la habilidad
            skillPopup.SetActive(false);
        }
    }


    void ShowSkills()
    {
        Debug.Log("Mostrar popup de habilidades");
        skillPopup.SetActive(!skillPopup.activeSelf);
        if (skillPopup.activeSelf)
            itemPopup.SetActive(false);
    }

    void ExecuteItem(int index)
    {
        if (index < 0 || index >= itemButtons.Length || buttonToItemIndex == null) return;
        Debug.Log("Usar objeto: " + itemButtonLabels[index].text); 

        FighterStats playerStats = player.GetComponent<FighterStats>();
        Item[] playerItems = playerStats.GetItems();

        int realItemIndex = buttonToItemIndex[index];
        if (realItemIndex >= 0 && realItemIndex < playerItems.Length)
        {
            Item itemSelected = playerItems[realItemIndex];
            itemSelected.Run();
            itemPopup.SetActive(false);
        }
    }

    void ShowItems()
    {
        Debug.Log("Mostrar popup de objetos");
        itemPopup.SetActive(!itemPopup.activeSelf);
        if (itemPopup.activeSelf)
            skillPopup.SetActive(false);
    }

    void Talk()
    {
        Debug.Log("Inicia conversación");
        // Dialogue System
    }

}

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("Action Menu")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI skillText;
    public TextMeshProUGUI itemText;
    public TextMeshProUGUI talkText;

    [Header("Popups")]
    public GameObject skillPopup;
    public GameObject itemPopup;

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
        actionOptions = new TextMeshProUGUI[] { attackText, skillText, itemText, talkText };
        UpdateHighlight();
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

    void UpdateHighlight()
    {
        for (int i = 0; i < actionOptions.Length; i++)
        {
            actionOptions[i].color = (i == selectedOption) ? new Color(0.5f, 1f, 1f) : Color.white;
        }
    }

    void ActivateOption(int option)
    {
        switch(option)
        {
            case 0: Attack(); break;
            case 1: ShowSkills(); break;
            case 2: ShowItems(); break;
            case 3: Talk(); break;
        }
    }

    void Attack()
    {
        Debug.Log("Jugador ataca al enemigo");
        // PlayerController
    }

    void ShowSkills()
    {
        Debug.Log("Mostrar popup de habilidades");
        skillPopup.SetActive(true);
        // SkillSystem y SkillData
    }

    void ShowItems()
    {
        Debug.Log("Mostrar popup de objetos");
        itemPopup.SetActive(true);
        // ItemData y ItemSystem
    }

    void Talk()
    {
        Debug.Log("Inicia conversación");
        // Dialogue System
    }
}

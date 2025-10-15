using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    [Header("Referencias externas")]
    public BattleUI ui;
    public FighterStats enemy;
    public TurnController turnController;
    public FighterStats player;
    public FighterAction playerAction;


    private int selectedOption = 0;
    private TextMeshProUGUI[] actionOptions;

    // Flag para saber si un popup está activo
    private bool isPopupActive => ui.skillPopup.activeSelf || ui.itemPopup.activeSelf;

    // Al terminar la batalla
    private System.Action onBattleEnd;
    private bool battleActive = false;

    void Start()
    {
        actionOptions = new TextMeshProUGUI[] { ui.attackText, ui.skillText, ui.itemText };
        SetupUI();
    }

    private void SetupUI()
    {
        ui.OnSkillSelected += ExecuteSkill;
        ui.OnItemSelected += ExecuteItem;

        ui.SetupSkillButtons(player);
        ui.SetupItemList(player);

        // Desactivamos popups al inicio
        ui.skillPopup.SetActive(false);
        ui.itemPopup.SetActive(false);
    }

    public void SetEnemy(FighterStats newEnemy)
    {
        if (enemy != null)
            ui.ClearEnemy();

        enemy = newEnemy;
        ui.SetupEnemy(enemy);
    }

    public void AttackEnemy(float damage)
    {
        if (enemy != null)
        {
            enemy.ReceiveDamage(damage);
        }
    }

    public void StartBattle(System.Action onBattleEnd)
    {
        if (enemy == null)
        {
            Debug.LogError("No hay enemigo asignado para la batalla");
            onBattleEnd?.Invoke();
            return;
        }

        battleActive = true;
        ui.gameObject.SetActive(true);
        ResetBattle();

        this.onBattleEnd = onBattleEnd;

        turnController.SetupTurnOrder(player, enemy);
    }

    public void ResetBattle()
    {
        // Inicializar opciones de accion
        if (actionOptions == null || actionOptions.Length == 0)
        {
            actionOptions = new TextMeshProUGUI[] { ui.attackText, ui.skillText, ui.itemText };
        }

        // Reiniciar enemigo
        enemy.health = enemy.startHealth;
        enemy.IQ = enemy.startIQ;
        enemy.gameObject.SetActive(true);

        enemy.OnDeath -= OnEnemyDeath;
        enemy.OnDeath += OnEnemyDeath;

        // Reiniciar jugador
        player.health = player.startHealth;
        player.IQ = player.startIQ;

        // Reiniciar UI
        ui.ResetUI(player, enemy);

        // Limpiar y setear popup
        ui.skillPopup.SetActive(false);
        ui.itemPopup.SetActive(false);

        selectedOption = 0;
        UpdateHighlight();
    }

    void Update()
    {
        // Cerrar popup con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ui.skillPopup.activeSelf)
                ui.skillPopup.SetActive(false);
            if (ui.itemPopup.activeSelf)
                ui.itemPopup.SetActive(false);
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
        if (isPopupActive) return;
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
        }
    }

    void Attack()
    {
        //Debug.Log("Jugador ataca al enemigo");
        playerAction.SelectOption(BattleConstants.MenuAttackOptions.Melee.ToString());
        turnController.EndTurn();
    }

    public void ExecuteSkill(int index)
    {
        Debug.Log($"*** ExecuteSkill called with index: {index} ***");
        if (index < 0 || index >= ui.skillButtons.Length) return;
        Debug.Log("Ejecutar habilidad: " + ui.skillButtonLabels[index].text);

        // Obtener las habilidades del jugador
        FighterStats playerStats = player.GetComponent<FighterStats>();
        Skill[] playerSkills = playerStats.GetSkills();

        if (index < playerSkills.Length)
        {
            Skill skillSelected = playerSkills[index];
            skillSelected.SetTargetanduser(playerStats, enemy.GetComponent<FighterStats>());
            skillSelected.Run();
            // Cerrar el popup después de usar la habilidad
            ui.skillPopup.SetActive(false);
        }
        turnController.EndTurn();
    }

    public void ExecuteItem(Item item)
    {
        item.Run();
        ui.itemPopup.SetActive(false);
        turnController.EndTurn();
    }

    void ShowSkills()
    {
        Debug.Log("Mostrar popup de habilidades");
        ui.skillPopup.SetActive(!ui.skillPopup.activeSelf);
        if (ui.skillPopup.activeSelf)
            ui.itemPopup.SetActive(false);
    }

    void ShowItems()
    {
        Debug.Log("Mostrar popup de objetos");
        ui.itemPopup.SetActive(!ui.itemPopup.activeSelf);
        if (ui.itemPopup.activeSelf)
            ui.skillPopup.SetActive(false);
    }

    private void OnEnemyDeath(FighterStats deadEnemy)
    {
        Debug.Log("Enemigo muerto: " + deadEnemy.name);

        // Elimina al enemigo
        deadEnemy.gameObject.SetActive(false);

        // Termina la batalla
        EndBattle();
    }

    public void EndBattle()
    {
        if (!battleActive) return;
        battleActive = false;

        turnController.BattleEnded();

        // Desactivar UI de batalla
        ui.gameObject.SetActive(false);

        // Avisar a RoomController
        onBattleEnd?.Invoke();
    }
}

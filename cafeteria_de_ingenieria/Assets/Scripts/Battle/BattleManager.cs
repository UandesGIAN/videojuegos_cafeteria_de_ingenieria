using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("Referencias externas")]
    public BattleUI ui;
    public FighterStats enemy;
    public TurnController turnController;
    public FighterStats player;
    public FighterAction playerAction;
    public ItemManager itemManager;

    [Header("Sistema de Preguntas")]
    public QuestionUI questionUI;
    public bool useQuestionSystem = true;

    private int selectedOption = 0;
    private TextMeshProUGUI[] actionOptions;

    // Flag para saber si un popup está activo
    private bool IsPopupActive => ui.skillPopup.activeSelf || ui.itemPopup.activeSelf;

    public Action OnPlayerActionCompleted; // para decirle a TurnController que llame al siguiente turno
    private bool isHandlingSkill = false;

    // Al terminar la batalla
    private Action onBattleEnd;
    private bool battleActive = false;

    // Multiplicador de fuerza por preguntas
    private float currentStrengthMultiplier = 1f;

    void Start()
    {
        Debug.Log("BattleManager GameObject Name: " + gameObject.name);

        playerAction.SetEnemy(enemy.gameObject);

        player.PrintStats();
        enemy.PrintStats();

        turnController.SetBattleManager(this);

        actionOptions = new TextMeshProUGUI[] { ui.attackText, ui.skillText, ui.itemText };
        SetupUI();

        // Configurar evento de preguntas
        if (questionUI != null)
        {
            questionUI.OnQuestionAnswered += OnQuestionAnswered;
        }
    }

    private void SetupUI()
    {
        // quitar suscripciones previas porsiacaso
        ui.OnSkillSelected -= ExecuteSkill;
        ui.OnItemSelected -= ExecuteItem;

        ui.OnSkillSelected += ExecuteSkill;
        ui.OnItemSelected += ExecuteItem;

        ui.SetupSkillButtons(player);
        ui.SetupItemList(player);

        ui.skillPopup.SetActive(false);
        ui.itemPopup.SetActive(false);
    }

    public void SetEnemy(FighterStats newEnemy)
    {
        //Debug.Log("Setting new enemy in BattleManager: " + newEnemy.fightername);
        //Debug.Log("Previous enemy: " + (enemy != null ? enemy.fightername : "null"));
        if (enemy != null) ui.ClearEnemy();

        enemy = newEnemy;
        ui.SetupEnemy(enemy);
    }

    public void AttackEnemy(float damage)
    {
        if (enemy != null)
        {
            // Aplicar multiplicador de fuerza
            float modifiedDamage = damage * currentStrengthMultiplier;
            Debug.Log($"Daño: {damage} x {currentStrengthMultiplier} = {modifiedDamage}");
            enemy.ReceiveDamage(modifiedDamage);
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

        this.onBattleEnd = onBattleEnd;

        // Si el sistema de preguntas está activado, mostrar pregunta primero
        if (useQuestionSystem && questionUI != null)
        {
            Debug.Log("Mostrando pregunta antes de la batalla...");
            questionUI.ShowQuestion();
            // La batalla se iniciará después de responder en OnQuestionAnswered()
        }
        else
        {
            // Iniciar batalla directamente sin pregunta
            currentStrengthMultiplier = 1f;
            StartBattleDirectly();
        }
    }

    private void OnQuestionAnswered(bool correct)
    {
        Debug.Log($"Pregunta respondida: {(correct ? "CORRECTA" : "INCORRECTA")}");
        
        // Aplicar multiplicador según resultado
        if (correct)
        {
            currentStrengthMultiplier = 1.5f; // +50% de fuerza
            Debug.Log("¡Bonus de fuerza aplicado! x1.5");
        }
        else
        {
            currentStrengthMultiplier = 0.5f; // -50% de fuerza
            Debug.Log("Penalización de fuerza aplicada x0.5");
        }

        // Ahora sí iniciar la batalla
        StartBattleDirectly();
    }

    private void StartBattleDirectly()
    {
        battleActive = true;
        turnController.SetBattleActive(battleActive);

        FighterStats newEnemy = playerAction.GetEnemy().GetComponent<FighterStats>();
        SetEnemy(newEnemy);

        Debug.Log("Enemy in BattleManager after StartBattle: ");
        enemy.PrintStats();

        ui.gameObject.SetActive(true);
        ResetBattle();

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
        enemy.UpdateHealthBar();
        enemy.IQ = enemy.startIQ;
        enemy.UpdateIQBar();

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

        // Solo navegar si no hay popup abierto y si es el turno del jugador
        if (!IsPopupActive && turnController.GetCanPlayerAct())
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
        if (IsPopupActive) return;
        selectedOption = index;
        UpdateHighlight();
    }

    public void OnClickOption(int index)
    {
        if (IsPopupActive) return;
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
        OnPlayerActionCompleted?.Invoke();
    }

    public void ExecuteSkill(int index)
    {
        if (isHandlingSkill)
        {
            Debug.LogWarning("ExecuteSkill ignored: already handling a skill");
            return;
        }

        isHandlingSkill = true;
        try
        {
            Debug.Log($"*** ExecuteSkill called with index: {index} ***");
            if (index < 0 || index >= ui.skillButtons.Length) return;
            Debug.Log("Ejecutar habilidad: " + ui.skillButtonLabels[index].text);

            Skill[] playerSkills = player.GetSkills();

            if (index < playerSkills.Length)
            {
                Skill skillSelected = playerSkills[index];
                // enemy por alguna razon no esta synceado con el UI enemy, pero el enemy de playerAction si
                skillSelected.SetTargetanduser(player, playerAction.GetEnemy().GetComponent<FighterStats>());
                skillSelected.Run();
                ShowSkills();
            }

            StartCoroutine(NotifyTurnControllerAfterSkillOrObjectAction());
        }
        finally
        {
            isHandlingSkill = false;
            ui.skillPopup.SetActive(false);
        }
    }

    public void ExecuteItem(Item item)
    {
        if (item == null) return;

        // Ejecuta el efecto del item
        item.Run();

        // Elimina la instancia del jugador
        itemManager.RemoveItemFromPlayerInstance(item);

        // Actualiza el HUD
        ui.itemPopup.SetActive(false);

        StartCoroutine(NotifyTurnControllerAfterSkillOrObjectAction());
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
        ui.SetupItemList(player);
        if (ui.itemPopup.activeSelf)
            ui.skillPopup.SetActive(false);
    }

    private void OnEnemyDeath(FighterStats deadEnemy)
    {
        Debug.Log("Enemigo muerto: " + deadEnemy.fightername);

        // Elimina al enemigo
        deadEnemy.gameObject.SetActive(false);
        Destroy(deadEnemy.gameObject);

        // Termina la batalla
        EndBattle();
    }

    public void EndBattle()
    {
        if (!battleActive) return;
        battleActive = false;

        // Resetear multiplicador
        currentStrengthMultiplier = 1f;

        turnController.BattleEnded();

        // Desactivar UI de batalla
        ui.gameObject.SetActive(false);

        // Avisar a RoomController
        onBattleEnd?.Invoke();
    }

    void OnDestroy()
    {
        if (ui != null)
        {
            ui.OnSkillSelected -= ExecuteSkill;
            ui.OnItemSelected -= ExecuteItem;
        }

        if (questionUI != null)
        {
            questionUI.OnQuestionAnswered -= OnQuestionAnswered;
        }
    }

    private IEnumerator NotifyTurnControllerAfterSkillOrObjectAction()
    {
        turnController.SetBattleMenuState(false);
        turnController.SetCanPlayerAct(false);

        // es necesario agregar este delay para que no se ejecute dos veces el turno del enemigo
        yield return new WaitForSeconds(0.5f);
        OnPlayerActionCompleted?.Invoke();
    }
}

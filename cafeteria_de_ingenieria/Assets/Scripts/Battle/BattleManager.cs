using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("Referencias externas")]
    public BattleUI ui;
    public FighterStats enemy;
    public TurnController turnController;
    public FighterStats player;
    public FighterAction playerAction;
    public ItemManager itemManager;

    [Header("Referencias de Diálogo")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Sistema de Preguntas")]
    public QuestionUI questionUI;
    public bool useQuestionSystem = true;

    // Flag para saber si un popup está activo
    private bool IsPopupActive => ui.skillPopup.activeSelf || ui.itemPopup.activeSelf;

    public Action OnPlayerActionCompleted; // para decirle a TurnController que llame al siguiente turno
    private bool isHandlingSkill = false;

    // Al terminar la batalla
    private Action onBattleEnd;
    private bool battleActive = false;
    int selectedOption = 0;

    public void Start()
    {
        Debug.Log("BattleManager GameObject Name: " + gameObject.name);

        playerAction.SetEnemy(enemy.gameObject);

        player.PrintStats();
        enemy.PrintStats();

        turnController.SetBattleManager(this);
        SetupUI();

        // Configurar evento de preguntas
        if (questionUI != null)
        {
            questionUI.OnQuestionAnswered -= OnQuestionAnswered;
            questionUI.OnQuestionAnswered += OnQuestionAnswered;
        }
        
        // Configurar dialogo
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            Debug.Log("DIALOGUE PANEL ACTIVATED IN START() OF BATTLEMANAGER");
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
        
        ui.OnOptionClicked += OnClickOption;
        ui.OnOptionHovered += OnHoverOption;
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
            // Los multiplicadores ahora se aplican directamente en las estadísticas
            enemy.ReceiveDamage(damage);
        }
    }

    public void StartBattle(Action onBattleEnd)
    {
        if (enemy == null)
        {
            Debug.LogError("No hay enemigo asignado para la batalla");
            onBattleEnd?.Invoke();
            return;
        }

        this.onBattleEnd = onBattleEnd;

        // Resetear multiplicadores del jugador antes de comenzar
        player.ResetCombatMultipliers();

        // Si el sistema de preguntas está activado, actualizar UI, luego mostrar pregunta
        ui.gameObject.SetActive(true);
        SetupUI();
        ui.ResetUI(player, enemy);

        // Reiniciar estados del jugador y enemigo
        player.health = player.startHealth;
        player.IQ = player.startIQ;
        enemy.health = enemy.startHealth;
        enemy.IQ = enemy.startIQ;
        bool start = true;
        enemy.UpdateHealthBar(start);
        enemy.UpdateIQBar();

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
                
        // Ahora la pregunta
        if (useQuestionSystem && questionUI != null)
        {
            Debug.Log("Cargando UI y mostrando pregunta antes de la batalla...");
            StartCoroutine(ShowQuestionAfterUIReady());
        }
        else
        {
            StartBattleDirectly();
        }
    }

    private IEnumerator ShowQuestionAfterUIReady()
    {
        // Espera un frame para que Unity actualice visualmente la UI
        yield return null;

        // Ahora muestra la pregunta sobre la UI ya cargada
        questionUI.ShowQuestion();
    }

    private void OnQuestionAnswered(bool correct)
    {
        Debug.Log($"Pregunta respondida: {(correct ? "CORRECTA" : "INCORRECTA")}");

        // Aplicar multiplicadores a las estadísticas del jugador
        player.ApplyQuestionMultiplier(correct);

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
                
        // Ahora sí iniciar la batalla
        StartBattleDirectly();
    }

    private void StartBattleDirectly()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
                
        //yield return StartCoroutine(ShowDialogue(enemy.dialogueOnBattleStart));

        Debug.Log("VOLVIENDO A START BATTLE DIRECTLY");

        // activando la batalla
        battleActive = true;
        turnController.SetBattleActive(battleActive);

        // se obtiene enemigo
        FighterStats newEnemy = playerAction.GetEnemy().GetComponent<FighterStats>();
        SetEnemy(newEnemy);

        Debug.Log("Enemy in BattleManager after StartBattle: ");
        enemy.PrintStats();

        // activando el menu
        ui.gameObject.SetActive(true);
        ResetBattle();

        // mostrando dialogo inicial
        ShowDialogue(enemy.dialogueOnBattleStart);

        // se disponen los turnos y se espera que el jugador ataque
        turnController.SetupTurnOrder(player, enemy);
    }

    public void ResetBattle()
    {
        // Resetear multiplicadores del jugador
        player.ResetCombatMultipliers();

        // Reiniciar enemigo
        enemy.health = enemy.startHealth;
        enemy.hasSaidMidHealthDialogue = false;
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
        if (player != null && player.IsDead())
        {
            Debug.Log("GAME OVER. Reiniciando juego...");
            
            // Reproducir música de Game Over
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayGameOverMusic();
            }
            
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f); // pequeña pausa antes del reinicio
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // recarga la escena actual
    }

    // Manejo del mouse
    void OnClickOption(int index)
    {
        if (IsPopupActive) return;
        ActivateOption(index);
    }

    void OnHoverOption(int index)
    {
        if (IsPopupActive) return;
        selectedOption = index;
        ui.SetSelectedOption(index);
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
        player.HasAttacked = true;
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
            player.HasAttacked = true;
            NotifyTurnControllerAfterSkillOrItemAction();
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

        // Establecer usuario del item
        item.SetUser(player);
        
        // Ejecuta el efecto del item
        item.Run();

        // Elimina la instancia del jugador
        itemManager.RemoveItemFromPlayerInstance(item);

        // Actualiza el HUD
        ui.itemPopup.SetActive(false);
        player.HasAttacked = true;
        NotifyTurnControllerAfterSkillOrItemAction();
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
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true); 
        StartCoroutine(HandleEnemyDeath(deadEnemy));
    }

    private IEnumerator HandleEnemyDeath(FighterStats deadEnemy)
    {
        Debug.Log("Enemigo muerto: " + deadEnemy.fightername);
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
                
        //yield return StartCoroutine(ShowDialogue(deadEnemy.dialogueOnDefeat));

        deadEnemy.gameObject.SetActive(false);
        Destroy(deadEnemy.gameObject);

        EndBattle();

        yield return null;
    }

    public void EndBattle()
    {
        if (!battleActive) return;
        battleActive = false;
        
        // Resetear multiplicadores de estadísticas del jugador
        player.ResetCombatMultipliers();

        turnController.BattleEnded();
        
        // Desuscribirse
        if (questionUI != null)
        {
            questionUI.OnQuestionAnswered -= OnQuestionAnswered;
        }

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

    private void NotifyTurnControllerAfterSkillOrItemAction()
    {
        turnController.SetBattleMenuState(false);
        turnController.SetCanPlayerAct(false);
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
                
        StartCoroutine(NotifyAsync());
    }

    private IEnumerator NotifyAsync()
    {
        gameObject.SetActive(true);
        yield return null; // ESPERA UN FRAME PARA QUE EL OBJETO SE ACTIVE

        yield return new WaitForSeconds(0.5f);
        OnPlayerActionCompleted?.Invoke();
    }
    
    public void ShowDialogue(string sentence)
    {
        if (!dialoguePanel) 
        {
            Debug.LogWarning("Dialogue Panel no está asignado en BattleManager.");
            return;
        }

        if (string.IsNullOrEmpty(sentence))
            sentence = "FALTA PONER DIÁLOGO ACÁ";

        // mostrar dialogo!!!
        dialoguePanel.SetActive(true);
        dialogueText.text = sentence;
        Debug.Log("MOSTRANDO DIALOGO: " + sentence);

        //while (!player.HasAttacked)
        //{
        //    Debug.Log("Esperando a que el jugador ataque para continuar el diálogo...");
        //    yield return null; // Espera un frame
        //}

        // Ocultar el diálogo una vez que atacó
        //dialogueText.text = "";
    }
}

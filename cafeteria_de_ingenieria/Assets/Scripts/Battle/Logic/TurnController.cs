using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnController : MonoBehaviour
{
    [SerializeField] private GameObject battleMenu;       // ActionMenu en el editor
    [SerializeField] private bool doesEnemyMoveFirst = false;
    [SerializeField] private float EnemyWaitTime = 0.5f;

    private List<FighterStats> fightersTurnOrder;
    private bool battleActive = false;
    private bool canPlayerAct = true;

    private BattleManager battleManager; // se actualiza cuando se cambia de room
    private Coroutine enemyCoroutine; // para cancelar corutinas de ataque del enemigo en casos donde ataca dos veces

    public void SetupTurnOrder(FighterStats player, FighterStats enemy, bool enemyFirst = false)
    {
        // por default, player va primero y luego el enemigo
        fightersTurnOrder = new List<FighterStats>
        {
            player,
            enemy
        };

        // si el enemigo se mueve primero, invertir la lista
        if (enemyFirst || doesEnemyMoveFirst) fightersTurnOrder.Reverse();

        PrintTurnOrder();

        NextTurn();
    }

    public void NextTurn()
    {
        Debug.Log($">>> NextTurn() called by: {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType}.{new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}");
        if (!battleActive || fightersTurnOrder.Count == 0) return;

        PrintTurnOrder();

        // obtener el fighter actual y removerlo de la lista temporalmente
        FighterStats currentFighterStats = fightersTurnOrder[0];
        fightersTurnOrder.Remove(currentFighterStats);

        if (currentFighterStats.IsDead())
        {
            Debug.Log(currentFighterStats.gameObject.tag + " is Dead!!!!!!");
            NextTurn();
            return;
        }

        // si el character actual no esta muerto, se agrega al final de la lista
        GameObject currentFighterObject = currentFighterStats.gameObject;
        fightersTurnOrder.Add(currentFighterStats);

        // determinar a quien le toca atacar
        if (currentFighterObject.CompareTag(BattleConstants.CharacterRole.Player.ToString()))
        {
            canPlayerAct = true;
            battleMenu.SetActive(true); //ActionMenu del jugador se activa y se espera que aprete algo
        }
        else // es el turno del enemigo!
        {
            canPlayerAct = false;
            // ocultar ActionMenu para evitar q player ataque cuando no es su turno
            battleMenu.SetActive(false);

            // asegurarse de no tener varias corutinas activas
            if (enemyCoroutine != null)
            {
                StopCoroutine(enemyCoroutine);
                enemyCoroutine = null;
            }

            enemyCoroutine = StartCoroutine(EnemyWaitsAndActs());

            // corutina de ataque del enemigo!
            IEnumerator EnemyWaitsAndActs()
            {
                // enemigo waitea y ataca para que no pase de inmediato todo!!
                yield return new WaitForSeconds(EnemyWaitTime);
                if (!battleActive || currentFighterStats == null) yield break;

                if (currentFighterObject.TryGetComponent<FighterAction>(out var currentFighterAction))
                {
                    // hay q hacer q FighterAction.SelectOption() soporte habilidades!
                    string attackType = Random.Range(0, 2) == 0 ?
                        BattleConstants.MenuAttackOptions.Melee.ToString() :
                        BattleConstants.MenuAttackOptions.Melee.ToString(); // cambiar por Ability cuando se cumpla el comentario de arriba

                    currentFighterAction.SelectOption(attackType);
                }

                yield return new WaitForSeconds(EnemyWaitTime);
                if (battleActive) NextTurn();
            }
        }
    }

    private void PrintTurnOrder()
    {
        Debug.Log("List of turn order:");
        foreach (FighterStats fighter in fightersTurnOrder)
        {
            Debug.Log("\t\t" + fighter + " (" + fighter.fightername + ", Tag: " + fighter.gameObject.tag + ")");
        }
    }

    public void BattleEnded()
    {
        battleActive = false;

        // limpiar corutina del enemigo
        if (enemyCoroutine != null)
        {
            StopCoroutine(enemyCoroutine);
            enemyCoroutine = null;
        }

        fightersTurnOrder.Clear();
        battleMenu.SetActive(false);
    }

    public bool GetCanPlayerAct()
    {
        return this.canPlayerAct;
    }
    
    public void SetCanPlayerAct(bool canPlayerAct)
    {
        this.canPlayerAct = canPlayerAct;
    }

    public void SetBattleMenuState(bool isBattleMenuActive)
    {
        battleMenu.SetActive(isBattleMenuActive);
    }

    public void SetBattleActive(bool isBattleActive)
    {
        this.battleActive = isBattleActive;
    }

    public void SetBattleManager(BattleManager battleManager)
    {
        this.battleManager = battleManager;
        battleManager.OnPlayerActionCompleted = NextTurn;
        Debug.Log("TurnController.battleManager set to " + this.battleManager.name);
    }
}

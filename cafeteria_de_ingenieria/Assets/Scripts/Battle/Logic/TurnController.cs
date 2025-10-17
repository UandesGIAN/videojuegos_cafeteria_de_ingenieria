using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    [SerializeField] private GameObject battleMenu; // ActionMenu en el editor
    [SerializeField] private bool doesEnemyMoveFirst = false;

    private List<FighterStats> fightersTurnOrder;
    private bool battleActive = false;

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

        //PrintTurnOrder();

        NextTurn();
    }

    public void NextTurn()
    {
        Debug.Log(">>> NextTurn() called");
        if (!battleActive || fightersTurnOrder.Count == 0) return;

        //PrintTurnOrder();

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
            battleMenu.SetActive(true); //ActionMenu del jugador se activa y se espera que aprete algo
        }
        else // es el turno del enemigo!
        {
            Debug.Log("Turno del enemigo.. ocultando menu de batalla");
            // ocultar ActionMenu para evitar q player ataque cuando no es su turno
            battleMenu.SetActive(false);

            // hay q hacer q FighterAction.SelectOption() soporte habilidades!
            string attackType = Random.Range(0, 2) == 0 ?
                BattleConstants.MenuAttackOptions.Melee.ToString() :
                BattleConstants.MenuAttackOptions.Melee.ToString(); // cambiar por Ability cuando se cumpla el comentario de arriba

            // enemigo waitea y ataca para que no pase de inmediato todo!!
            FighterAction currentFighterAction = currentFighterObject.GetComponent<FighterAction>();
            StartCoroutine(EnemyWaitsAndActs());

            System.Collections.IEnumerator EnemyWaitsAndActs()
            {
                yield return new WaitForSeconds(1f);
                currentFighterAction.SelectOption(attackType);
                yield return new WaitForSeconds(0.5f); 
                NextTurn();
            }
        }
    }

    private void PrintTurnOrder()
    {
        Debug.Log("List of turn order:");
        foreach (FighterStats fighter in fightersTurnOrder)
        {
            Debug.Log("\t\t" + fighter);
        }
    }

    public void BattleEnded()
    {
        battleActive = false;
        fightersTurnOrder.Clear();
        battleMenu.SetActive(false);
    }

    public void SetBattleActive(bool isBattleActive)
    {
        this.battleActive = isBattleActive;
    }
}

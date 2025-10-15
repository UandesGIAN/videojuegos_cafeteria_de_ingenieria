using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public FighterStats currentPlayer;
    public FighterAction enemyAction;

    private List<FighterStats> fightersTurnOrder;

    [SerializeField]
    private GameObject battleMenu; // ActionMenu en el editor

    [SerializeField]
    private bool doesEnemyMoveFirst = false;
    private int currentIndex = 0;

    private bool battleActive = false;

    private void Awake() {
    }

    public void SetupTurnOrder(FighterStats player, FighterStats enemy, bool enemyFirst = false)
    {
        fightersTurnOrder = new List<FighterStats>();
        fightersTurnOrder.Add(player);
        fightersTurnOrder.Add(enemy);

        if (enemyFirst)
            fightersTurnOrder.Reverse();

        currentIndex = 0;
        battleActive = true;
        
        NextTurn();
    }

    public void NextTurn()
    {
        if (!battleActive || fightersTurnOrder.Count == 0) return;
        FighterStats currentFighter = fightersTurnOrder[currentIndex];

        //PrintTurnOrder();

        if (currentFighter.IsDead())
        {
            Debug.Log(currentFighter.gameObject.tag + " is Dead!!!!!!");
            currentIndex = (currentIndex + 1) % fightersTurnOrder.Count;
            NextTurn();
            return;
        }

        // si el character actual no esta muerto:
        GameObject currentFighterObject = currentFighter.gameObject;
        fightersTurnOrder.Add(currentFighter);

        // determinar a quien le toca atacar
        if (currentFighterObject.CompareTag(BattleConstants.CharacterRole.Player.ToString()))
        {
            battleMenu.SetActive(true); //ActionMenu
        }
        else
        {
            // ocultar ActionMenu para evitar q player ataque cuando no es su turno
            battleMenu.SetActive(false);

            // reemplazar limite superior por 2 cuando esten hechas las skills
            // igual obviamente habria que hacer la pega de que FighterAction.SelectOption() soporte habilidades
            string attackType = Random.Range(0, 2) == 0 ?
                BattleConstants.MenuAttackOptions.Melee.ToString() :
                BattleConstants.MenuAttackOptions.Ability.ToString();

            enemyAction.SelectOption(attackType);
        }
    }

    public void EndTurn()
    {
        if (!battleActive) return;
        currentIndex = (currentIndex + 1) % fightersTurnOrder.Count;
        NextTurn();
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
        currentIndex = 0;
        fightersTurnOrder.Clear();
        battleMenu.SetActive(false);
    }
}

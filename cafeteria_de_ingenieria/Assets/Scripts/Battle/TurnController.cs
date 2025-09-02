using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    private List<FighterStats> fightersTurnOrder;

    [SerializeField]
    private GameObject battleMenu; //ActionMenu en el editor

    [SerializeField]
    private bool doesEnemyMoveFirst = false;

    private void Start()
    {
        // por default, player va primero y luego el enemigo
        fightersTurnOrder = new List<FighterStats>();
        GameObject player = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Player.ToString());
        FighterStats currentPlayerStats = player.GetComponent<FighterStats>();
        fightersTurnOrder.Add(currentPlayerStats);

        GameObject enemy = GameObject.FindGameObjectWithTag(BattleConstants.CharacterRole.Enemy.ToString());
        FighterStats currentEnemyStats = enemy.GetComponent<FighterStats>();
        fightersTurnOrder.Add(currentEnemyStats);

        // si el enemigo se mueve primero, invertir la lista
        if (doesEnemyMoveFirst) fightersTurnOrder.Reverse();

        NextTurn();
    }

    public void NextTurn()
    {
        FighterStats currentFighterStats = fightersTurnOrder[0];
        fightersTurnOrder.Remove(currentFighterStats);

        //PrintTurnOrder();

        if (currentFighterStats.IsDead())
        {
            Debug.Log(currentFighterStats.gameObject.tag + " is Dead!!!!!!");
            NextTurn();
            return;
        }

        // si el character actual no esta muerto:
        GameObject currentFighterObject = currentFighterStats.gameObject;
        fightersTurnOrder.Add(currentFighterStats);

        // determinar a quien le toca atacar
        if (currentFighterObject.CompareTag(BattleConstants.CharacterRole.Player.ToString()))
        {
            battleMenu.SetActive(true); //ActionMenu se activa y se espera a que se aprete algo
        }
        else
        {
            // ocultar ActionMenu para evitar q player ataque cuando no es su turno
            battleMenu.SetActive(false);

            // reemplazar limite superior por 2 cuando esten hechas las skills
            // igual obviamente habria que hacer la pega de que FighterAction.SelectOption() soporte abilidades
            string attackType = Random.Range(0, 1) == 0 ? BattleConstants.MenuAttackOptions.Melee.ToString() : BattleConstants.MenuAttackOptions.Ability.ToString();
            currentFighterObject.GetComponent<FighterAction>().SelectOption(attackType);
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
}

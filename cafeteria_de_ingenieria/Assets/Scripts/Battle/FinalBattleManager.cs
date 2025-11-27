using UnityEngine;
using System.Collections;

public class FinalBattleManager : BattleManager
{
    [Header("Jefe Final — 2 Fases")]
    public FighterStats phase1Enemy;
    public FighterStats phase2Enemy;

    private int currentPhase = 1;

    public override void OnEnemyDeath(FighterStats deadEnemy)
    {
        if (currentPhase == 1)
        {
            Debug.Log("[FinalBattleManager] Fase 1 completada, bloqueando EndBattle...");
            blockHandleEnemyDeath = true; // Evita que BattleManager llame a EndBattle()
            StartCoroutine(StartPhase2(deadEnemy));
            return;
        }

        // Fase 2 - Ahora cuando el jefe 2 muere, termina
        base.OnEnemyDeath(deadEnemy);
    }

    private IEnumerator StartPhase2(FighterStats deadEnemy)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayFinalBattlePhase2Music();
            Debug.Log("[FinalBattleManager] Reproduciendo música de batalla final - Fase 2");
        }

        currentPhase = 2;
        Debug.Log("FASE 1 derrotada. Iniciando FASE 2 del jefe final...");

        // DIÁLOGO DE VICTORIA DE FASE 1
        DialogueManager.Instance.ShowDialogue(deadEnemy.dialogueOnDefeat);
        yield return new WaitForSeconds(0.8f);

        // DESACTIVAR Y DESTRUIR ENEMIGO DE FASE 1
        Vector3 spawnPos = deadEnemy.transform.position;
        deadEnemy.gameObject.SetActive(false);
        Destroy(deadEnemy.gameObject);

        // SPAWNEAR FASE 2 EN MISMO PUNTO
        FighterStats newBoss = Instantiate(phase2Enemy, spawnPos, Quaternion.identity);
        enemy = newBoss;
        FighterAction playerAction = player.GetComponent<FighterAction>();
        if (playerAction != null)
        {
            playerAction.SetEnemy(enemy.gameObject); // ahora apunta al jefe de fase 2
        }

        // RESETEAR ENEMIGO
        enemy.health = enemy.startHealth;
        enemy.IQ = enemy.startIQ;
        enemy.hasSaidMidHealthDialogue = false;
        enemy.gameObject.SetActive(true);

        enemy.OnDeath -= OnEnemyDeath;
        enemy.OnDeath += OnEnemyDeath;
        enemy.UpdateHealthBar();
        enemy.UpdateIQBar();

        // REINICIAR UI COMPLETA DE BATALLA
        SetupUI(); // Re-suscribe eventos
        ui.ResetUI(player, enemy); // reinicia barras, sprites, popups
        turnController.SetupTurnOrder(player, enemy); // reinicia turnos
        turnController.SetBattleActive(true);

        yield return new WaitForSeconds(0.25f);

        // DIÁLOGO DE INICIO DE FASE 2
        DialogueManager.Instance.ShowDialogue(enemy.dialogueOnBattleStart);
        yield return new WaitForSeconds(0.5f);

        // REINICIAR SISTEMA DE TURNOS
        turnController.SetupTurnOrder(player, enemy);
        turnController.SetBattleActive(true);

        Debug.Log("FASE 2 lista.");
        blockHandleEnemyDeath = false;
    }


    private Vector3 enemySpawnPoint()
    {
        return enemy != null ? enemy.transform.position : transform.position;
    }
}

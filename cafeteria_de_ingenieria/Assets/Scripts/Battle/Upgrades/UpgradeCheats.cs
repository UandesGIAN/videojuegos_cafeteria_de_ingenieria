using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StatCheats : MonoBehaviour
{
    public FighterStats player;

    void Update()
    {
        if (player == null) return;

        // BACKSPACE = Reiniciar juego
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StartCoroutine(RestartGame());
        }

        // SHIFT + X = Cerrar juego
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            Input.GetKeyDown(KeyCode.X))
        {
            QuitGame();
        }

        // Health +20: Ctrl + H
        if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKeyDown(KeyCode.H))
        {
            AddHealth(20);
        }

        // IQ +20 → I + Q
        if (Input.GetKey(KeyCode.I) &&
            Input.GetKeyDown(KeyCode.Q))
        {
            AddIQ(20);
        }

        // Attack +20 → A + T
        if (Input.GetKey(KeyCode.A) &&
            Input.GetKeyDown(KeyCode.T))
        {
            AddAttack(20);
        }

        // IQArmor +20 → I + A
        if (Input.GetKey(KeyCode.I) &&
            Input.GetKeyDown(KeyCode.A))
        {
            AddIQArmor(1);
        }

        // PhysicalArmor +20 → A + R
        if (Input.GetKey(KeyCode.R) &&
            Input.GetKeyDown(KeyCode.A))
        {
            AddPhysicalArmor(1);
        }
    }

    void AddHealth(float amount)
    {
        player.startHealth += amount;
        player.health += amount;
        player.UpdateHealthBar();

        Debug.Log($"CHEAT: +{amount} salud máxima");
        player.PrintStats();
    }

    void AddIQ(float amount)
    {
        player.startIQ += amount;
        player.IQ += amount;
        player.UpdateIQBar();

        Debug.Log($"CHEAT: +{amount} IQ máximo");
        player.PrintStats();
    }

    void AddAttack(float amount)
    {
        player.attack += amount;

        Debug.Log($"CHEAT: +{amount} ataque");
        player.PrintStats();
    }

    void AddIQArmor(float amount)
    {
        player.IQArmor += amount;

        Debug.Log($"CHEAT: +{amount} armadura IQ");
        player.PrintStats();
    }

    void AddPhysicalArmor(float amount)
    {
        player.physicalArmor += amount;

        Debug.Log($"CHEAT: +{amount} armadura física");
        player.PrintStats();
    }

    // VOLVER AL MENU PRINCIPAL
    private IEnumerator RestartGame()
    {
        yield return null;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // SALIR DEL JUEGO
    private void QuitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
        }
}


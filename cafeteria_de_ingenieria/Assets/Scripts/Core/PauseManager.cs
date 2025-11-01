using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI del Menú de Pausa")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Referencias opcionales")]
    [SerializeField] private FighterStats playerStats;
    [SerializeField] private FighterStats enemyStats;

    public static bool IsPaused { get; private set; } = false;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Al presionar P pausa
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f; // Detiene el tiempo

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        // Deshabilita inputs del jugador (opcional, si tienes scripts específicos)
        TogglePlayerInputs(false);

        Debug.Log("Juego pausado.");
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f; // Reanuda el tiempo

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        TogglePlayerInputs(true);

        Debug.Log("Juego reanudado.");
    }

    private void TogglePlayerInputs(bool enabled)
    {
        // Desactiva temporalmente scripts de control, si los tienes
        if (playerStats != null)
        {
            var playerAction = playerStats.GetComponent<FighterAction>();
            if (playerAction != null) playerAction.enabled = enabled;
        }

        // Lo mismo con el HUD o control de batalla si quieres evitar que se activen
        if (enemyStats != null)
        {
            var enemyAI = enemyStats.GetComponent<FighterAction>();
            if (enemyAI != null) enemyAI.enabled = enabled;
        }
    }
}

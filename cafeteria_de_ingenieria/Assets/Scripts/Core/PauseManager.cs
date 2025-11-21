using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PauseManager : MonoBehaviour
{
    [Header("UI del Menú de Pausa")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Referencias del Jugador y Enemigo")]
    [SerializeField] private FighterStats player;
    [SerializeField] private FighterStats enemy;

    [Header("Elementos UI")]
    [SerializeField] private Image playerIcon;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerStatsText;
    [SerializeField] private Button[] skillButtons;
    [SerializeField] private TextMeshProUGUI[] skillButtonLabels;
    [SerializeField] private Transform itemListContainer;
    [SerializeField] private GameObject itemButtonPrefab;

    [Header("UI del Enemigo (solo si hay batalla)")]
    [SerializeField] private Image enemyImage;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyStatsText;

    public static bool IsPaused { get; private set; } = false;

    // Diccionario de personajes
    private Dictionary<string, (string desc, string tipo, string skill)> personajes =
        new Dictionary<string, (string, string, string)>
        {
            {"BORJA", ("Alumno fornido y guapo, de pelo y ojos cafés.","TIPO CEMENTO","HORMIGONAZO. Lanza hormigón al enemigo (15 DAÑO / 30 IQ).")},
            {"VICE", ("Estudiante que controla las plantas","TIPO PLANTA","ESPORAS. Infecta al enemigo con E. Coli (30 DAÑO / 20 IQ).")},
            {"GIAN", ("Alumno alto con lentes y peinado singular","TIPO ELECTRÓNICO","DISTORSIÓN. Modifica el campo magnético del enemigo (20 DAÑO / 10 IQ).")},
            {"PEDE", ("Alumno de apariencia otaku (alérgico al agua).","TIPO FUEGO","OTAKUFLAMA. Quema los ojos del enemigo con un dibujo de anime (20 DAÑO / 10 IQ).")},
            {"LUCAS", ("Estudiante que se deja fluir como el agua.","TIPO AGUA","PUTREFACCIÓN. Contamina al enemigo con agua estancada (15 DAÑO / 15 IQ).")}
        };

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (IsPaused) ResumeGame();
            else PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsPaused)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            UpdatePauseMenu();
        }

        TogglePlayerInputs(false);
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        TogglePlayerInputs(true);
    }

    private void TogglePlayerInputs(bool enabled)
    {
        if (player != null)
        {
            var action = player.GetComponent<FighterAction>();
            if (action != null) action.enabled = enabled;
        }

        if (enemy != null)
        {
            var ai = enemy.GetComponent<FighterAction>();
            if (ai != null) ai.enabled = enabled;
        }
    }

    private void UpdatePauseMenu()
    {
        if (player == null) return;

        // Imagen del jugador
        if (playerIcon != null && player.img != null)
            playerIcon.sprite = player.img;

        // Nombre
        if (playerNameText != null)
            playerNameText.text = player.fightername;

        // Info del personaje desde el diccionario
        if (personajes.TryGetValue(player.fightername.ToUpper(), out var info))
        {
            playerStatsText.text =
                $"{info.desc}\n" +
                $"{info.tipo}\n" +
                $"HABILIDAD BASE: {info.skill}\n\n" +
                $"Vida: {player.health}/{player.startHealth}\n" +
                $"IQ: {player.IQ}/{player.startIQ}\n" +
                $"Ataque: {player.attack}\n" +
                $"Nivel: {player.level}";
        }

        UpdateSkillButtons();
        UpdateItemButtons();
        UpdateEnemyInfo();
    }

    private void UpdateSkillButtons()
    {
        Skill[] skills = player.GetSkills();
        if (skills == null || skillButtons == null || skillButtonLabels == null) return;

        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (i < skills.Length)
            {
                skillButtons[i].gameObject.SetActive(true);

                if (skills[i] is HealthModSkill healthSkill)
                    skillButtonLabels[i].text = $"{skills[i].skillName}: {healthSkill.amount} DAÑO / {healthSkill.cost} IQ";
                else
                    skillButtonLabels[i].text = skills[i].skillName;

                // Sin listeners, solo visual
                skillButtons[i].onClick.RemoveAllListeners();
            }
            else
            {
                skillButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateItemButtons()
    {
        if (itemListContainer == null || itemButtonPrefab == null) return;

        foreach (Transform child in itemListContainer)
            Destroy(child.gameObject);

        Item[] items = player.GetItems();
        if (items == null || items.Length == 0) return;

        foreach (var item in items)
        {
            GameObject itemButton = Instantiate(itemButtonPrefab, itemListContainer);
            
            // Agregar componente de sonido (aunque el botón sea solo visual)
            Button btn = itemButton.GetComponent<Button>();
            if (btn != null && btn.GetComponent<UIButtonSound>() == null)
            {
                UIButtonSound buttonSound = btn.gameObject.AddComponent<UIButtonSound>();
                buttonSound.soundType = UIButtonSound.ButtonSoundType.Normal;
                buttonSound.playClickSound = false; // No hace nada al hacer clic
            }
            
            TextMeshProUGUI[] texts = itemButton.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2)
            {
                texts[0].text = item.itemName;
                texts[1].text = "x" + player.GetItemCount(item.itemName);
            }
        }
    }

    private void UpdateEnemyInfo()
    {
        // Busca el BattleManager activo
        BattleManager activeBattle = FindObjectsOfType<BattleManager>().FirstOrDefault(b => b.isActiveAndEnabled && b.gameObject.activeInHierarchy);

        if (activeBattle == null || activeBattle.enemy == null)
        {
            if (enemyImage != null) enemyImage.enabled = false;
            if (enemyNameText != null) enemyNameText.text = "";
            if (enemyStatsText != null) enemyStatsText.text = "";
            return;
        }

        FighterStats enemy = activeBattle.enemy;
        if (enemyImage != null)
        {
            enemyImage.enabled = true;
            enemyImage.sprite = enemy.img;
        }

        if (enemyNameText != null)
            enemyNameText.text = enemy.fightername;

        if (enemyStatsText != null)
        {
            enemyStatsText.text =
                $"Vida: {enemy.health}/{enemy.startHealth}\n" +
                $"Ataque: {enemy.attack}\n" +
                $"Tipo: {enemy.elementType}\n" +
                $"Número de habilidades: {enemy.GetSkills().Length}\n";
        }
    }
}

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverScreenController : MonoBehaviour
{
    [Header("Textos del menú")]
    public TextMeshProUGUI reiniciar_text;
    public TextMeshProUGUI salir_text;

    [Header("Colores")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private int selectedIndex = 0;
    private TextMeshProUGUI[] options;

    private void Start()
    {
        options = new TextMeshProUGUI[] { reiniciar_text, salir_text };

        // Colores iniciales
        foreach (var t in options)
            t.color = normalColor;

        HighlightSelected();
    }

    private void Update()
    {
        HandleKeyboardNavigation();
        HandleSelection();
    }

    private void HandleKeyboardNavigation()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
            HighlightSelected();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % options.Length;
            HighlightSelected();
        }
    }

    private void HighlightSelected()
    {
        for (int i = 0; i < options.Length; i++)
            options[i].color = (i == selectedIndex) ? hoverColor : normalColor;
    }

    private void HandleSelection()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            ActivateOption(selectedIndex);
    }

    //  MÉTODOS PARA MOUSE
    public void OnMouseEnterOption(int index)
    {
        selectedIndex = index;
        HighlightSelected();
    }

    public void OnMouseClickOption(int index)
    {
        ActivateOption(index);
    }

    private void ActivateOption(int index)
    {
        switch (index)
        {
            case 0: // Reiniciar
                CoroutineRunner.Instance.StartCoroutine(RestartGame());
                break;

            case 1: // Salir
                #if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
                #else
                                Application.Quit();
                #endif
                                break;
        }
    }
    
    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f); // pequeña pausa antes del reinicio
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name); // recarga la escena actual
    }
}
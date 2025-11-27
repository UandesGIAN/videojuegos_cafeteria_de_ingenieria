using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Textos del menú")]
    public TextMeshProUGUI comenzar_text;
    public TextMeshProUGUI manual_text;
    public TextMeshProUGUI salir_text;

    [Header("Colores")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    [Header("URL del Manual de Instrucciones")]
    public string manualURL = "url";

    private int selectedIndex = 0;
    private TextMeshProUGUI[] options;

    private void Start()
    {
        options = new TextMeshProUGUI[] { comenzar_text, manual_text, salir_text };

        // Colores iniciales
        foreach (var t in options)
            t.color = normalColor;

        HighlightSelected();
        
        // Iniciar música del menú
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMenuMusic();
        }
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
            case 0: // Comenzar
                RoomManager.Instance.GoToNextRoom();
                break;

            case 1: // Manual
                Application.OpenURL(manualURL);
                break;

            case 2: // Salir
                #if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
                #else
                                Application.Quit();
                #endif
                                break;
                        }
                    }
                }

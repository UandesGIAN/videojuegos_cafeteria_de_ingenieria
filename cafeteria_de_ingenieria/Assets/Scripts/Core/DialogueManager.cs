using TMPro;
using UnityEngine;


// singleton para manejar dialogos de enemigos y sus acciones
public class DialogueManager : MonoBehaviour
{
    // idealmente, esta instancia solo es llamada cuando se tenga por certeza que el BattleHUD y el DialoguePanel estan activos!!
    public static DialogueManager Instance;

    [Header("Referencias de Diálogo")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    private void Awake()
    {
        Debug.Log("INICIANDO DIALOGUE MANAGER...");
        if (!Instance) Instance = this;
        
        DontDestroyOnLoad(this.gameObject);

        // getteo de paneles porsiacaso
        if (!dialoguePanel) GameObject.Find("DialogueBox");
        if (!dialogueText) GameObject.Find("Dialogue");
        
        if (!dialoguePanel || !dialogueText) Debug.Log("NO SE PUDIERON ENCONTRAR PANELES DE DIALOGO");

        Debug.Log("\tDialoguePanel: " + dialoguePanel.name);
        Debug.Log("\tDialogueText: " + dialogueText.name);
    }

    public void ActivateDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            Debug.Log("DIALOGUE PANEL ACTIVATED IN DIALOGUEMANAGER");
        }
    }

    // metodo genérico para mostrar dialogos
    public void ShowDialogue(string sentence)
    {
        if (!dialoguePanel) 
        {
            Debug.LogWarning("Dialogue Panel no está asignado en DialogueManager.");
            return;
        }

        if (string.IsNullOrEmpty(sentence))
            sentence = "FALTA PONER DIÁLOGO ACÁ...";

        // mostrar dialogo!!! 
            // en batalla, siempre habrá un dialogo que mostrar, por lo que no es necesario que texto sea vacío
            // afuera de batalla, el texto se pone vacio.
        dialoguePanel.SetActive(true);
        dialogueText.text = sentence;
        Debug.Log("MOSTRANDO DIALOGO: " + sentence);

        //while (!player.HasAttacked)
        //{
        //    Debug.Log("Esperando a que el jugador ataque para continuar el diálogo...");
        //    yield return null; // Espera un frame
        //}

        // Ocultar el diálogo una vez que atacó
        //dialogueText.text = "";
    }
}
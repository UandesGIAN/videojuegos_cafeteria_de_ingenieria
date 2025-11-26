using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Animations;

public class RoomController : MonoBehaviour
{
    public static RoomController Instance { get; private set; }

    [Header("Componentes de la sala")]
    public BattleManager battle;
    public UpgradeSelection upgradeSelection;
    public GameObject roomSelectionUI; // Para elegir siguiente sala
    public GameObject battleHUD;

    [Header("Salas disponibles")]
    public List<GameObject> nextRoomObjects; // Ahora asignamos los GameObjects directamente
    private List<RoomController> nextRooms = new List<RoomController>();
    private Button[] roomButtons = new Button[0];
    private bool nextRoomsInitialized = false;


    // para cheats
    private string cheatBuffer = "";
    private bool cheatModeActive = false;

    private void Awake()
    {
        if (battleHUD != null)
            battleHUD.SetActive(false);

        if (upgradeSelection != null)
            upgradeSelection.gameObject.SetActive(false);
    }

    private void Start()
    {
        RoomManager.Instance.MarkRoomVisited(this);
    }

    private void Update()
    {
        DetectRoomCheat();
    }

    private void DetectRoomCheat()
    {
        // SHIFT + CTRL presionados
        bool mods = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                    (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));

        if (!mods)
        {
            // Si se sueltan SHIFT/CTRL y había número → ejecutar cheat
            if (cheatModeActive && cheatBuffer.Length > 0)
            {
                int roomNumber = int.Parse(cheatBuffer);
                cheatBuffer = "";
                cheatModeActive = false;
                TeleportToRoom(roomNumber);
            }
            return;
        }

        cheatModeActive = true;

        // Leer números del 0 al 9 (teclado normal + numpad)
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) ||
                Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                cheatBuffer += i.ToString();

                // Limitar a 2 dígitos
                if (cheatBuffer.Length > 2)
                    cheatBuffer = cheatBuffer.Substring(1);

                Debug.Log("[CHEAT] Sala ingresada: " + cheatBuffer);
            }
        }
    }

    private void InitializeNextRooms()
    {
        if (nextRoomsInitialized) return; // Evita hacerlo más de una vez
        nextRoomsInitialized = true;

        if (nextRoomObjects == null || nextRoomObjects.Count == 0)
        {
            Debug.LogWarning($"[RoomController: {name}] No hay salas siguientes asignadas.");
            return;
        }
    
        nextRooms.Clear();
        foreach (var obj in nextRoomObjects)
        {
            if (obj == null)
            {
                Debug.LogWarning($"[RoomController: {transform.parent.gameObject}] nextRoomObjects contiene un elemento null, se ignora.");
                continue;
            }

            obj.SetActive(true); // Activamos temporalmente solo para leer el componente
            RoomController rc = obj.GetComponentInChildren<RoomController>(true);
            if (rc != null)
            {
                nextRooms.Add(rc);
                obj.SetActive(false); // Volvemos a desactivarla después de registrar
                Debug.Log($"[RoomController: {transform.parent.gameObject}] Cargada referencia de {obj.name}");
            }
            else
            {
                Debug.LogWarning($"[RoomController: {transform.parent.gameObject}] El GameObject {obj.name} no tiene RoomController.");
            }
        }
    }
    
    public void EnterRoom()
    {
        roomSelectionUI.SetActive(false);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        if (battle != null)
        {
            Debug.Log($"[RoomController: {transform.parent.gameObject}] Iniciando batalla...");
            
            // Cambiar a música de batalla en la primera room con combate
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayBattleMusic();
            }

            battle.Start(); // para que agarre bien el enemy y player

            if (battleHUD != null)
                battleHUD.SetActive(true); // Activar HUD de batalla

            battle.gameObject.SetActive(true);
            battle.StartBattle(() => OnBattleEnd());
        }
        else
        {
            Debug.Log($"[RoomController: {name}] No hay batalla, saltando a selección de mejora.");
            OnBattleEnd();
        }
    }

    public void OnBattleEnd()
    {
        Debug.Log($"[RoomController: {transform.parent.gameObject}] Batalla finalizada.");
        if (battleHUD != null)
            battleHUD.SetActive(false);

        // Cambiar a música de batalla en la primera room con combate
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMenuMusic();
        }
        
        // Activar selección de mejoras
        if (upgradeSelection != null)
        {
            Debug.Log($"[RoomController: {transform.parent.gameObject}] Mostrando mejoras.");
            upgradeSelection.gameObject.SetActive(true);
            upgradeSelection.ShowUpgrades();
        }
        else
        {
            Debug.Log($"[RoomController: {transform.parent.gameObject}] No hay mejoras, pasando a selección de sala.");
            OnUpgradeSelected(); // Si no hay mejoras, pasar directo a seleccion
        }
    }

    public void OnUpgradeSelected()
    {
        // Ahora habilitamos la selección de la próxima sala
        Debug.Log($"[RoomController: {transform.parent.gameObject}] Habilitando UI de selección de sala.");
        if (upgradeSelection != null)
            upgradeSelection.gameObject.SetActive(false);
        // Salsas futuras
        InitializeNextRooms();
        Transform t = roomSelectionUI.transform;
        while (t != null)
        {
            t.gameObject.SetActive(true);
            t = t.parent;
        }
        roomSelectionUI.SetActive(true);
        foreach (var btn in roomButtons)
        {
            Debug.Log($"{btn.name} interactable: {btn.interactable}, activeInHierarchy: {btn.gameObject.activeInHierarchy}");
        }


        // Botones con las salas válidas
        List<RoomController> validRooms = RoomManager.Instance.GetNextAvailableRooms(nextRooms);
        roomButtons = roomSelectionUI.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < roomButtons.Length; i++)
        {
            // Agregar componente de sonido si no existe
            if (roomButtons[i].GetComponent<UIButtonSound>() == null)
            {
                UIButtonSound buttonSound = roomButtons[i].gameObject.AddComponent<UIButtonSound>();
                buttonSound.soundType = UIButtonSound.ButtonSoundType.Confirm;
            }
            
            if (i < validRooms.Count)
            {
                roomButtons[i].gameObject.SetActive(true);
                int index = i;
                roomButtons[i].onClick.RemoveAllListeners();
                roomButtons[i].onClick.AddListener(() => GoToRoom(validRooms[index]));
            }
            else
            {
                roomButtons[i].gameObject.SetActive(false);
            }
        }
    }


    private void GoToRoom(RoomController nextRoom)
    {
        Debug.Log($"[RoomController: {transform.parent.gameObject}] Cambiando a sala {nextRoom.transform.parent.gameObject.name}.");

        // Ocultar room actual
        transform.parent.gameObject.SetActive(false);

        nextRoom.transform.parent.gameObject.SetActive(true);
        nextRoom.EnterRoom();
    }

    private void TeleportToRoom(int inputNumber)
    {
        var rooms = RoomManager.Instance.allRoomObjects;
        int total = rooms.Count;

        // Salas jugables están entre índice 2 y total - 3
        int firstPlayableIndex = 2;
        int lastPlayableIndex = total - 3;

        int playableCount = lastPlayableIndex - firstPlayableIndex + 1;

        // El jugador NO está ingresando el índice real
        // Está ingresando el número "humano" que corresponde a las salas jugables (del 0 al XX)
        // Entonces convertimos:
        int targetIndex = firstPlayableIndex + inputNumber;

        // Validación: si se pasa del límite, no hace nada
        if (inputNumber <= 0 || targetIndex > lastPlayableIndex)
        {
            Debug.LogWarning($"[CHEAT] Sala inválida. Solo hay {playableCount} salas jugables.");
            return;
        }

        Debug.Log($"[CHEAT] Teletransportando a la sala #{inputNumber} (índice real {targetIndex}).");

        // Detener batalla actual si existe
        if (battle != null)
        {
            try { battle.EndBattle(); } 
            catch { Debug.LogWarning("Error al terminar batalla"); }
        }

        // Cerrar upgrade selection si está abierta
        if (upgradeSelection != null)
            upgradeSelection.gameObject.SetActive(false);

        // Apagar sala actual
        transform.parent.gameObject.SetActive(false);

        // Activar la sala destino
        GameObject targetObj = rooms[targetIndex];
        targetObj.SetActive(true);

        RoomController rc = targetObj.GetComponentInChildren<RoomController>(true);

        if (rc != null)
        {
            RoomManager.Instance.MarkRoomVisited(rc);
            rc.EnterRoom();
        }
        else
        {
            Debug.LogError("[CHEAT] La sala destino no tiene RoomController");
        }
    }
}

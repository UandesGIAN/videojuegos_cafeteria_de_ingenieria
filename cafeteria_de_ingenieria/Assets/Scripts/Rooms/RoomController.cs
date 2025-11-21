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
}

using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Todas las salas del juego")]
    public List<GameObject> allRoomObjects;

    private List<RoomController> visitedRooms = new List<RoomController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Desactivar todas las salas desde el inicio
        if (allRoomObjects != null && allRoomObjects.Count > 0)
        {
            foreach (var obj in allRoomObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    private void Start()
    {
        if (allRoomObjects == null || allRoomObjects.Count == 0) return;

        // Activar solo la primera sala = SELECCION DE PERSONAJE
        allRoomObjects[0].SetActive(true);
    }

    public void GoToNextRoom()
    {
        if (allRoomObjects.Count < 2)
        {
            Debug.LogWarning("RoomManager: No hay una segunda sala configurada.");
            return;
        }

        // Desactivar la sala de selección
        allRoomObjects[0].SetActive(false);

        // Activar la siguiente (la cafeteria)
        allRoomObjects[1].SetActive(true);

        RoomController nextRoom = allRoomObjects[1].GetComponentInChildren<RoomController>(true);
        if (nextRoom != null)
        {
            nextRoom.EnterRoom();
            MarkRoomVisited(nextRoom);
        }
        else
        {
            Debug.LogError($"La sala '{allRoomObjects[1].name}' no tiene RoomController.");
        }
    }

    public List<RoomController> GetNextAvailableRooms(List<RoomController> possibleRooms)
    {
        List<RoomController> available = new List<RoomController>();
        foreach (var room in possibleRooms)
        {
            if (!visitedRooms.Contains(room))
                available.Add(room);
        }
        return available;
    }

    public void MarkRoomVisited(RoomController room)
    {
        if (!visitedRooms.Contains(room))
            visitedRooms.Add(room);
    }
}

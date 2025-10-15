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

        // Activar solo la primera sala
        allRoomObjects[0].SetActive(true);

        RoomController firstRoom = allRoomObjects[0].GetComponentInChildren<RoomController>(true);
        if (firstRoom != null)
        {
            firstRoom.EnterRoom();
            MarkRoomVisited(firstRoom);
        }
        else
        {
            Debug.LogError($"La primera sala ({allRoomObjects[0].name}) no tiene RoomController.");
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

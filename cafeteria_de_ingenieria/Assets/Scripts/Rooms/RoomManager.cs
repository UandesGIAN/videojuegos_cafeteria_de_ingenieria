using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Todas las salas del juego")]
    public List<GameObject> allRoomObjects;

    private List<RoomController> visitedRooms = new List<RoomController>();
    private int currentRoomIndex = 0;

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

        // Activar solo la primera sala = MAIN MENU
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
        allRoomObjects[currentRoomIndex].SetActive(false);

        // Avanzar al siguiente índic
        currentRoomIndex++;

        if (currentRoomIndex >= allRoomObjects.Count)
        {
            Debug.Log("RoomManager: No hay más salas por avanzar.");
            return;
        }

        // Activar nueva sala
        GameObject nextRoomObject = allRoomObjects[currentRoomIndex];
        nextRoomObject.SetActive(true);

        // Proximo RoomController
        RoomController nextRoom = nextRoomObject.GetComponentInChildren<RoomController>(true);
        if (nextRoom != null)
        {
            nextRoom.EnterRoom();
            MarkRoomVisited(nextRoom);
        }
        else
        {
            Debug.Log($"La sala '{nextRoomObject.name}' NO tiene RoomController — se activa igual.");
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

    public void GoToGameOverRoom()
    {
        int gameOverIndex = allRoomObjects.FindIndex(room => room.name == "GameOverScreen");
        
        if (gameOverIndex == -1)
        {
            Debug.LogError("RoomManager: No se encontró una sala llamada 'GameOverScreen'");
            return;
        }

        // Apagar TODAS las salas
        for (int i = 0; i < allRoomObjects.Count; i++)
        {
            allRoomObjects[i].SetActive(i == gameOverIndex);
        }

        // Actualizar índice
        currentRoomIndex = gameOverIndex;

        Debug.Log("RoomManager: Cargando sala de GAME OVER.");
    }
}

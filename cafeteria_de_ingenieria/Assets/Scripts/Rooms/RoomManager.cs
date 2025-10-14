using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public RoomData currentRoom;
    public List<RoomData> availableRooms;

    public void LoadRoom(RoomData room)
    {
        currentRoom = room;
        // Cambia fondo, enemigo y UI
        //GameManager.Instance.ChangeState(GameState.Battle);
        //BattleManager.Instance.StartBattle(room.enemy);
    }

    public void ShowNextRooms()
    {
        // Lógica para mostrar 2–3 opciones de la siguiente sala
        //currentRoom.gameObject.SetActive(false);

        //nextRoom.gameObject.SetActive(true);
    }
}

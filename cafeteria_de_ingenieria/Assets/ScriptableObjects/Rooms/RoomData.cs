using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rooms/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;
    public Sprite background;
    public EnemyData enemy;
    public List<RoomData> nextRooms;
}

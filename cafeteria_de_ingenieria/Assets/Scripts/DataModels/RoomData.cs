using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Game/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;
    public Image backgroundImage;
    public GameObject enemyPrefab;
    //public AudioClip music;
}
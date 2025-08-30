using UnityEngine;
 

[CreateAssetMenu(fileName = "NewRoom", menuName = "Game/Room")]
public class RoomData : ScriptableObject {
    public string roomName;
    public Sprite background;
    public EnemyData enemy;
    public bool isBossRoom;
    public string dialogueIntro;
}
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum GameState { Exploration, Battle, UpgradeSelection }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
    }
}

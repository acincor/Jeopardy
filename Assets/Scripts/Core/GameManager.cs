using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState CurrentState { get; private set; }

    void Awake()
    {
        Instance = this;
        CurrentState = GameState.Playing;
    }
    
    public void EndGame(GameResult result)
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = result.bossWin
            ? GameState.BossWin
            : GameState.WorkerWin;

        Time.timeScale = 0f;
        Debug.Log(result.bossWin ? "Boss Win" : "Workers Win");
    }
}

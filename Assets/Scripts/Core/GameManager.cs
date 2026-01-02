using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State { get; private set; }

    void Awake()
    {
        Instance = this;
        State = GameState.Init;
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        State = GameState.Playing;
    }

    public void EndGame()
    {
        State = GameState.End;

        Debug.Log("Game Over");
        Debug.Log($"Machines: {GameResult.destroyedMachines}");
        Debug.Log($"Catches: {GameResult.totalCatches}");
    }
}

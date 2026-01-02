using UnityEngine;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance;

    [Header("Win Conditions")]
    public int machinesToWin = 3;
    public int catchesToWin = 5;
    public int workersToResign = 1;   // 至少多少人成功撤离

    private int resignedWorkers = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        GameResult.Reset();
    }

    void OnEnable()
    {
        // ★ 监听事件，而不是被 Machine 直接调用
        MachineEvents.OnMachineDestroyed += HandleMachineDestroyed;
    }

    void OnDisable()
    {
        MachineEvents.OnMachineDestroyed -= HandleMachineDestroyed;
    }

    // ================= 机器 =================

    void HandleMachineDestroyed(Machine machine)
    {
        if (GameResult.gameEnded)
            return;

        GameResult.destroyedMachines++;

        if (GameResult.destroyedMachines >= machinesToWin)
        {
            WorkerWin();
        }
    }

    // ================= 抓捕 =================

    public void OnWorkerCaught()
    {
        if (GameResult.gameEnded)
            return;

        GameResult.totalCatches++;

        if (GameResult.totalCatches >= catchesToWin)
        {
            BossWin();
        }
    }

    // ================= 撤离 =================

    public bool CanWorkerResign(WorkerController worker)
    {
        if (worker == null)
            return false;

        if (worker.IsCaught)
            return false;

        if (GameResult.gameEnded)
            return false;

        return true;
    }

    public void WorkerResignSuccess(WorkerController worker)
    {
        if (GameResult.gameEnded)
            return;

        resignedWorkers++;

        if (resignedWorkers >= workersToResign)
        {
            WorkerWin();
        }
    }

    // ================= 结算 =================

    void WorkerWin()
    {
        if (GameResult.gameEnded) return;

        GameResult.workerWin = true;
        GameResult.bossWin = false;
        GameResult.gameEnded = true;

        GameManager.Instance.EndGame();
    }

    void BossWin()
    {
        if (GameResult.gameEnded) return;

        GameResult.bossWin = true;
        GameResult.workerWin = false;
        GameResult.gameEnded = true;

        GameManager.Instance.EndGame();
    }
}

using UnityEngine;
using System.Collections.Generic;
public class GameMode : MonoBehaviour
{
    public static GameMode Instance;

    [Header("Win Conditions")]
    public int totalMachines = 5;
    public int requiredDestroyedMachines = 3;
    public int requiredCatches = 3;

    private int destroyedMachines;
    private int totalCatches;
    public WorkerController worker;
    public BossChase bossChase;
    public List<WorkerController> workers = new List<WorkerController>();
    void Awake()
    {
        Instance = this;
    }
    public void RegisterWorker(WorkerController worker)
    {
        if (!workers.Contains(worker))
            workers.Add(worker);
    }
    // ===== 员工行为 =====

    public void OnMachineDestroyed()
    {
        destroyedMachines++;

        Debug.Log($"Machine destroyed: {destroyedMachines}");

        if (destroyedMachines >= requiredDestroyedMachines)
        {
            // 注意：不是立刻胜利
            Debug.Log("Worker can now resign");
        }
    }
    void RespawnWorker()
    {
        worker.ResetToSpawn();
        bossChase.ResumeChase();
    }
    public bool CanWorkerResign()
    {
        return destroyedMachines >= requiredDestroyedMachines;
    }

    // ===== 老板行为 =====

    public void OnWorkerCaught()
    {
        Invoke(nameof(RespawnWorker), 1.5f);
        totalCatches++;
        
        if (totalCatches >= requiredCatches)
        {
            BossWin();
        }
        
    }

    // ===== 结算 =====

    public void WorkerResignSuccess()
    {
        GameResult result = new GameResult
        {
            bossWin = false,
            destroyedMachines = destroyedMachines,
            totalCatches = totalCatches
        };

        GameManager.Instance.EndGame(result);
    }
    public void OnWorkerEliminated(WorkerController worker)
    {
        CheckBossWin();
    }

    void CheckBossWin()
    {
        foreach (var w in workers)
        {
            if (!w.IsEliminated)
                return;
        }

        BossWin();
    }
    void BossWin()
    {
        GameResult result = new GameResult
        {
            bossWin = true,
            destroyedMachines = destroyedMachines,
            totalCatches = totalCatches
        };

        GameManager.Instance.EndGame(result);
    }
}

using UnityEngine;
using System.Collections.Generic;

public class Machine : MonoBehaviour
{
    [Header("Destroy Settings")]
    public float destroyTime = 5f;

    private float progress = 0f;
    private List<WorkerController> workers = new();

    public bool IsDestroyed { get; private set; }

    void Update()
    {
        if (IsDestroyed || workers.Count == 0)
            return;

        // 多人加速拆机（1人 = 1x，2人 = 1.5x，3人 = 2x ...）
        float speedMultiplier = 1f + (workers.Count - 1) * 0.5f;
        progress += Time.deltaTime * speedMultiplier;

        if (progress >= destroyTime)
            CompleteDestroy();
    }

    // ================= 对外接口 =================

    public void BeginDestroy(WorkerController worker)
    {
        if (IsDestroyed || worker == null)
            return;

        if (!workers.Contains(worker))
            workers.Add(worker);
    }

    public void CancelDestroy(WorkerController worker)
    {
        if (worker == null)
            return;

        workers.Remove(worker);
    }

    // ================= 内部 =================

    void CompleteDestroy()
    {
        IsDestroyed = true;

        // 通知所有参与的 Worker
        foreach (var worker in workers)
        {
            if (worker != null)
                worker.OnMachineDestroyed(this);
        }

        workers.Clear();

        // 广播事件（GameMode 监听）
        MachineEvents.RaiseMachineDestroyed(this);

        // 关闭机器
        gameObject.SetActive(false);
    }
}

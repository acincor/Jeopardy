using UnityEngine;

public class Machine : MonoBehaviour
{
    [Header("Destroy Settings")]
    public float destroyTime = 5f;

    private float progress = 0f;
    private bool isBeingDestroyed = false;
    private WorkerController currentWorker;

    public bool IsDestroyed { get; private set; }

    void Update()
    {
        if (!isBeingDestroyed || IsDestroyed || currentWorker == null)
            return;

        progress += Time.deltaTime;

        if (progress >= destroyTime)
        {
            CompleteDestroy();
        }
    }
    // ================= 原有实现 =================

    public void BeginDestroy(WorkerController worker)
    {
        Debug.Log("BeginDestroy by " + worker.name);
        if (IsDestroyed)
            return;

        // 如果已经被别人拆，直接拒绝
        if (isBeingDestroyed && currentWorker != worker)
            return;

        currentWorker = worker;
        isBeingDestroyed = true;
    }

    public void CancelDestroy()
    {
        isBeingDestroyed = false;
        currentWorker = null;
    }

    // ================= 内部 =================

    void CompleteDestroy()
    {
        IsDestroyed = true;
        isBeingDestroyed = false;

        // 通知 Worker
        if (currentWorker != null)
            currentWorker.OnMachineDestroyed(this);

        // 通知 GameMode
        GameMode.Instance.OnMachineDestroyed();

        // 视觉 / 碰撞关闭
        gameObject.SetActive(false);
    }
}

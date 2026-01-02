using UnityEngine;

public class BossCatch : MonoBehaviour
{
    public BossChase bossChase;
    public Transform eyePoint;
    public float catchDistance = 4f;
    public float catchCooldown = 2f;
    private float lastCatchTime = -999f;
    public LayerMask obstacleLayer;
    public LayerMask workerLayer;

    public GameMode gameMode;

    void Update()
    {
        if (Time.time - lastCatchTime < catchCooldown)
            return;
        Ray ray = new Ray(eyePoint.position, transform.forward);
        Debug.DrawRay(eyePoint.position, transform.forward * catchDistance, Color.red);

        RaycastHit hit;

        // 1️⃣ 先检查前方最近的障碍物
        if (Physics.Raycast(ray, out hit, catchDistance, obstacleLayer))
        {
            // 前面有墙，直接 return，隔墙不能抓
            return;
        }
        // 2️⃣ 没有墙，再检查 Worker
        if (Physics.Raycast(ray, out hit, catchDistance, workerLayer))
        {
            WorkerController worker = hit.collider.GetComponent<WorkerController>();
            if (worker != null && !worker.IsCaught && !worker.IsInvincible)
            {
                Debug.Log("2");
                worker.Caught();
                gameMode.OnWorkerCaught();
                bossChase.LoseTarget();
                bossChase.PauseThenResume(1.0f);
            }
        }
        lastCatchTime = Time.time;
    }
}

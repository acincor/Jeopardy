using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossChase : MonoBehaviour
{
    [Header("AI Switch")]
    public bool enableAI = true;

    [Header("Detection")]
    public float detectRadius = 12f;
    public float loseTargetDistance = 18f;
    public LayerMask workerLayer;

    [Header("Update Interval")]
    public float decisionInterval = 0.3f;

    private NavMeshAgent agent;
    private BossController bossController;

    private WorkerController currentTarget;
    private float decisionTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        bossController = GetComponent<BossController>();
    }

    void Update()
    {
        if (!enableAI) return;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer > 0f) return;

        decisionTimer = decisionInterval;

        Think();
        Move();
    }

    // =========================
    // AI 决策
    // =========================
    void Think()
    {
        if (currentTarget == null)
        {
            AcquireTarget();
            return;
        }

        // 目标失效
        if (currentTarget.IsCaught ||
            Vector3.Distance(transform.position, currentTarget.transform.position) > loseTargetDistance)
        {
            LoseTarget();
        }
    }

    // =========================
    // 目标选择（博弈核心）
    // =========================
    void AcquireTarget()
    {
        // 1️⃣ 拆机器的（最高优先级）
        WorkerController destroying = FindWorkerDestroyingMachine();
        if (destroying != null)
        {
            SetTarget(destroying);
            return;
        }

        // 2️⃣ 最近的
        WorkerController nearest = FindNearestWorker();
        if (nearest != null)
        {
            SetTarget(nearest);
        }
    }

    WorkerController FindWorkerDestroyingMachine()
    {
        foreach (WorkerController worker in FindAllWorkersInRange())
        {
            if (worker.IsDestroyingMachine)
                return worker;
        }
        return null;
    }

    WorkerController FindNearestWorker()
    {
        float minDist = float.MaxValue;
        WorkerController result = null;

        foreach (WorkerController worker in FindAllWorkersInRange())
        {
            float dist = Vector3.Distance(transform.position, worker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                result = worker;
            }
        }
        return result;
    }

    List<WorkerController> FindAllWorkersInRange()
    {
        List<WorkerController> result = new();
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, workerLayer);

        foreach (Collider hit in hits)
        {
            WorkerController worker = hit.GetComponent<WorkerController>();
            if (worker != null && !worker.IsCaught)
                result.Add(worker);
        }
        return result;
    }

    // =========================
    // 移动 & 状态输出
    // =========================
    void Move()
    {
        if (currentTarget == null)
        {
            bossController.SetChasing(false);
            return;
        }

        bossController.SetChasing(true);
        agent.SetDestination(currentTarget.transform.position);
    }

    // =========================
    // Target 生命周期
    // =========================
    void SetTarget(WorkerController worker)
    {
        currentTarget = worker;
    }

    public void LoseTarget()
    {
        currentTarget = null;
        bossController.SetChasing(false);
    }

    // =========================
    // 外部输入（Controller / Trigger）
    // =========================

    public void OnTargetCaught()
    {
        LoseTarget();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
#endif
}

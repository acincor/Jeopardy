using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossChase : BossController
{
    public bool enabledAI;
    private float findTimer;
    private const float FIND_INTERVAL = 0.2f; // 5 次 / 秒，足够
    private Vector3 lastTargetPosition;

    public override void Start()
    {
        base.Start();
        findTimer = 0f;
    }

    void Update()
    {
        if (!enabledAI)
            return;

        // ---------- 降频查找目标 ----------
        findTimer -= Time.deltaTime;
        if (findTimer <= 0f || target == null)
        {
            findTimer = FIND_INTERVAL;
            DiscoverTarget();
        }

        if (target == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        // ---------- 只有目标移动时才重算路径 ----------
        if ((target.transform.position - lastTargetPosition).sqrMagnitude > 0.1f)
        {
            lastTargetPosition = target.transform.position;
            agent.SetDestination(lastTargetPosition);
        }
    }
    public override void DiscoverTarget()
    {
        float closestSqr = float.MaxValue;
        WorkerController closest = null;
        Vector3 myPos = transform.position;
        List<WorkerController> targets = WorkerController.All;
        foreach (var t in targets)
        {
            if(closest != null && closest.state == WorkerState.Destroying && t.state != WorkerState.Destroying)
                continue;
            float sqr = (t.transform.position - myPos).sqrMagnitude;
            if (sqr < closestSqr)
            {
                closestSqr = sqr;
                closest = t;
            }
        }
        target = closest;
        if (target != null) {
            lastTargetPosition = target.transform.position;
        }
    }
}

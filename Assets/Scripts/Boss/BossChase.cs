using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossChase : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;

    public bool isChasing;
    private bool canChase = true;

    private SafetyZone lastZone; // 上一次所在 Zone

    [Header("Speed Settings")]
    public float baseSpeed = 3.5f;
    public float dangerousZoneSpeedMultiplier = 0.85f; // 板区 / 危险区减速
    public float angrySpeedMultiplier = 0.8f;           // 愤怒减速

    [Header("Angry Settings")]
    public float timeToGetAngry = 8f;    // 连续追击多久进入愤怒
    public float angryDuration = 4f;     // 愤怒持续时间

    private float chaseTimer = 0f;
    private bool isAngry = false;

    void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        baseSpeed = agent.speed;
    }

    void Update()
    {
        // ★ 玩家控制时，AI 不运行（兼容真人切换）
        if (BossController.Instance != null &&
            BossController.Instance.IsPlayerControl())
            return;

        if (!isChasing || !canChase || target == null)
            return;

        chaseTimer += Time.deltaTime;

        if (!isAngry && chaseTimer >= timeToGetAngry)
        {
            StartCoroutine(AngryRoutine());
        }

        // ===== Zone 处理 =====
        WorkerController worker = target.GetComponent<WorkerController>();
        SafetyZone currentZone = null;

        if (worker != null)
            currentZone = worker.CurrentZone;

        if (currentZone != lastZone)
        {
            if (lastZone != null)
                lastZone.bossChasingInside = false;

            if (currentZone != null)
                currentZone.bossChasingInside = true;

            lastZone = currentZone;
        }

        // ===== 速度计算 =====
        float speed = baseSpeed;

        if (currentZone != null && currentZone.IsDangerous)
        {
            speed *= dangerousZoneSpeedMultiplier;
        }

        if (isAngry)
        {
            speed *= angrySpeedMultiplier;
        }

        agent.speed = speed;

        agent.SetDestination(target.position);
    }
    // 当前 Boss 是否在危险区
public bool IsInDangerousZone()
{
    return lastZone != null && lastZone.IsDangerous;
}

// 当前速度倍率（统一规则出口）
public float GetSpeedMultiplier()
{
    float multiplier = 1f;

    if (IsInDangerousZone())
        multiplier *= dangerousZoneSpeedMultiplier;

    if (isAngry)
        multiplier *= angrySpeedMultiplier;

    return multiplier;
}
    // ================= 状态控制 =================

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isChasing = true;
        canChase = true;
        chaseTimer = 0f;
        isAngry = false;

        agent.isStopped = false;
    }

    public void LoseTarget()
    {
        isChasing = false;
        target = null;

        PauseChase();
        ClearZoneState();
        ResetAngryState();
    }

    public void PauseChase()
    {
        canChase = false;
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void ResumeChase()
    {
        agent.isStopped = false;
        canChase = true;
    }

    public void PauseThenResume(float seconds)
    {
        StopAllCoroutines();
        ClearZoneState();
        ResetAngryState();
        StartCoroutine(PauseRoutine(seconds));
    }

    IEnumerator PauseRoutine(float seconds)
    {
        PauseChase();
        yield return new WaitForSeconds(seconds);
        ResumeChase();
    }

    // ================= 愤怒逻辑 =================

    IEnumerator AngryRoutine()
    {
        isAngry = true;
        yield return new WaitForSeconds(angryDuration);
        ResetAngryState();
    }

    void ResetAngryState()
    {
        isAngry = false;
        chaseTimer = 0f;
        agent.speed = baseSpeed;
    }

    // ================= Zone 清理 =================

    void ClearZoneState()
    {
        if (lastZone != null)
        {
            lastZone.bossChasingInside = false;
            lastZone = null;
        }

        SafetyZone[] zones = FindObjectsOfType<SafetyZone>();
        foreach (var z in zones)
            z.bossChasingInside = false;
    }
}

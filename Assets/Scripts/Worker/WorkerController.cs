using UnityEngine;
using UnityEngine.AI;
using System.Collections;
[RequireComponent(typeof(NavMeshAgent))]
public class WorkerController : MonoBehaviour
{
    [Header("Movement")]
    public float baseMoveSpeed = 4.5f;

    [Header("Invincible")]
    public float invincibleTime = 2f;
    public float InvincibleUntil { get; private set; }

    [Header("Machine Detect")]
    public float machineDetectRadius = 2.5f;
    public LayerMask machineLayer;

    [Header("Runtime")]
    public WorkerState currentState = WorkerState.Idle;
    public WorkerLifeState LifeState { get; private set; } = WorkerLifeState.Alive;

    public bool IsCaught => currentState == WorkerState.Caught;
    public bool IsEliminated => LifeState == WorkerLifeState.Eliminated;
    public bool IsDestroyingMachine => currentState == WorkerState.Destroying;
    public bool IsInvincible => Time.time < InvincibleUntil;
    
    private NavMeshAgent agent;
    [Header("Catch")]
    public float caughtDuration = 3f;   // 被抓后停留时间
    public Transform spawnPos;        // 出生点（拖拽）

    private Coroutine caughtRoutine;

    private Machine currentMachine;

    // Zone 只保存“我在哪”，不管逻辑
     public SafetyZone CurrentZone { get; private set; }
    // ================= 生命周期 =================

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = baseMoveSpeed;
    }

    void Update()
    {
        if (IsEliminated || IsCaught)
            return;

        HandleMovementInput();
        HandleDestroyInput();
    }

    // ================= 移动 =================

    void HandleMovementInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0, v);

        if (input.sqrMagnitude < 0.01f)
        {
            if (currentState == WorkerState.Moving)
                currentState = WorkerState.Idle;
            return;
        }

        currentState = WorkerState.Moving;

        Vector3 targetPos = transform.position + input.normalized;
        agent.SetDestination(targetPos);
    }

    // ================= 拆机器 =================

    void HandleDestroyInput()
    {
        if (!Input.GetKey(KeyCode.C))
        {
            CancelDestroy();
            return;
        }

        if (currentMachine != null)
            return;

        Machine nearby = FindNearbyMachine();
        if (nearby != null)
        {
            BeginDestroy(nearby);
        }
    }

    Machine FindNearbyMachine()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            machineDetectRadius,
            machineLayer
        );

        float minDist = float.MaxValue;
        Machine result = null;

        foreach (Collider hit in hits)
        {
            Machine m = hit.GetComponent<Machine>();
            if (m == null || m.IsDestroyed)
                continue;

            float dist = Vector3.Distance(transform.position, m.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                result = m;
            }
        }

        return result;
    }

    void BeginDestroy(Machine machine)
    {
        currentMachine = machine;
        currentState = WorkerState.Destroying;
        agent.ResetPath();

        machine.BeginDestroy(this);
    }

    void CancelDestroy()
    {
        if (currentMachine == null)
            return;

        currentMachine.CancelDestroy(this);
        currentMachine = null;

        if (currentState == WorkerState.Destroying)
            currentState = WorkerState.Idle;
    }

    public void OnMachineDestroyed(Machine machine)
    {
        if (currentMachine == machine)
        {
            currentMachine = null;
            currentState = WorkerState.Idle;
        }
    }

    // ================= 抓捕 =================

    public void Caught()
    {
        if (Time.time < InvincibleUntil)
            return;

        currentState = WorkerState.Caught;
        GameMode.Instance.OnWorkerCaught();// 结算一次
        // 开始处理被抓流程
        caughtRoutine = StartCoroutine(CaughtProcess());
    }
    IEnumerator CaughtProcess()
    {
        // 1️⃣ 等待 3 秒
        yield return new WaitForSeconds(caughtDuration);

        // 2️⃣ 回出生点
        ResetToSpawn();
    }
    public void ResetToSpawn()
    {
        agent.isStopped = true;
        agent.Warp(spawnPos.position);
        agent.isStopped = false;
        currentState = WorkerState.Idle;
        InvincibleUntil = Time.time + invincibleTime;
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, machineDetectRadius);
    }
#endif

    void OnTriggerEnter(Collider other)
    {
        SafetyZone zone = other.GetComponent<SafetyZone>();
        if (zone != null)
        {
            CurrentZone = zone;
            zone.OnWorkerEnter();
        }
    }

    void OnTriggerExit(Collider other)
    {
        SafetyZone zone = other.GetComponent<SafetyZone>();
        if (zone != null && zone == CurrentZone)
        {
            zone.OnWorkerExit();
            CurrentZone = null;
        }
    }


}

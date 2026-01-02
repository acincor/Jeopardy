using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public float moveSpeed = 5f;

    [Header("Destroy Machine")]
    public float interactRange = 2f;
    public LayerMask machineLayer;

    [Header("Life")]
    public float invincibleTime = 2f;
    private float invincibleUntil;
    public bool IsInvincible => Time.time < invincibleUntil;
    public WorkerLifeState LifeState { get; private set; } = WorkerLifeState.Alive;
    public bool IsEliminated => LifeState == WorkerLifeState.Eliminated;
    public bool IsCaught { get; private set; }

    public WorkerState currentState = WorkerState.Idle;
    public Machine currentMachine;

    public SafetyZone CurrentZone { get; private set; }

    private Vector3 spawnPos;

    void Start()
    {
        spawnPos = transform.position;
    }

    void Update()
    {
        if (IsCaught) return;

        HandleMovement();
        HandleMachineDestroy();
    }

    // =========================
    // Movement
    // =========================
    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        float zoneSlowMultiplier = 1f;
        if (CurrentZone != null)
        {
            zoneSlowMultiplier = Mathf.Lerp(
                1f,
                0.6f,
                CurrentZone.dangerLevel / CurrentZone.maxDanger
            );
        }

        controller.Move(move * moveSpeed * zoneSlowMultiplier * Time.deltaTime);
    }

    // =========================
    // Machine Destroy
    // =========================
    void HandleMachineDestroy()
    {
        // 没按 C：停止拆
        if (!Input.GetKey(KeyCode.C))
        {
            if (currentMachine != null)
            {
                currentMachine.CancelDestroy();
                currentMachine = null;
                currentState = WorkerState.Idle;
            }
            return;
        }
        // 已在拆
        if (currentMachine != null)
            return;

        // 找附近 Machine
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange,
            machineLayer
        );
        if (hits.Length == 0)
            return;

        Machine machine = hits[0].GetComponentInParent<Machine>();
if (machine == null)
{
    Debug.LogWarning("Hit but no Machine found in parent");
    return;
}

        currentMachine = machine;
        currentState = WorkerState.Destroying;
        machine.BeginDestroy(this);
    }

    // Machine 被销毁时回调
    public void OnMachineDestroyed(Machine m)
    {
        if (currentMachine == m)
        {
            currentMachine = null;
            currentState = WorkerState.Idle;
        }
    }

    // =========================
    // Zone
    // =========================
    void OnTriggerEnter(Collider other)
    {
        SafetyZone zone = other.GetComponent<SafetyZone>();
        if (zone != null)
        {
            CurrentZone = zone;
            zone.workerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        SafetyZone zone = other.GetComponent<SafetyZone>();
        if (zone != null && zone == CurrentZone)
        {
            zone.workerInside = false;
            CurrentZone = null;
        }
    }

    // =========================
    // Caught / Reset
    // =========================
    public void Caught()
    {
        IsCaught = true;
        controller.enabled = false;

        if (currentMachine != null)
        {
            currentMachine.CancelDestroy();
            currentMachine = null;
        }
    }

    public void ResetToSpawn()
    {
        controller.enabled = false;
        transform.position = spawnPos;
        controller.enabled = true;

        IsCaught = false;
        invincibleUntil = Time.time + invincibleTime;
    }
}

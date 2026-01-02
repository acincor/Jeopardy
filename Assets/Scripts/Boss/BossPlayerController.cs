using UnityEngine;
using UnityEngine.AI;

public class BossPlayerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public BossChase bossChase;

    public float inputMoveDistance = 2.0f;

    void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (bossChase == null)
            bossChase = GetComponent<BossChase>();
    }

    void Update()
    {
        if (BossController.Instance == null ||
            !BossController.Instance.IsPlayerControl())
            return;

        HandleMovement();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0, v);

        if (input.sqrMagnitude < 0.01f)
        {
            agent.ResetPath();
            return;
        }

        // === 摄像机方向 ===
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = camForward.normalized * v + camRight.normalized * h;
        Vector3 dest = transform.position + moveDir.normalized * inputMoveDistance;

        // === 速度倍率（共享规则）===
        float speedMultiplier = 1f;
        if (bossChase != null)
            speedMultiplier = bossChase.GetSpeedMultiplier();

        agent.speed = bossChase.baseSpeed * speedMultiplier;
        agent.SetDestination(dest);
    }
}

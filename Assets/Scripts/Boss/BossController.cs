using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    public Transform target;
    public static BossController Instance;

    [Header("Control Mode")]
    public BossControlMode controlMode = BossControlMode.AI;

    [Header("Refs")]
    public BossChase bossChase;
    public NavMeshAgent agent;

    void Awake()
    {
        Instance = this;

        if (agent == null) {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = true;
            agent.updatePosition = true;
        }
    }

    void Update()
    {
        // 调试用：P 键切换 AI / 玩家
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleControlMode();
        }
    }

    public void ToggleControlMode()
    {
        if (controlMode == BossControlMode.AI)
            SetPlayerControl();
        else
            SetAIControl();
    }

    public void SetPlayerControl()
    {
        controlMode = BossControlMode.Player;

        // ★ 核心：AI 必须完全停
        bossChase.PauseChase();
        agent.ResetPath();
    }

    public void SetAIControl()
    {
        controlMode = BossControlMode.AI;

        bossChase.ResumeChase();
    }

    public bool IsPlayerControl()
    {
        return controlMode == BossControlMode.Player;
    }
}

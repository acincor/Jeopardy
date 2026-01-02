using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossController : MonoBehaviour
{
    public float baseSpeed = 3.5f;
    public float chaseBonus = 1.2f;
    public float zoneBonus = 1.15f;

    private NavMeshAgent agent;
    private bool isChasing;
    private SafetyZone currentZone;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdateSpeed();
    }

    public void SetChasing(bool chasing)
    {
        isChasing = chasing;
        UpdateSpeed();

        if (currentZone != null)
        {
            if (chasing)
                currentZone.OnBossChaseEnter();
            else
                currentZone.OnBossChaseExit();
        }
    }

    public void SetZone(SafetyZone zone)
    {
        // 离开旧 zone
        if (currentZone != null && isChasing)
            currentZone.OnBossChaseExit();

        currentZone = zone;

        // 进入新 zone
        if (currentZone != null && isChasing)
            currentZone.OnBossChaseEnter();

        UpdateSpeed();
    }

    void UpdateSpeed()
    {
        float speed = baseSpeed;

        if (isChasing)
            speed *= chaseBonus;

        if (currentZone != null && currentZone.IsDangerous)
            speed *= zoneBonus;

        agent.speed = speed;
    }
}

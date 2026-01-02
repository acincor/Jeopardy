using UnityEngine;

public class SafetyZone : MonoBehaviour
{
    [Header("Danger")]
    public float dangerLevel;
    public float maxDanger = 1f;
    public float dangerIncreasePerSecond = 0.25f;
    public float dangerDecreasePerSecond = 0.5f;

    private int workerCount = 0;
    private bool bossChasingInside = false;

    public bool IsDangerous => dangerLevel >= maxDanger;

    // ===== Worker =====
    public void OnWorkerEnter()
    {
        workerCount++;
    }

    public void OnWorkerExit()
    {
        workerCount = Mathf.Max(0, workerCount - 1);
    }

    // ===== Boss =====
    public void OnBossChaseEnter()
    {
        bossChasingInside = true;
    }

    public void OnBossChaseExit()
    {
        bossChasingInside = false;
    }

    void Update()
    {
        bool activeDanger = bossChasingInside && workerCount > 0;

        if (activeDanger)
            dangerLevel += dangerIncreasePerSecond * Time.deltaTime;
        else
            dangerLevel -= dangerDecreasePerSecond * Time.deltaTime;

        dangerLevel = Mathf.Clamp(dangerLevel, 0f, maxDanger);
    }
}

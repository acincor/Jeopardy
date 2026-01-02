using UnityEngine;

public class SafetyZone : MonoBehaviour
{
    [Header("Stability")]
    public float maxStability = 100f;
    public float stability = 100f;

    public float decayPerSecond = 25f;     // 被追时消耗
    public float recoverPerSecond = 10f;   // 脱战恢复

    [Header("State")]
    public bool workerInside;
    public bool bossChasingInside;
     [Header("Runtime State")]
    public float dangerLevel = 0f;

    [Header("Config")]
    public float dangerIncreasePerSecond = 0.25f;
    public float dangerDecreasePerSecond = 0.5f;
    public float maxDanger = 1f;

    public bool IsDangerous => dangerLevel >= maxDanger;

    
    void Update()
    {
        if (bossChasingInside)
        {
            dangerLevel += dangerIncreasePerSecond * Time.deltaTime;
        }
        else
        {
            dangerLevel -= dangerDecreasePerSecond * Time.deltaTime;
        }

        dangerLevel = Mathf.Clamp(dangerLevel, 0f, maxDanger);
    
        if (workerInside && bossChasingInside)
        {
            stability -= decayPerSecond * Time.deltaTime;
        }
        else
        {
            stability += recoverPerSecond * Time.deltaTime;
        }

        stability = Mathf.Clamp(stability, 0f, maxStability);
    }

    public float StabilityPercent
    {
        get { return stability / maxStability; }
    }
}

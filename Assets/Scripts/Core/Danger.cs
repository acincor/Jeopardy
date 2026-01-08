using UnityEngine;
using System.Collections.Generic;
public class Danger : MonoBehaviour
{
    private WorkerState worker = WorkerState.Idle;
    private BossState boss = BossState.Idle;
    public void SetState(BossState bossState) {
        boss = bossState;
    }
    public void SetState(WorkerState workerState) {
        worker = workerState;
    }
    void LateUpdate()
    {
        float rate = GetDangerRate(boss, worker);
        if (rate <= 0f) return;
        List<SafetyZone> zones = SafetyZone.All;
        foreach (var zone in zones)
            zone.AddDanger(rate);
    }

    float GetDangerRate(BossState bossState, WorkerState workerState)
    {
        float dangerRate = 0f;
        switch (bossState)
        {
            case BossState.Chasing: { dangerRate += 0.005f; break;}
            case BossState.LongChasing: { dangerRate += 0.01f; break;}
            default: break;
        }
        switch (workerState)
        {
            case WorkerState.Destroying: { dangerRate += 0.03f; break;}
            case WorkerState.DestroyingWhenDangerous: { dangerRate += 0.1f; break;}
            default: break;
        }
        return dangerRate;
    }
}

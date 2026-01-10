using UnityEngine;
using System.Collections.Generic;
public class Danger : MonoBehaviour
{
    void LateUpdate()
    {
        if(Time.timeScale == 0)
            return;
        float rate = GetDangerRate();
        if (rate <= 0f) return;
        List<SafetyZone> zones = SafetyZone.All;
        float jeopardy = 0f;
        foreach (var zone in zones) {
            zone.AddDanger(rate);
            if(zone.danger > jeopardy)
                jeopardy = zone.danger;
        }
        if(jeopardy >= 0.3f) {
            List<BossController> bosses = BossController.All;
            foreach(var boss in bosses) {
                if(boss.isInZone)
                    continue;
                boss.ApplyDanger(jeopardy);
            }
        }
    }

    float GetDangerRate()
    {
        float bossRate = 0f;
        float workerRate = 0f;
        List<BossController> bosses = BossController.All;
        List<WorkerController> workers = WorkerController.All;
        foreach(var boss in bosses) {
            switch (boss.bossState)
            {
                case BossState.Chasing: { if(bossRate < 0.005f)bossRate = 0.005f; break;}
                case BossState.LongChasing: { if(bossRate < 0.01f)bossRate = 0.01f; break;}
                default: break;
            }
        }
        foreach(var worker in workers) {
            switch (worker.workerState)
            {
                case WorkerState.Destroying: { if(workerRate < 0.03f)workerRate = 0.03f; break;}
                case WorkerState.DestroyingWhenDangerous: { if(workerRate < 0.1f)workerRate = 0.1f; break;}
                default: break;
            }
        }
        return bossRate + workerRate;
    }
}

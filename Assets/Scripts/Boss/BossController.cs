using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    protected WorkerController target;
    protected NavMeshAgent agent;
    protected BossState bossState;
    protected Danger systemDanger;
    // Start is called before the first frame update
    void OnEnable() {
        systemDanger = GameObject.FindObjectOfType<Danger>();
    }
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }
    public virtual void SetState(BossState boss) {
        bossState = boss;
        systemDanger.SetState(bossState);
        Debug.Log($"danger = {systemDanger}");
    }
    public virtual void DiscoverTarget()
    {
    }
}

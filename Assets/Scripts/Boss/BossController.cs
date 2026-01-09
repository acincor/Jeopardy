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
    protected CharacterController controller;
    // Start is called before the first frame update
    void OnEnable() {
        systemDanger = GameObject.FindObjectOfType<Danger>();
    }
    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        agent.updatePosition = false;
        agent.updateRotation = false;
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

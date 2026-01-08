using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public WorkerState state;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }
    public static readonly List<WorkerController> All = new List<WorkerController>();
    void OnDisable()
    {
        All.Remove(this);
    }
    void OnEnable()
    {
        All.Add(this);
    }
    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0, v);
        Vector3 targetPos = transform.position + input.normalized;
        agent.SetDestination(targetPos);
    }
}

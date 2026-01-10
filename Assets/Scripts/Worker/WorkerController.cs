using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public WorkerState workerState;
    public Transform BornPoint;
    private CharacterController controller;
    public float speed = 5f;
    public float gravity = 9.81f;
    private Vector3 velocity;
    public bool IsCaught;
    public int CatchCount = 0;
    public bool isLeft => CatchCount == 3;
    public static readonly List<WorkerController> All = new List<WorkerController>();
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }
    void OnDisable()
    {
        All.Remove(this);
    }
    void OnEnable()
    {
        All.Add(this);
    }
    void Destroying() {
        workerState = WorkerState.Destroying;
    }
    void Update() {
        if(Time.timeScale == 0f)
            return;
        if(isLeft) {
            controller.enabled = false;
            return;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;
        controller.Move(moveDirection * speed * Time.deltaTime);
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public void Caught() {
        IsCaught = true;
        StartCoroutine(Born(3f));
    }
    private IEnumerator Born(float time)
    {
        controller.enabled = false;
        yield return new WaitForSeconds(time);
        transform.position = BornPoint.position;
        controller.enabled = true;
        IsCaught = false;
        CatchCount ++;
    }
}

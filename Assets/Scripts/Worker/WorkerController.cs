using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerController : MonoBehaviour
{
    private NavMeshAgent agent;
    public WorkerState state;
    private CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        agent.updatePosition = false;
        agent.updateRotation = false;
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
    void Destroying() {
        state = WorkerState.Destroying;
    }
    public float speed = 5f;
    public float gravity = 9.81f;
    private Vector3 velocity;
    void Update() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * x + transform.forward * z;
        controller.Move(moveDirection * speed * Time.deltaTime);
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

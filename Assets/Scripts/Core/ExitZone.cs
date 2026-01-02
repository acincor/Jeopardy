using UnityEngine;
public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        WorkerController worker = other.GetComponent<WorkerController>();
        if (worker == null) return;

        if (GameMode.Instance.CanWorkerResign(worker))
        {
            GameMode.Instance.WorkerResignSuccess(worker);
        }
        else
        {
            Debug.Log("Not enough damage done to resign");
        }
    }
}

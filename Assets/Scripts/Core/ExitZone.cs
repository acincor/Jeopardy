using UnityEngine;
public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        WorkerController worker = other.GetComponent<WorkerController>();
        if (worker == null) return;

        if (GameMode.Instance.CanWorkerResign())
        {
            GameMode.Instance.WorkerResignSuccess();
        }
        else
        {
            Debug.Log("Not enough damage done to resign");
        }
    }
}

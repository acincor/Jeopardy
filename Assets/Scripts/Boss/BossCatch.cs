using UnityEngine;

public class BossCatch : MonoBehaviour
{
    public float catchDistance = 1.5f;
    public float catchRadius = 0.6f;
    public LayerMask workerLayer;
    void Update()
    {
        TryCatch();
    }

    void TryCatch()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.8f, transform.forward);

        if (Physics.SphereCast(
            ray,
            catchRadius,
            out RaycastHit hit,
            catchDistance,
            workerLayer
        ))
        {
            WorkerController worker = hit.collider.GetComponent<WorkerController>();
            if (worker == null) return;
            if (worker.IsCaught || worker.isLeft) return;
            Catch(worker);
        }
    }

    void Catch(WorkerController worker)
    {
        Debug.Log("Catch worker: " + worker.name);
        worker.Caught();
        BossController boss = GetComponent<BossController>();
        if(boss != null)
            boss.Caught();
    }
    #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.8f, transform.forward * catchDistance);
        }
    #endif
}

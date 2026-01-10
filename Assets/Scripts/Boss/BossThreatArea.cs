using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BossThreatArea : MonoBehaviour
{
    protected BossController bossController;
    private float interval = 0f;
    private bool enter = false;
    // Start is called before the first frame update
    void Start()
    {
        bossController = GetComponentInParent<BossController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0f)
            return;
        if(enter) {
            interval += Time.deltaTime;
            if(interval >= 12f) {
                bossController.SetState(BossState.LongChasing);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(Time.timeScale == 0f)
            return;
        if (other.CompareTag("Worker"))
        {
            interval = 0f;
            enter = true;
            bossController.SetState(BossState.Chasing);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(Time.timeScale == 0f)
            return;
        if (other.CompareTag("Worker"))
        {
            enter = false;
        }
    }
}

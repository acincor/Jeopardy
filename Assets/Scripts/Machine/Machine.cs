using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{
    private float progress = 81f;
    private bool enter = false;
    private bool destroying = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
            destroying = true;
        else if(Input.GetKeyDown(KeyCode.C))
            destroying = false;
        if (progress <= 0)
            destroying = false;
        if(!enter || !destroying)
            return;
        progress -= 1f * Time.deltaTime;
        Debug.Log($"progress={progress}");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("Worker"))
        {
            enter = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Worker"))
        {
            enter = false;
        }
    }
}

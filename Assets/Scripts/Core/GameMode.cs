using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public int LeftWorkerCount = 0;
    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0)
            return;
        LeftWorkerCount = 0;
        List<WorkerController> workers = WorkerController.All;
        foreach(var worker in workers) {
            if(worker.isLeft)
                LeftWorkerCount++;
        }
        if(workers.Count == LeftWorkerCount) {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("游戏结束");
            return;
        }
    }
}

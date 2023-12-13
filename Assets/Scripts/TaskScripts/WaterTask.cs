using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTask : GenericTask
{
    WorkerScript[] coworkers;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Give a coworker some water";
    }

    private void OnLevelWasLoaded(int level)
    {
        coworkers = FindObjectsByType<WorkerScript>(0);
        destObject = GameObject.FindGameObjectWithTag("WaterFountain");
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TaskComplete && loaded)
        {
            foreach (WorkerScript coworker in coworkers)
            {
                if (coworker.receivedWater)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

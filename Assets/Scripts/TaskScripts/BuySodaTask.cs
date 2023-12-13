using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySodaTask : GenericTask
{
    SodaMachineScript[] sodaMachines;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Buy a refreshing Conk";
    }

    private void OnLevelWasLoaded(int level)
    {
        sodaMachines = FindObjectsByType<SodaMachineScript>(0);
        destObject = GameObject.FindGameObjectWithTag("VendingMachine");
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!TaskComplete && loaded)
        {
            foreach(SodaMachineScript machine in sodaMachines)
            {
                if (machine.hasDispensed)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

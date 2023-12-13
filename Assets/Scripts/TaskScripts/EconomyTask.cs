using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyTask : GenericTask
{
    ATMScript[] atms;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Participate in the Economy";
    }

    private void OnLevelWasLoaded(int level)
    {
        atms = FindObjectsByType<ATMScript>(0);
        destObject = GameObject.FindGameObjectWithTag("ATM");
        loaded = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(!TaskComplete && loaded)
        {
            foreach(ATMScript atm in atms)
            {
                if (atm.hasDispensed)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

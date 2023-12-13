using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeTask : GenericTask
{
    CoffeeMachineScript[] cofeeMachines;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Stay Alert";
    }

    private void OnLevelWasLoaded(int level)
    {
        cofeeMachines = FindObjectsByType<CoffeeMachineScript>(0);
        destObject = GameObject.FindGameObjectWithTag("CoffeeMachine");
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TaskComplete && loaded)
        {
            foreach (CoffeeMachineScript coffeeMachine in cofeeMachines)
            {
                if (coffeeMachine.hasDispensed)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

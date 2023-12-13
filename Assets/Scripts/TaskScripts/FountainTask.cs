using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainTask : GenericTask
{
    FountainScript[] fountains;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Pour A Cup Of Water";
    }

    private void OnLevelWasLoaded(int level)
    {
        fountains = FindObjectsByType<FountainScript>(0);
        destObject = GameObject.FindGameObjectWithTag("WaterFountain");
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TaskComplete && loaded)
        {
            foreach (FountainScript fountain in fountains)
            {
                if (fountain.hasPoured)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

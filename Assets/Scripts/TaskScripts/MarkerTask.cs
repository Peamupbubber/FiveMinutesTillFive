using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerTask : GenericTask
{
    CanScript[] trashCans;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Throw away an old marker";
    }

    private void OnLevelWasLoaded(int level)
    {
        trashCans = FindObjectsByType<CanScript>(0);
        destObject = GameObject.FindGameObjectWithTag("Marker");
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!TaskComplete && loaded)
        {
            foreach(CanScript trashCan in trashCans)
            {
                if (trashCan.receivedMarker)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

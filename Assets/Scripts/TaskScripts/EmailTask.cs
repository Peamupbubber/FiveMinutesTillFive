using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailTask : GenericTask
{
    PlayerPCScript[] playerPCs;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Send those emails";
    }

    private void OnLevelWasLoaded(int level)
    {
        playerPCs = FindObjectsByType<PlayerPCScript>(0);
        destObject = GameObject.FindGameObjectWithTag("PlayerPC");
        loaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!TaskComplete && loaded)
        {
            foreach(PlayerPCScript pc in playerPCs)
            {
                if (pc.hasSentEmail)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

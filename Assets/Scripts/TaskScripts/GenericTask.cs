using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericTask : MonoBehaviour
{
    public string TaskName;
    public bool TaskComplete;
    public GameObject destObject;
    private bool decremented = false;
    // Start is called before the first frame update
    void Start()
    {
        TaskComplete = false;
    }

    private void Update()
    {
        if (TaskComplete && !decremented)
        {
            GameManager.gameManager.incompleteTasks--;
            GameManager.gameManager.selfCompleteTasks++;
            decremented = true;
        }   
    }
}

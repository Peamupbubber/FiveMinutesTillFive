using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Task : MonoBehaviour
{
    GameObject taskComplete;
    [SerializeField] private TextMeshProUGUI taskText;
    public GenericTask myTask;
    bool setCompleted;

    void Start()
    {
        taskComplete = transform.Find("Task Complete").gameObject;
        taskText = transform.Find("Task Text").gameObject.GetComponent<TextMeshProUGUI>();
        taskComplete.SetActive(false);
        setCompleted = false;
    }

    private void Update()
    {
        if(!setCompleted && myTask.TaskComplete)
        {
            TaskComplete();
            setCompleted = true;
        }
    }

    public void TaskComplete() {
        taskComplete.SetActive(true);
    }

    public void SetText(string text) {
        taskText.text = text;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterTask : GenericTask
{
    PrinterScript[] printers;
    private bool loaded = false;

    private void Awake()
    {
        TaskName = "Copy Those Important Documents";   
    }

    private void OnLevelWasLoaded(int level)
    {
        printers = FindObjectsByType<PrinterScript>(0);
        destObject = GameObject.FindGameObjectWithTag("Printer");
        loaded = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!TaskComplete && loaded)
        {
            foreach(PrinterScript printer in printers)
            {
                if (printer.hasCopiedDoc)
                {
                    TaskComplete = true;
                    loaded = false;
                    break;
                }
            }
        }
    }
}

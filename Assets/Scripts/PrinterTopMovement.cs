using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterMovement : MonoBehaviour
{
    public Animator anim;
    public PrinterScript myParent;

    // Update is called once per frame
    void Update()
    {
        if (myParent.isPrinting)
        {
            anim.SetBool("IsPrinting", true);
        } else
        {
            anim.SetBool("IsPrinting", false);
        }
    }
}

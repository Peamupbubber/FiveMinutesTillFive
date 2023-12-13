using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Animator animator;

    private HashSet<Collider> colliders = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        animator.SetBool("Open", true);
        colliders.Add(other);
    }


    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);

        if(colliders.Count <= 0)
        {
            animator.SetBool("Open", false);
        }
    }
}

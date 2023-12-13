using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicleRoomCheck : MonoBehaviour
{
    private Tutorial tutorialScript;

    private void Start()
    {
        tutorialScript = GameObject.Find("Game Manager").GetComponent<Tutorial>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && tutorialScript.timeToCheckForEnteringRoom) {
            tutorialScript.timeToCheckForEnteringRoom = false;
            tutorialScript.enteredCubicleRoom = true;
        }
    }
}

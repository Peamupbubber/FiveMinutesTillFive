using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPCScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    public bool hasSentEmail = false;
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Interact") && playerControl.targetInteractable == gameObject)
        {
            hasSentEmail = true;
        }
    }
}

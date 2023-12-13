using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    public bool isDraggable = true;
    private bool isBeingDragged = false;
    // At some point make it so people can sit in these
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && playerControl.targetInteractable == gameObject)
        {
            if (!isBeingDragged && !playerControl.isDragging)
            {
                isBeingDragged = true;
                playerControl.isDragging = true;
                playerControl.draggingItem = gameObject;
            } else if(isBeingDragged)
            {
                isBeingDragged = false;
                playerControl.isDragging = false;
                playerControl.draggingItem = null;
            }
        }

        if(isBeingDragged && playerControl.targetInteractable != gameObject)
        {
            isBeingDragged = false;
            playerControl.isDragging = false;
            playerControl.draggingItem = null;
        }
    }
}

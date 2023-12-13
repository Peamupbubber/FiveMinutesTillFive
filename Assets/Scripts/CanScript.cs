using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanScript : MonoBehaviour
{
    GameObject thePlayer;
    MeshRenderer playerMesh;
    bool inCan;
    Vector3 playerPrevPos;
    PlayerLogic playerLogic;
    public bool receivedMarker = false;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerMesh = thePlayer.GetComponent<MeshRenderer>();
        playerLogic = thePlayer.GetComponent<PlayerLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (inCan)
            {
                thePlayer.transform.position = playerPrevPos;
                thePlayer.layer = LayerMask.NameToLayer("Default");
                Physics.IgnoreCollision(gameObject.GetComponent<CapsuleCollider>(), thePlayer.GetComponent<CapsuleCollider>(), false);
                playerLogic.speedMultiplier = 1;
                inCan = false;
                playerLogic.isHiding = false;
                playerLogic.hidingObject = null;
            }
            else if (playerLogic.targetInteractable == gameObject && GameManager.gameManager.trashCanPowerEnabled && (playerLogic.selectedItem == null || (playerLogic.selectedItem != null && !playerLogic.selectedItem.CompareTag("Marker"))))
            {
                if (!inCan)
                {
                    playerPrevPos = thePlayer.transform.position;
                    thePlayer.transform.position = transform.position;
                    thePlayer.layer = LayerMask.NameToLayer("Ignore Raycast");
                    Physics.IgnoreCollision(gameObject.GetComponent<CapsuleCollider>(), thePlayer.GetComponent<CapsuleCollider>(), true);
                    playerLogic.speedMultiplier = 0;
                    inCan = true;
                    playerLogic.isHiding = true;
                    playerLogic.hidingObject = gameObject;
                }
            }
            else if (playerLogic.targetInteractable == gameObject && playerLogic.selectedItem != null && playerLogic.selectedItem.CompareTag("Marker")) {
                receivedMarker = true;
                GameManager.gameManager.RemoveItem();
            }
        }
        // Make it so that players can't hide forever without consequences
        if (inCan)
        {
            GameManager.gameManager.managerAggression += .005f * Time.deltaTime;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    CupScript cupScript;
    public GameObject myJug;
    public GameObject myCup;
    bool hasCup = false;
    bool hasJug = false;
    public bool hasPoured = false;
    public Transform cupPosition;

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
            // Place jug
            if (playerControl.selectedItem != null && playerControl.selectedItem.CompareTag("Jug") && !hasJug)
            {
                playerControl.isCarrying = false;
                Destroy(playerControl.selectedItem);
                playerControl.RemoveCarryable();
                playerControl.newCarrying = false;
                myJug.SetActive(true);
                hasJug = true;
                playerControl.interactables.Remove(playerControl.selectedItem);
            // Place cup
            } else if(playerControl.selectedItem != null && playerControl.selectedItem.CompareTag("Cup") && !hasCup)
            {
                myCup = playerControl.selectedItem;
                cupScript = myCup.GetComponent<CupScript>();
                myCup.GetComponent<CarryableScript>().isCarriable = false;
                myCup.GetComponent<Rigidbody>().isKinematic = true;
                myCup.transform.localPosition = cupPosition.position;
                myCup.SetActive(true);
                cupScript.Normalize();
                playerControl.RemoveCarryable();
                hasCup = true;
                playerControl.interactables.Remove(playerControl.selectedItem);
            }
            // Fill cup
            else if(hasJug && hasCup && cupScript != null && !cupScript.isFilled)
            {
                cupScript.Fill("Water");
                playerControl.interactables.Add(myCup);
                hasPoured = true;
            // Player takes filled cup
            } else if (cupScript != null && cupScript.isFilled && !playerControl.isCarrying)
            {
                myCup.GetComponent<CarryableScript>().isCarriable = true;
                myCup.GetComponent<Rigidbody>().isKinematic = false;
                hasCup = false;
                myCup.GetComponent<CarryableScript>().Pickup();
                myCup = null;
                cupScript = null;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMachineScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    public GameObject cup;
    public CarryableScript carryableScript;
    public CupScript cupScript;
    public GameObject myScreen;
    public Transform cupPosition;
    public Material oldMaterial;
    public Material highlightedScreen;
    public bool hasCup = false;

    public bool hasDispensed = false;
    public bool dispensing = false;
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
        oldMaterial = myScreen.GetComponent<MeshRenderer>().material;
    }

    // This script is functional however picking up filled cup can be oddly difficult, revisit

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && playerControl.targetInteractable != null && playerControl.targetInteractable == gameObject)
        {
            // Place cup
            if(playerControl.selectedItem != null && playerControl.selectedItem.CompareTag("Cup") && cup == null)
            {
                cup = playerControl.selectedItem;
                cup.GetComponent<BoxCollider>().enabled = false;
                cup.SetActive(true);
                carryableScript = cup.GetComponent<CarryableScript>();
                cupScript = cup.GetComponent<CupScript>();
                carryableScript.isCarriable = false;
                cup.GetComponent<Rigidbody>().isKinematic = true;
                cup.transform.localPosition = cupPosition.position;
                cupScript.Normalize();
                hasCup = true;
                playerControl.RemoveCarryable();
                playerControl.interactables.Remove(playerControl.selectedItem);
            // Fill Cup
            } else if(hasCup && !cupScript.isFilled)
            {
                StartCoroutine(StartTimer());
                cupScript.Fill("Coffee");
                //playerControl.interactables.Add(cup);
            // Pickup Cup
            } else if(hasCup && cupScript.isFilled && !dispensing)
            {
                cup.GetComponent<BoxCollider>().enabled = true;
                carryableScript.isCarriable = true;
                cup.GetComponent<Rigidbody>().isKinematic = false;
                hasCup = false;
                carryableScript.Pickup();
                cup = null;
                carryableScript = null;
                cupScript = null;
            }
        }
    }

    IEnumerator StartTimer()
    {
        dispensing = true;
        myScreen.GetComponent<MeshRenderer>().material = highlightedScreen;
        yield return new WaitForSeconds(3);
        myScreen.GetComponent<MeshRenderer>().material = oldMaterial;
        hasDispensed = true;
        dispensing = false;
    }
}

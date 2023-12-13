using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodaMachineScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    public GameObject vendingDrink;
    public Transform sodaSpawn;

    public bool hasDispensed = false;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Interact") && playerControl.targetInteractable != null && playerControl.targetInteractable == gameObject)
        {
            if(playerControl.selectedItem != null && playerControl.selectedItem.CompareTag("Coin"))
            {
                Destroy(playerControl.selectedItem);
                playerControl.RemoveCarryable();

                Quaternion drinkRotation = new Quaternion(0, 0, 90, 1);
                GameObject dispensedDrink = Instantiate(vendingDrink, sodaSpawn.position, drinkRotation) as GameObject;
                hasDispensed = true;
            }
        }
    }
}

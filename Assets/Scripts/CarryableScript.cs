using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryableScript : MonoBehaviour
{ 
    public GameObject thePlayer;
    public PlayerLogic playerControl;
    public bool isCarriable = true;

    [SerializeField] private Sprite sprite;

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
        if (Input.GetButtonDown("Interact") && isCarriable && playerControl.targetInteractable == gameObject)
        {
            Pickup();
        }
    }
    public void Drop()
    {
        isCarriable = true;
        playerControl.RemoveCarryable();
    }

    public void Pickup()
    {
        int index = GameManager.gameManager.GetNextIndex();
        if (index != -1)
        {
            playerControl.AddCarryable(gameObject, sprite, index);
            playerControl.newCarrying = true;
            isCarriable = false;
            playerControl.interactables.Remove(gameObject);
            playerControl.targetInteractable = null;
            gameObject.SetActive(false);
        }
    }

    public void SetSprite(Sprite newSprite) {
        sprite = newSprite;
    }
}

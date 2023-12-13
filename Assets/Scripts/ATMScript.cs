using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATMScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    public GameObject coin;
    public GameObject myScreen;
    public Transform coinSpawn;
    public Material oldMaterial;
    public Material highlightedScreen;

    public bool hasDispensed = false;
    bool dispensing = false;
    // Start is called before the first frame update
    void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
        oldMaterial = myScreen.GetComponent<MeshRenderer>().material;

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Interact") && playerControl.targetInteractable != null && playerControl.targetInteractable == gameObject)
        {
            if(GameManager.gameManager.playerMoney > 0 && !dispensing)
            {
                GameManager.gameManager.UpdateMoney(-1);
                Quaternion coinRotation = new Quaternion(0, 0, 90, 1);
                GameObject dispensedCoin = Instantiate(coin, coinSpawn.position, coinRotation) as GameObject;
                hasDispensed = true;
                StartCoroutine(StartTimer());
            }
        }
    }

    IEnumerator StartTimer()
    {
        dispensing = true;
        myScreen.GetComponent<MeshRenderer>().material = highlightedScreen;
        yield return new WaitForSeconds(5);
        myScreen.GetComponent<MeshRenderer>().material = oldMaterial;
        dispensing = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    public bool isPrinting;
    private int resetTime = 10;
    public bool hasCopiedDoc = false;
    public GameObject copyDoc;
    public GameObject copiedDocLocation;
    public GameObject screen;
    private Material oldMat;
    public Material newMat;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
        oldMat = screen.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && playerControl.targetInteractable == gameObject)
        {
            if(!isPrinting){
                isPrinting = true;
                if (playerControl.selectedItem != null && playerControl.selectedItem.CompareTag("Document"))
                {
                    hasCopiedDoc = true;
                    copyDoc = playerControl.selectedItem;
                }
                StartCoroutine(StartTimer());
            }
        }
    }

    private void Reset()
    {
        isPrinting = false;
        if (copyDoc)
        {
            hasCopiedDoc = true;
            Quaternion docRotation = new Quaternion(0, 0, 90, 1);
            GameObject copiedDoc = Instantiate(copyDoc, copiedDocLocation.transform.position, docRotation) as GameObject;
            copiedDoc.SetActive(true);
            copiedDoc.GetComponent<CarryableScript>().isCarriable = true;
            copyDoc = null;
        }
        screen.GetComponent<MeshRenderer>().material = oldMat;
    }

    IEnumerator StartTimer()
    {
        screen.GetComponent<MeshRenderer>().material = newMat;
        yield return new WaitForSeconds(resetTime);
        Reset();
    }
}

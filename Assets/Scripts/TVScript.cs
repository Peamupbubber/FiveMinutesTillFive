using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVScript : MonoBehaviour
{
    GameObject thePlayer;
    PlayerLogic playerControl;
    private Material oldMat;
    public Material newMat;
    public GameObject screen;
    public GameObject location;
    public AudioSource audioSrc;
    public GameObject lightSource;
    bool isOn;
    GameObject manager;
    ManagerLogic managerScript;

    // Start is called before the first frame update
    void Start()
    {
        location = GameObject.Find("TVLocation");
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
        oldMat = screen.GetComponent<MeshRenderer>().material;
        isOn = false;
        lightSource.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && playerControl.targetInteractable == gameObject)
        {
            if (!isOn)
            {
                if(manager == null)
                {
                    GameObject[] managerArray = GameObject.FindGameObjectsWithTag("Manager");
                    manager = managerArray[Random.Range(0, managerArray.Length - 1)];
                    managerScript = manager.GetComponent<ManagerLogic>();
                    managerScript.Investigate(location.transform.position);
                }
                screen.GetComponent<MeshRenderer>().material = newMat;
                audioSrc.Play();
                isOn = true;
                lightSource.SetActive(true);
            } else
            {
                TurnOff();
                manager = null;
            }
        }
        if(manager != null)
        {
            managerScript.agent.destination = location.transform.position;
            if(isOn && Vector3.Distance(manager.transform.position, location.transform.position) <= 1f)
            {
                TurnOff();
                manager = null;
            }
        }
    }

    public void TurnOff()
    {
        lightSource.SetActive(false);
        screen.GetComponent<MeshRenderer>().material = oldMat;
        audioSrc.Stop();
        isOn = false;
    }
}

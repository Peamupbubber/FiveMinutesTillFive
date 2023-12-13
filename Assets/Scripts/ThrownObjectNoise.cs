using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThrownObjectNoise : MonoBehaviour
{
    private AudioSource source;
    public AudioClip clip;
    public GameObject thePlayer;
    public PlayerLogic playerControl;
    public ManagerLogic managerLogic;
    public bool beenThrown;

    private float noiseRange = 10f;
    private void Start()
    {
        thePlayer = PlayerLogic.playerObject;
        playerControl = thePlayer.GetComponent<PlayerLogic>();
        source = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (playerControl.hasThrown && beenThrown)
        {
            PlaySound();
            Collider[] colliders = Physics.OverlapSphere(transform.position, noiseRange);
            foreach (Collider c in colliders)
            {
                if (c.gameObject.CompareTag("Manager") || c.gameObject.CompareTag("CEO"))
                {
                    managerLogic = c.GetComponent<ManagerLogic>();
                    managerLogic.Investigate(collision.contacts[0].point);
                }
            }
        }
    }
    private void PlaySound()
    {
        source.PlayOneShot(clip);
        playerControl.hasThrown = false;
        beenThrown = false;
    }
}

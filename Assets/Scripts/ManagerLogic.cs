using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManagerLogic : MonoBehaviour
{
    public float visionDistance = 2f;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    private int index = 0;
    public int waitTime = 5;
    private bool waiting = false;
    private bool foundPlayer = false;
    public AudioClip discovered;
    public AudioClip[] caught;
    private AudioSource source;
    private bool saidDiscoveredLine;
    private bool saidDiscoveredActive;
    private bool saidCaughtLine;
    private bool saidCaughtActive;

    Animator anim;

    private float weight = 1;

    private LayerMask layerMask;

    private Coroutine waitRoutine;

    void Start()
    {
        layerMask = LayerMask.NameToLayer("Manager");
        agent = GetComponent<NavMeshAgent>();
        agent.destination = waypoints[0].position;
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Manager"))
        {
            Physics.IgnoreCollision(gameObject.GetComponent<CapsuleCollider>(), g.GetComponent<CapsuleCollider>(), false);
        }
        saidDiscoveredLine = false;
        saidDiscoveredActive = false;
        saidCaughtLine = false;
        saidCaughtActive = false;
    }

    void Update()
    {
        if (agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!waiting)
            {
                waitRoutine = StartCoroutine(WaitAtDestination());
            }
        }

        if (Physics.Raycast(transform.position, PlayerLogic.playerObject.transform.position - transform.position, out RaycastHit hitInfo, visionDistance, layerMask))
        {
            if (hitInfo.collider.gameObject == PlayerLogic.playerObject)
            {
                foundPlayer = true;
                agent.destination = PlayerLogic.playerObject.transform.position;
                anim.SetBool("Point", true);
                if (!saidDiscoveredLine)
                {
                    saidDiscoveredLine = true;
                    source.PlayOneShot(discovered);
                }
            }
            else
            {
                foundPlayer = false;
                anim.SetBool("Point", false);
                if (saidDiscoveredLine && !saidDiscoveredActive)
                {
                    saidDiscoveredActive = true;
                    StartCoroutine(AudioLineCoolDown(0));
                }
            }
        }
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerLogic.playerObject)
        {
            if (Physics.Raycast(transform.position, PlayerLogic.playerObject.transform.position - transform.position, out RaycastHit hitInfo, visionDistance, layerMask))
            {
                if (hitInfo.collider.gameObject == PlayerLogic.playerObject)
                {
                    if (!saidCaughtLine)
                    {
                        saidCaughtLine = true;
                        int index = Random.Range(0, 2);
                        source.PlayOneShot(caught[index]);
                    }
                    int unfinished = GameManager.gameManager.incompleteTasks;
                    int timeLeft = GameManager.gameManager.time;
                    weight = (unfinished * .4f) * (GameManager.gameManager.levelTime / (timeLeft + .01f)); // .01 is to avoid edgecase where this is calculated at time == 0
                    GameManager.gameManager.managerAggression += (.001f * weight);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == PlayerLogic.playerObject && saidCaughtLine && !saidCaughtActive)
        {
            saidCaughtActive = true;
            StartCoroutine(AudioLineCoolDown(1));
        }
    }

    private IEnumerator WaitAtDestination()
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        index = (index + 1) % waypoints.Length;
        agent.destination = waypoints[index].position;
        waiting = false;
    }

    // It seems like with the behavior of discovery/catching you can go in and out of being discovered and caught so this tries to cut back on the frequency of the audio clips being played
    // Val is a hacky fix because it's awkward to do pass by reference into IEnumerator so the bools couldn't be easily passed in
    // This is all to try and make the audio feel more natural but this code sucks and can get removed
    private IEnumerator AudioLineCoolDown(int val)
    {
        yield return new WaitForSeconds(3);
        if(val == 0)
        {
            saidDiscoveredLine = false;
            saidDiscoveredActive = false;
        } else
        {
            saidCaughtLine = false;
            saidCaughtActive = false;
        }
    }
    public void Investigate(Vector3 objectPosition)
    {
        agent.SetDestination(objectPosition);
        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waiting = false;
            waitRoutine = null;
        }
    }
}

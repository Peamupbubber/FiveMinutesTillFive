using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CEOLogic : ManagerLogic
{
    private int index = 0;
    private bool waiting = false;
    private bool attacking = false;
    private bool foundPlayer = false;

    private float attackChance = 1f;

    Animator anim;

    private float weight = 1;

    private LayerMask layerMask;

    private Coroutine currentRoutine;

    public GameObject target;
    public WorkerScript targetScript;

    private AudioSource source;
    private bool saidDiscoveredLine;
    private bool saidDiscoveredActive;
    private bool saidCaughtLine;
    private bool saidCaughtActive;

    void Start()
    {
        layerMask = LayerMask.NameToLayer("CEO");
        agent = GetComponent<NavMeshAgent>();
        agent.destination = waypoints[0].position;
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        agent.stoppingDistance = 1f;
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
            if (attacking && !waiting)
            {
                anim.SetBool("Point", true);
                if (!saidDiscoveredLine)
                {
                    saidDiscoveredLine = true;
                    source.PlayOneShot(discovered);
                }
                StartCoroutine(targetScript.Die());
                currentRoutine = StartCoroutine(WaitAtDestination());
            }
            if (!waiting && !attacking)
            {
                GetRandomEvent();
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
                if (!attacking)
                {
                    anim.SetBool("Point", false);
                    if (saidDiscoveredLine && !saidDiscoveredActive)
                    {
                        saidDiscoveredActive = true;
                        StartCoroutine(AudioLineCoolDown(0));
                    }
                }
            }
        }
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }
    private void GetRandomEvent()
    {
        target = GameObject.FindWithTag("Coworker");
        int rand = Random.Range(0, 100);
        if(target == null)
        {
            target = GameObject.FindWithTag("Player");
            agent.destination = target.transform.position;
        }
        else if (rand < 75)
        {
            currentRoutine = StartCoroutine(WaitAtDestination());
        }
        else
        {
            attacking = true;
            targetScript = target.GetComponent<WorkerScript>();
            targetScript.agent.destination = targetScript.transform.position;
            targetScript.isDying = true;
            agent.destination = target.transform.position;
        }
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
                    weight = (unfinished * .5f) * (GameManager.gameManager.levelTime / (timeLeft + .1f)); // .1 is to avoid edgecase where this is calculated at time == 0
                    GameManager.gameManager.managerAggression += (.001f * weight);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerLogic.playerObject && saidCaughtLine && !saidCaughtActive)
        {
            saidCaughtActive = true;
            StartCoroutine(AudioLineCoolDown(1));
        }
    }
    private IEnumerator WaitAtDestination()
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        anim.SetBool("Point", false);
        index = (index + 1) % waypoints.Length;
        agent.destination = waypoints[index].position;
        attacking = false;
        waiting = false;
    }

    private IEnumerator AudioLineCoolDown(int val)
    {
        yield return new WaitForSeconds(3);
        if (val == 0)
        {
            saidDiscoveredLine = false;
            saidDiscoveredActive = false;
        }
        else
        {
            saidCaughtLine = false;
            saidCaughtActive = false;
        }
    }
}

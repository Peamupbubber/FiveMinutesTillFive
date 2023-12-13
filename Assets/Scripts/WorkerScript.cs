using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class WorkerScript : MonoBehaviour
{
    public NavMeshAgent agent;

    Animator anim;

    public float maxDistance = 5f;

    private Vector3 originalPosition;

    private Rigidbody rb;

    private bool isWaiting;
    private bool seesPlayer;
    private bool inDialogue;
    private bool hasTask;
    public bool isDying = false;

    public bool receivedSoda = false;
    public bool receivedWater = false;

    private int priceBasedOnWillingnessToEngageinDuplicitousBehavior;

    PlayerLogic playerControl;

    private Coroutine waitRoutine;

    public GenericTask currentTask;
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        agent.destination = PickNewDestination(originalPosition, 5f);
        playerControl = PlayerLogic.playerObject.GetComponent<PlayerLogic>();
        priceBasedOnWillingnessToEngageinDuplicitousBehavior = Random.Range(4, 11);
    }

    void Update()
    {
        if (!isDying)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if (playerControl.targetInteractable == gameObject && playerControl.selectedItem != null && playerControl.selectedItem.CompareTag("Soda"))
                {
                    receivedSoda = true;
                    playerControl.RemoveCarryable();
                }
                else if (playerControl.targetInteractable == gameObject && playerControl.selectedItem != null && playerControl.selectedItem.CompareTag("Cup") && playerControl.selectedItem.GetComponent<CupScript>().isFilled)
                {
                    receivedWater = true;
                    playerControl.RemoveCarryable();
                }
            }
            if (agent.pathPending)
            {
                return;
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!isWaiting && !hasTask && !inDialogue)
                {
                    waitRoutine = StartCoroutine(Wait());
                }
                if (hasTask)
                {
                    StartCoroutine(CompleteTask());
                }
            }
            if (Input.GetButtonDown("Dialogue"))
            {
                if (seesPlayer)
                {
                    inDialogue = true;
                    CoworkerDialogue();
                }
            }

            anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        }
    }
    private Vector3 PickNewDestination(Vector3 center, float distance)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += center;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, distance, -1);
        return hit.position;
    }

    private IEnumerator Wait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(0, 30));
        agent.destination = PickNewDestination(originalPosition, maxDistance);
        isWaiting = false;
    }

    private IEnumerator CompleteTask()
    {
        hasTask = false;
        anim.SetBool("CompletingTask", true);
        yield return new WaitForSeconds(5);
        currentTask.TaskComplete = true;
        GameManager.gameManager.coworkerCompleteTasks++;
        anim.SetBool("CompletingTask", false);
        agent.destination = PickNewDestination(originalPosition, maxDistance);
    }

    private void OnTriggerStay(Collider other)
    {
        if(agent.velocity.sqrMagnitude < 0.1f)
        {
            if (other.gameObject == PlayerLogic.playerObject)
            {
                seesPlayer = true;
                if (inDialogue)
                {
                    transform.LookAt(PlayerLogic.playerObject.transform.position);
                    agent.destination = transform.position;
                    if(waitRoutine != null)
                    {
                        StopCoroutine(waitRoutine);
                        waitRoutine = null;
                    }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        inDialogue = false;
        seesPlayer = false;
    }
    private void CoworkerDialogue()
    {
        if(GameManager.gameManager.playerMoney >= priceBasedOnWillingnessToEngageinDuplicitousBehavior)
        {
            GameManager.gameManager.UpdateMoney(-priceBasedOnWillingnessToEngageinDuplicitousBehavior);
            foreach(GenericTask task in GameManager.gameManager.curTasks)
            {
                if (!task.TaskComplete)
                {
                    currentTask = task;
                    agent.destination = task.destObject.transform.position;
                }
            }
            hasTask = true;
        }
        inDialogue = false;
    }
    public IEnumerator Die()
    {
        isDying = true;
        anim.SetBool("IsDead", true);
        yield return new WaitForSeconds(3);
        StopAllCoroutines();
        Destroy(gameObject);
    }
}

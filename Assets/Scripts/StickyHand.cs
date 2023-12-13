using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyHand : MonoBehaviour
{
    public GameObject stickyHandObj;
    public MeshRenderer stickyhandrender;
    public LineRenderer handLine;

    public Transform head;
    public Transform hand;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip pullSound;

    private LayerMask interactable;
    

    private const float RANGE = 10;
    private float handspeed = 50f;

    private AudioSource stickySource;

    private bool handout = false;
    // Update is called once per frame

    private void Start()
    {
        stickyHandObj = Instantiate(stickyHandObj, transform.position, Quaternion.identity);
        stickyhandrender = stickyHandObj.GetComponent<MeshRenderer>();
        stickySource = stickyHandObj.GetComponent<AudioSource>();

        handLine = Instantiate(handLine, transform.position, Quaternion.identity).GetComponent<LineRenderer>();

        stickyhandrender.enabled = false;
        handLine.enabled = false;

        interactable = LayerMask.NameToLayer("Interactable");


    }
    void Update()
    {
        if (!GameManager.gameManager.stickyHandPowerEnabled || handout)
        {
            return;
        }

        if (Input.GetButtonDown("StickyHand"))
        {
            ThrowHands();
        }
        
    }


    void ThrowHands()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, RANGE, ~interactable);

        //RaycastHit[] hits = Physics.SphereCastAll(head.position, RADIUS, transform.forward, RANGE);
        float maxDiff = float.PositiveInfinity;
        CarryableScript target = null;

        // Get all interactables in range
        foreach (Collider g in hits)
        {
            // Check if it is Line of Sight
            if (Physics.Raycast(head.position, g.bounds.center - head.position, out RaycastHit hitInfo, RANGE))
            {
                if (hitInfo.collider != g)
                {
                    continue;
                }
            }
            else
            {
                continue;
            }

            // Check if it is carryable
            if (g.TryGetComponent(out CarryableScript c))
            {
                // Pick the object closest to the center of the player's forward
                float diff = Vector3.Angle(transform.forward, g.transform.position - transform.position);
                if (diff < maxDiff && diff < 35)
                {
                    target = c;
                    maxDiff = diff;
                }
            }
        }
        
        // Now get the item

        if (target == null)
        {
            return;
        }

        StartCoroutine(SnatchObject(target));
    }



    private IEnumerator SnatchObject(CarryableScript target)
    {
        handout = true;
        stickyHandObj.transform.position = hand.transform.position;
        stickyhandrender.enabled = true;
        handLine.enabled = true;
        Vector3[] vertices = new Vector3[] { hand.position, stickyHandObj.transform.position };

        stickyHandObj.transform.rotation = transform.rotation;

        stickySource.PlayOneShot(throwSound);

        // Make hand go to object
        while (stickyHandObj.transform.position != target.transform.position)
        {
            stickyHandObj.transform.position = Vector3.MoveTowards(stickyHandObj.transform.position, target.transform.position, handspeed * Time.deltaTime);
            vertices[0] = hand.position;
            vertices[1] = stickyHandObj.transform.position;
            handLine.SetPositions(vertices);
            yield return null;
        }

        float time = 0;

        stickySource.PlayOneShot(hitSound);
        // freeze hand at location
        while (time < .4f)
        {
            time += Time.deltaTime;
            stickyHandObj.transform.position = target.transform.position;
            vertices[0] = hand.position;
            vertices[1] = stickyHandObj.transform.position;
            handLine.SetPositions(vertices);
            yield return null;
        }


        // Make hand go back to player

        while (Vector3.Distance(stickyHandObj.transform.position,transform.position) > .1f)
        {
            stickyHandObj.transform.position = Vector3.MoveTowards(stickyHandObj.transform.position, transform.position, handspeed * Time.deltaTime);
            target.transform.position = stickyHandObj.transform.position;   
            vertices[0] = hand.position;
            vertices[1] = stickyHandObj.transform.position;
            handLine.SetPositions(vertices);
            yield return null;
        }
        stickySource.PlayOneShot(pullSound);

        // Pick up the object
        target.Pickup();
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;

        stickyhandrender.enabled = false;
        handLine.enabled = false;
        handout = false;
    }
}

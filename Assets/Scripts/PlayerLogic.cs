using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public static GameObject playerObject;
    public Transform head;
    public Transform hand;
    public float speed = 5;
    public float speedMultiplier = 1;
    public float crouchMult = 0.3f;
    public float sensitivity;

    private Rigidbody player;
    private Vector2 rotation = Vector2.zero;
    private const float MOVEMENT_DIVISION = 1.35f;

    public float camYOffset = 1.7f;
    public float camDistance = 5.4f;

    public bool isDragging = false;
    public bool isCarrying = false;
    public bool newCarrying = false;

    public Material preview;
    private Material matSave;
    bool previewActive = false;

    public GameObject cam;
    public GameObject draggingItem;
    public GameObject[] carryingItems;
    public GameObject selectedItem;

    public HashSet<GameObject> interactables = new HashSet<GameObject>();
    public GameObject targetInteractable;
    public bool isHiding = false;
    public GameObject hidingObject;

    public bool hasThrown = false;

    private void Awake()
    {
        playerObject = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
        GameManager.gameManager.SetCurrentLevelAudio(transform.Find("Camera").GetComponent<AudioSource>());
        carryingItems = new GameObject[GameManager.gameManager.GetMaxItems()];
        sensitivity = 75;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameManager.deathScreenActive) {
            cam.transform.localPosition = new Vector3(2.6f, 5.94f, 11.04f);
            cam.transform.LookAt(hand.transform);
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponent<SphereCollider>());
            Destroy(this);
            return;
        }
        rotation.y += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        transform.eulerAngles = rotation;
        selectedItem = carryingItems[GameManager.gameManager.GetSelectedIndex()];
        
        SetCamera();
        CheckDrop();

        FindTargetInteractable();
    }

    private void SetCamera() {
        if (Physics.Raycast(transform.position + Vector3.up * camYOffset, -transform.forward, out RaycastHit hitInfo, camDistance))
        {
            cam.transform.position = hitInfo.point;
        }
        else
        {
            cam.transform.position = transform.position + Vector3.up * camYOffset + (-transform.forward * camDistance);
        }
    }
    
    private void Throw()
    {
        this.hasThrown = true;
        if(selectedItem == null)
        {
            return;
        }
        selectedItem.SetActive(true);
        selectedItem.GetComponent<ThrownObjectNoise>().beenThrown = true;
        selectedItem.transform.position = hand.position + (transform.forward * 1.5f);
        // Issue here is that it needs to go in the same direction player is looking
        Vector3 throwVect = player.transform.forward;
        throwVect.y = 700;
        throwVect.x *= 500;
        throwVect.z *= 500;
        selectedItem.GetComponent<Rigidbody>().AddForce(throwVect);
        selectedItem.GetComponent<CarryableScript>().Drop();
        newCarrying = true;
    }

    private void CheckDrop() {
        if (selectedItem != null && Input.GetButton("Drop"))
        {
            selectedItem.SetActive(true);
            MeshRenderer renderer = selectedItem.GetComponent<MeshRenderer>();
            selectedItem.GetComponent<Rigidbody>().isKinematic = true;
            selectedItem.transform.position = transform.position + (transform.forward * 2);
            // Show the object
            if (!previewActive)
            {
                matSave = renderer.material;
            }
            previewActive = true;
            renderer.material = preview;
        }
        if (selectedItem != null && (Input.GetButtonUp("Drop")) && previewActive)
        {
            selectedItem.GetComponent<Rigidbody>().isKinematic = false;
            selectedItem.GetComponent<MeshRenderer>().material = matSave;
            selectedItem.transform.position = transform.position + (transform.forward * 2);
            selectedItem.GetComponent<CarryableScript>().Drop();
            previewActive = false;
            newCarrying = true;
        }
    }

    // Calculate target interactable
    private void FindTargetInteractable() {
        if (interactables.Count > 0)
        {
            float maxDiff = float.PositiveInfinity;
            if (!isHiding)
            {
                foreach (GameObject g in interactables)
                {
                    // Make angle from forward and distance from player effect priority
                    float diff = Vector3.Angle(transform.forward, g.transform.position - transform.position) + (Vector3.Distance(g.transform.position, transform.position) * 90);
                    if (diff < maxDiff)
                    {
                        targetInteractable = g;
                        maxDiff = diff;
                    }
                }
            }
            else
            {
                targetInteractable = hidingObject;
            }
        }
    }

    public void AddCarryable(GameObject obj, Sprite sprite, int index) {
        selectedItem = obj;
        carryingItems[index] = obj;
        GameManager.gameManager.AddItem(sprite, index);
    }

    public void RemoveCarryable() {
        selectedItem = null;
        carryingItems[GameManager.gameManager.GetSelectedIndex()] = null;
        GameManager.gameManager.RemoveItem();
    }


    private void FixedUpdate()
    {
        //player.velocity = Vector3.zero;

        Vector3 move = Vector3.ClampMagnitude(transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical"), 1) * speed * speedMultiplier;
        /*
        if (Input.GetButton("Crouch"))
        {
            // change the scale to be an animation later.
            transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            move *= crouchMult;
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        }
        */
        player.velocity = move;
        if(isDragging)
        {
            draggingItem.GetComponent<Rigidbody>().velocity = (move * 1.19f);
        }

    }

    // Add to list
    private void OnTriggerEnter(Collider other)
    {
        interactables.Add(other.gameObject);
    }

    // Highlight items
    private void OnTriggerStay(Collider other)
    {
        Debug.DrawRay(head.position, (other.bounds.center - head.position) * 2.1f);
        bool haveLOS = false;
        if (Physics.Raycast(head.position, other.bounds.center - head.position, out RaycastHit hitInfo, 2.1f))
        {
            if (hitInfo.collider.gameObject == other.gameObject)
            {
                haveLOS = true;
            }
        }

        if (haveLOS && !interactables.Contains(other.gameObject))
        {
            interactables.Add(other.gameObject);
        } else if((!haveLOS && interactables.Contains(other.gameObject)) || (isHiding && other.gameObject != hidingObject))
        {
            interactables.Remove(other.gameObject);
            if (targetInteractable != null && targetInteractable == other.gameObject)
            {
                targetInteractable = null;
            }
        }
    }

    // Remove from list
    private void OnTriggerExit(Collider other)
    {
        interactables.Remove(other.gameObject);
        if(targetInteractable !=  null && targetInteractable == other.gameObject)
        {
            targetInteractable = null;
        }
    }
}

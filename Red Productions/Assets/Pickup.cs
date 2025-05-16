using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private TomatoLauncher weapon;

    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float hitRange = 3;
    [SerializeField] private Transform pickupParent;
    [SerializeField] private float minDistance = 1f;  
    [SerializeField] private float maxDistance = 3f;  
    [SerializeField] private float scrollSensitivity = 1f; 
    private RaycastHit hit;
    private float currentDistance;
    [SerializeField] GameObject inHandItem;

    void Start()
    {
        currentDistance = Vector3.Distance(pickupParent.position, playerCameraTransform.position);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }


    void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);
        if (hit.collider != null)
        {
            hit.collider.GetComponent<HighLight>()?.ToggleHighLight(false);
        }
        if (inHandItem != null)
        {
            inHandItem.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            return;
        }

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
        {
            hit.collider.GetComponent<HighLight>()?.ToggleHighLight(true);
        }
    }

    public void AdjustDistance(float scrollDelta)
    {
        currentDistance -= scrollDelta * scrollSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Calculate the new position for the pickupParent
        Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
        pickupParent.position = newPosition;
    }

    public void PickuP()
    {
        Debug.Log("Picked up");
        if (hit.collider != null)
        {
            Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();

            Debug.Log(hit.collider.name);
            if (hit.collider.GetComponent<Ingredient>() || hit.collider.GetComponent<Food>())
            {
                Debug.Log("Holding Ingredient");
                inHandItem = hit.collider.gameObject;
                inHandItem.transform.SetParent(pickupParent.transform, true);
                inHandItem.transform.localPosition = Vector3.zero; 
                inHandItem.transform.localRotation = Quaternion.identity; 
                if (rb != null)
                {
                    rb.isKinematic = true;
                    Debug.Log(rb.isKinematic);
                }
                return;
            }
            if (hit.collider.GetComponent<TomatoLauncher>() != null)
            {
                // Handle TomatoLauncher pickup
            }
        }
    }

    public void Drop()
    {
        Debug.Log("Dropped down");
        if (inHandItem != null)
        {
            Rigidbody rb = inHandItem.GetComponent<Rigidbody>(); 
            inHandItem.transform.SetParent(null);
            inHandItem = null;

            if (rb != null)
            {
                rb.isKinematic = false;
                Debug.Log(rb.isKinematic);
            }
        }
    }

    public void Interact()
    {
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
        }
    }
}
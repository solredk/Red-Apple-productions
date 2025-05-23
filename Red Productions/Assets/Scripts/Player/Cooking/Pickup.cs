using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private TomatoLauncher weapon;

    [SerializeField] private Transform pickupParent;
    [SerializeField] private Transform playerCameraTransform;

    [SerializeField] private GameObject tomatoWeapon;
    [SerializeField] private GameObject inHandItem;

    [SerializeField] private LayerMask pickableLayerMask;

    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float scrollSensitivity = 1f;
    
    [SerializeField] private float hitRange = 3;

    [SerializeField] private float minDistance = 1f;  
    [SerializeField] private float maxDistance = 3f;

    private RaycastHit hit; 

    private float currentDistance;

    void Start()
    {
        currentDistance = Vector3.Distance(pickupParent.position, playerCameraTransform.position);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        tomatoWeapon.SetActive(true);
    }

    void Update()
    {
        Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * hitRange, Color.red);

        if (hit.collider != null)
            hit.collider.GetComponent<HighLight>()?.ToggleHighLight(false);

        if (inHandItem != null)
        {
            inHandItem.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            return;
        }

        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
            hit.collider.GetComponent<HighLight>()?.ToggleHighLight(true);
    }

    public void AdjustDistance(float scrollDelta)
    {
        currentDistance -= scrollDelta * scrollSensitivity;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            
        Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
        pickupParent.position = newPosition;
    }

    public void PickuP()
    {
        Debug.Log("Picked up");
        if (hit.collider != null)
        {
            Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();

            if (hit.collider.GetComponent<Ingredient>() || hit.collider.GetComponent<Food>())
            {
                inHandItem = hit.collider.gameObject;
                inHandItem.transform.SetParent(pickupParent.transform, true);
                inHandItem.transform.localPosition = Vector3.zero; 
                inHandItem.transform.localRotation = Quaternion.identity; 
               
                if (rb != null)
                    rb.isKinematic = true;

            }
            tomatoWeapon.SetActive(false);
            // animation 
            return;
        }
    }

    public void Drop()
    {
        if (inHandItem != null)
        {
            Rigidbody rb = inHandItem.GetComponent<Rigidbody>();
            inHandItem.transform.SetParent(null);
            inHandItem = null;

            if (rb != null)
                rb.isKinematic = false;

            inHandItem = null;
            tomatoWeapon.SetActive(true);
        }
    }

    public void Interact()
    {
        if (hit.collider != null)
            Debug.Log(hit.collider.name);
    }
}

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
        {
            hit.collider.GetComponent<HighLight>()?.ToggleHighLight(false);
        }

        if (inHandItem != null)
        {
            inHandItem.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            return;
        }

        // Use Physics.Raycast with all layers
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange))
        {
            // Only highlight if it has Ingredient or Food component
            if (hit.collider.GetComponent<Ingredient>() != null ||
                hit.collider.GetComponent<Food>() != null)
            {
                hit.collider.GetComponent<HighLight>()?.ToggleHighLight(true);
            }
        }
    }

    public void AdjustDistance(float scrollDelta)
    {
        // Calculate potential new distance before applying it
        float potentialDistance = currentDistance - scrollDelta * scrollSensitivity;

        // Check if the new position would be valid
        // Only enforce minimum distance when scrolling inward (positive scrollDelta)
        if (scrollDelta <= 0 || potentialDistance >= minDistance)
        {
            // Apply the change
            currentDistance = potentialDistance;

            // Ensure we stay within allowed distance range
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // Update position based on new distance
            Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
            pickupParent.position = newPosition;
        }
    }

    public void PickuP()
    {
        if (hit.collider != null)
        {
            // Only pick up objects with Ingredient or Food component
            Ingredient ingredient = hit.collider.GetComponent<Ingredient>();
            Food food = hit.collider.GetComponent<Food>();

            if (ingredient != null || food != null)
            {
                Rigidbody rb = hit.collider.gameObject.GetComponent<Rigidbody>();

                inHandItem = hit.collider.gameObject;
                inHandItem.transform.SetParent(pickupParent.transform, true);
                inHandItem.transform.localPosition = Vector3.zero;
                inHandItem.transform.localRotation = Quaternion.identity;

                if (rb != null)
                    rb.isKinematic = true;

                tomatoWeapon.SetActive(false);
            }
        }
    }

    public void Drop()
    {
        if (inHandItem != null)
        {
            Rigidbody rb = inHandItem.GetComponent<Rigidbody>();
            inHandItem.transform.SetParent(null);

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
        {
            // First check if what we're looking at directly has CookingStation
            CookingStation directStation = hit.collider.GetComponent<CookingStation>();
            if (directStation != null)
            {
                directStation.Interact();
                return;
            }

            // If not, check for nearby CookingStations
            float checkRadius = 3f;
            Collider[] nearbyColliders = Physics.OverlapSphere(hit.point, checkRadius);

            foreach (Collider col in nearbyColliders)
            {
                CookingStation station = col.GetComponent<CookingStation>();
                if (station != null)
                {
                    station.Interact();
                    return;
                }
            }
        }
    }
}
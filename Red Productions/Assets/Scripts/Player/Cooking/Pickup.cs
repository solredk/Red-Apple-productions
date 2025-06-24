using UnityEngine;

public class Pickup : MonoBehaviour
{

    [SerializeField] public Transform pickupSlot;
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

    // Item follow parameters
    [SerializeField] private float followSpeed = 12f;
    [SerializeField] private float rotationLerpSpeed = 8f;
    [SerializeField] private bool isHolding = false;

    private RaycastHit hit;

    private float currentDistance;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

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

        // Handle held item
        if (inHandItem != null)
        {
            if (!isHolding)
            {
                // Regular rotation for when not in "held" mode
                inHandItem.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                UpdatePickupParentPosition();
                // Active holding behavior - make item follow the pickup slot
                UpdateHeldItemPosition();

            }
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

    private void UpdatePickupParentPosition()
    {
        // Position the pickup parent along the player's forward direction
        Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
        pickupParent.position = Vector3.Lerp(pickupParent.position, newPosition, followSpeed * Time.deltaTime);

        // Make pickup parent rotation match the camera rotation exactly
        pickupParent.rotation = Quaternion.Lerp(pickupParent.rotation, playerCameraTransform.rotation, rotationLerpSpeed * Time.deltaTime);

        // Optional: Ensure the pickupSlot is properly aligned
        // Remove this if pickupSlot is already correctly positioned in the parent's space
        pickupSlot.rotation = playerCameraTransform.rotation;
    }
    private void UpdateHeldItemPosition()
    {
        if (inHandItem == null) return;

        // Get the exact position along the ray where the item should be
        Vector3 rayPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;

        // Use stronger vertical correction to prevent falling appearance
        Vector3 currentPos = inHandItem.transform.position;
        float horizontalLerp = followSpeed * Time.deltaTime;
        float verticalLerp = followSpeed * 2f * Time.deltaTime; // Stronger Y correction

        Vector3 newPos = new Vector3(
            Mathf.Lerp(currentPos.x, pickupSlot.position.x, horizontalLerp),
            Mathf.Lerp(currentPos.y, pickupSlot.position.y, verticalLerp),
            Mathf.Lerp(currentPos.z, pickupSlot.position.z, horizontalLerp)
        );

        // Apply position and rotation
        inHandItem.transform.position = newPos;
        inHandItem.transform.rotation = Quaternion.Slerp(
            inHandItem.transform.rotation,
            playerCameraTransform.rotation,
            rotationLerpSpeed * Time.deltaTime);
    }

    public void ToggleHoldMode()
    {
        // Toggle hold mode when E is pressed
        if (inHandItem != null)
        {
            isHolding = !isHolding;

            Rigidbody rb = inHandItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (isHolding)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                else
                {
                    rb.isKinematic = true;  // Still kinematic but not in hold mode
                }
            }
        }
    }

    public void AdjustDistance(float scrollDelta)
    {
        // Calculate potential new distance before applying it
        float potentialDistance = currentDistance - scrollDelta * scrollSensitivity;

        if (scrollDelta <= 0 || potentialDistance >= minDistance)
        {
            // Apply the change
            currentDistance = potentialDistance;

            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);


            Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
            pickupParent.position = newPosition;
        }
    }

    public void PickuP()
    {
        // Don't pick up if already holding something
        if (inHandItem != null)
            return;

        if (hit.collider != null)
        {
            // Only pick up objects with Ingredient or Food component
            Ingredient ingredient = hit.collider.GetComponent<Ingredient>();
            Food food = hit.collider.GetComponent<Food>();

            if (ingredient != null || food != null)
            {
                inHandItem = hit.collider.gameObject;

                // Initially parent to the pickup slot
                inHandItem.transform.SetParent(pickupSlot.transform, true);
                inHandItem.transform.localPosition = Vector3.zero;
                inHandItem.transform.localRotation = Quaternion.identity;

                // Set up physics components
                RigidbodySetup();

                // Automatically start in held mode when picking up
                isHolding = true;

                tomatoWeapon.SetActive(false);
            }
        }
    }

    public void Drop()
    {
        // Don't try to drop if nothing is held
        if (inHandItem == null)
            return;

        Rigidbody rb = inHandItem.GetComponent<Rigidbody>();

        inHandItem.transform.SetParent(null);
        isHolding = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

        }

        inHandItem = null;
        tomatoWeapon.SetActive(true);
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

    private void RigidbodySetup()
    {
        if (inHandItem == null) return;

        Rigidbody rb = inHandItem.GetComponent<Rigidbody>();
        if (rb == null)
            rb = inHandItem.AddComponent<Rigidbody>();

        rb.mass = 25f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.isKinematic = true;
        rb.useGravity = false;
    }
}
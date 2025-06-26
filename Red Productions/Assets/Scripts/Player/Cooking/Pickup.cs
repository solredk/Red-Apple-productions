using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] public Transform pickupSlot;
    [SerializeField] private Transform pickupParent;
    [SerializeField] private Transform playerCameraTransform;

    [SerializeField] private GameObject tomatoWeapon;
    [SerializeField] private GameObject inHandItem;

    [SerializeField] private LayerMask pickableLayerMask;
    [SerializeField] private LayerMask detectableLayerMask;

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
                UpdateHeldItemPosition();
            }
            return;
        }

        // First check for ingredients and food with pickableLayerMask
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
        {
            // Only highlight if it has Ingredient or Food component
            if (hit.collider.GetComponent<Ingredient>() != null ||
                hit.collider.GetComponent<Food>() != null)
            {
                hit.collider.GetComponent<HighLight>()?.ToggleHighLight(true);
            }
        }

        // Check for interactive objects
        RaycastHit interactionHit;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out interactionHit, hitRange, detectableLayerMask))
        {
            // Check for IngredientSpawner first
            IngredientSpawner spawner = interactionHit.collider.GetComponent<IngredientSpawner>();
            if (spawner != null)
            {
                spawner.ShowInteractionIndicator(true);
            }
            else
            {
                // Check for CookingStation directly
                CookingStation station = interactionHit.collider.GetComponent<CookingStation>();
                if (station != null)
                {
                    station.ShowInteractionIndicator(true);
                }
                else
                {
                    // Check nearby objects
                    Collider[] nearbyColliders = Physics.OverlapSphere(interactionHit.point, 3f);
                    foreach (Collider col in nearbyColliders)
                    {
                        // Check for nearby CookingStation
                        CookingStation nearbyStation = col.GetComponent<CookingStation>();
                        if (nearbyStation != null)
                        {
                            nearbyStation.ShowInteractionIndicator(true);
                            break;
                        }

                        // Check for nearby IngredientSpawner if no station found
                        if (nearbyStation == null)
                        {
                            IngredientSpawner nearbySpawner = col.GetComponent<IngredientSpawner>();
                            if (nearbySpawner != null)
                            {
                                nearbySpawner.ShowInteractionIndicator(true);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdatePickupParentPosition()
    {
        // Position the pickup parent along the player's forward direction
        Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
        pickupParent.position = Vector3.Lerp(pickupParent.position, newPosition, followSpeed * Time.deltaTime);


        pickupParent.rotation = Quaternion.Lerp(pickupParent.rotation, playerCameraTransform.rotation, rotationLerpSpeed * Time.deltaTime);
    }

    private void UpdateHeldItemPosition()
    {
        if (inHandItem == null) return;

        // Get the exact position along the ray where the item should be
        Vector3 rayPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;

        Vector3 currentPos = inHandItem.transform.position;
        float horizontalLerp = followSpeed * Time.deltaTime;
        float verticalLerp = followSpeed * 2f * Time.deltaTime; 

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

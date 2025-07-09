using UnityEngine;
public class Pickup : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] private GameObject regularCrosshair;
    [SerializeField] private bool isTargetingEnemy = false;
    [SerializeField] public Transform pickupSlot;
    [SerializeField] private Transform pickupParent;
    [SerializeField] private Transform playerCameraTransform;

    [SerializeField] public GameObject tomatoWeapon;
    [SerializeField] public GameObject inHandItem;

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
    [SerializeField] public bool isHolding = false;

    private RaycastHit hit;

    private float currentDistance;

    void Start()
    {
        currentDistance = Vector3.Distance(pickupParent.position, playerCameraTransform.position);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        tomatoWeapon.SetActive(true);

        if (regularCrosshair != null)
        {
            regularCrosshair.SetActive(true);
        }
    }

    void Update()
    {
        isTargetingEnemy = false;
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
                inHandItem.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            }
            else
            {
                UpdatePickupParentPosition();
                UpdateHeldItemPosition();
            }
            return;
        }

        // Raycast for pickable objects and enemies
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pickableLayerMask))
        {
            // Check for Enemy tag first
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Detected");
                isTargetingEnemy = true;
            }
            // Then check for Ingredient or Food
            else if (hit.collider.GetComponent<Ingredient>() != null ||
                     hit.collider.GetComponent<Food>() != null)
            {
                hit.collider.GetComponent<HighLight>()?.ToggleHighLight(true);
            }
        }

        // Check for interactive objects
        RaycastHit interactionHit;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out interactionHit, hitRange, detectableLayerMask))
        {
            IngredientSpawner spawner = interactionHit.collider.GetComponent<IngredientSpawner>();
            if (spawner != null)
            {
                spawner.ShowInteractionIndicator(true);
            }
            else
            {
                CookingStation station = interactionHit.collider.GetComponent<CookingStation>();
                if (station != null)
                {
                    station.ShowInteractionIndicator(true);
                }
                else
                {
                    Collider[] nearbyColliders = Physics.OverlapSphere(interactionHit.point, 3f);
                    foreach (Collider col in nearbyColliders)
                    {
                        CookingStation nearbyStation = col.GetComponent<CookingStation>();
                        if (nearbyStation != null)
                        {
                            nearbyStation.ShowInteractionIndicator(true);
                            break;
                        }
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
        Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
        pickupParent.position = Vector3.Lerp(pickupParent.position, newPosition, followSpeed * Time.deltaTime);

        pickupParent.rotation = Quaternion.Lerp(pickupParent.rotation, playerCameraTransform.rotation, rotationLerpSpeed * Time.deltaTime);
    }

    private void UpdateHeldItemPosition()
    {
        if (inHandItem == null) return;

        Vector3 rayPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;

        Vector3 currentPos = inHandItem.transform.position;
        float horizontalLerp = followSpeed * Time.deltaTime;
        float verticalLerp = followSpeed * 2f * Time.deltaTime;

        Vector3 newPos = new Vector3(
            Mathf.Lerp(currentPos.x, pickupSlot.position.x, horizontalLerp),
            Mathf.Lerp(currentPos.y, pickupSlot.position.y, verticalLerp),
            Mathf.Lerp(currentPos.z, pickupSlot.position.z, horizontalLerp)
        );

        inHandItem.transform.position = newPos;
        inHandItem.transform.rotation = Quaternion.Slerp(
            inHandItem.transform.rotation,
            playerCameraTransform.rotation,
            rotationLerpSpeed * Time.deltaTime);
    }

    public void ToggleHoldMode()
    {
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
                    rb.isKinematic = true;
                }
            }
        }
    }

    public void AdjustDistance(float scrollDelta)
    {
        float potentialDistance = currentDistance - scrollDelta * scrollSensitivity;

        if (scrollDelta <= 0 || potentialDistance >= minDistance)
        {
            currentDistance = potentialDistance;

            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            Vector3 newPosition = playerCameraTransform.position + playerCameraTransform.forward * currentDistance;
            pickupParent.position = newPosition;
        }
    }

    public void PickuP()
    {
        if (inHandItem != null)
            return;

        if (hit.collider != null)
        {
            Ingredient ingredient = hit.collider.GetComponent<Ingredient>();
            Food food = hit.collider.GetComponent<Food>();

            if (ingredient != null || food != null)
            {
                inHandItem = hit.collider.gameObject;

                inHandItem.transform.SetParent(pickupSlot.transform, true);
                inHandItem.transform.localPosition = Vector3.zero;
                inHandItem.transform.localRotation = Quaternion.identity;

                RigidbodySetup();

                isHolding = true;

                tomatoWeapon.SetActive(false);
            }
        }
    }

    public void Drop()
    {
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
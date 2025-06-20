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
                inHandItem.transform.SetParent(pickupParent.transform, true);
                inHandItem.transform.localPosition = Vector3.zero;
                inHandItem.transform.localRotation = Quaternion.identity;

                // Set up physics components for better handling
                RigidbodySetup();
                ConfigJSetup();

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
        ConfigurableJoint joint = inHandItem.GetComponent<ConfigurableJoint>();

        // Clean up the ConfigurableJoint if it exists
        if (joint != null)
            Destroy(joint);

        inHandItem.transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.angularDamping = 10f;        // Reset to default values
            rb.linearDamping = 10f; // Reset to default values
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


        rb.mass = 0.5f;
        rb.linearDamping = 25f;              // Linear damping
        rb.angularDamping = 25f;       // Angular damping
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.isKinematic = false;    
        rb.useGravity = true;    
    }

    private void ConfigJSetup()
    {
        if (inHandItem == null) return;

        // Remove any existing ConfigurableJoint
        ConfigurableJoint existingJoint = inHandItem.GetComponent<ConfigurableJoint>();
        if (existingJoint != null)
            Destroy(existingJoint);

        // Add new ConfigurableJoint
        ConfigurableJoint joint = inHandItem.AddComponent<ConfigurableJoint>();

        // Make sure the pickup parent has a rigidbody
        Rigidbody parentRb = pickupParent.GetComponent<Rigidbody>();
        if (parentRb == null)
        {
            parentRb = pickupParent.gameObject.AddComponent<Rigidbody>();
            parentRb.isKinematic = true;
            parentRb.useGravity = false;
        }

        // Connect to pickup parent
        joint.connectedBody = parentRb;

        // Configure joint with exact specifications
        joint.autoConfigureConnectedAnchor = true;

        // Lock all linear motion
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        // Limited angular motion
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;

        // Configure X-drive
        JointDrive xDrive = joint.xDrive;
        xDrive.positionSpring = 1000f;
        xDrive.positionDamper = 500f;
        xDrive.maximumForce = 2000f;
        joint.xDrive = xDrive;

        // Configure Y-drive
        JointDrive yDrive = joint.yDrive;
        yDrive.positionSpring = 1000f;
        yDrive.positionDamper = 500f;
        yDrive.maximumForce = 0f;
        joint.yDrive = yDrive;

        // Configure Z-drive
        JointDrive zDrive = joint.zDrive;
        zDrive.positionSpring = 1000f;
        zDrive.positionDamper = 500f;
        zDrive.maximumForce = 2000f;
        joint.zDrive = zDrive;

        // Configure X-limit - Fix for the spring property
        SoftJointLimit angularXLimit = joint.highAngularXLimit;
        angularXLimit.limit = 1000;
        joint.highAngularXLimit = angularXLimit;
    }
}
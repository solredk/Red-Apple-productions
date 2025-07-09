using UnityEngine;

public class PlayerLook : MonoBehaviour // Look & Interact checker 
{
    [Header("Mouse and Controller Sensitivity")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float controllerSensitivity = 100f;
    [SerializeField] private Camera cam;
    private float Sensitivity;

    [SerializeField] private Transform playerBody;

    private Vector2 input;

    private float xRotation = 0f;

    private Interactable interactable;

    [Header("interacting")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerUI playerUI; // Make sure this is assigned in the Inspector

    [Header("Raycast Offset")]
    [SerializeField] private float raycastStartOffset = 0.1f;

    // Crosshair UI references
    [Header("Crosshairs")]
    [SerializeField] private GameObject regularCrosshair;
    [SerializeField] private GameObject enemyDetectCrosshair;
    [SerializeField] private GameObject itemSelectCrosshair;

    [SerializeField] private LayerMask toolLayer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (playerUI == null)
        {
            playerUI = GetComponent<PlayerUI>();
            if (playerUI == null)
            {
                Debug.LogError("PlayerUI component not found on this GameObject.  Please assign it in the Inspector or add the PlayerUI script to this GameObject.");
            }
        }
    }

    private void Update()
    {
        if (playerUI != null)
        {
            playerUI.UpdateText(string.Empty);
        }

        float mouseX = input.x * Sensitivity * Time.deltaTime;
        float mousey = input.y * Sensitivity * Time.deltaTime;

        xRotation -= mousey;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);

        Vector3 raycastOrigin = cam.transform.position + cam.transform.forward * raycastStartOffset;
        Debug.DrawRay(raycastOrigin, cam.transform.forward * interactDistance, Color.red);

        Ray ray = new Ray(raycastOrigin, cam.transform.forward);
        RaycastHit hitInfo;

        // --- UI Crosshair System ---
        // Use SphereCast for a "thicker" UI ray
        Ray uiRay = new Ray(raycastOrigin, cam.transform.forward);
        RaycastHit uiHit;
        bool crosshairSet = false;
        float uiRayThickness = 0.3f; // Adjust this value for more/less forgiveness

        if (Physics.SphereCast(uiRay, uiRayThickness, out uiHit, Mathf.Infinity))
        {
            if (uiHit.collider.GetComponent<Ingredient>() != null)
            {
                SetAllCrosshairs(false, false, false);
                crosshairSet = true;
            }
            else if (uiHit.collider.CompareTag("Enemy"))
            {
                SetAllCrosshairs(false, true, false);
                var img = enemyDetectCrosshair.GetComponent<UnityEngine.UI.Image>();
                if (img != null) img.color = Color.red;
                crosshairSet = true;
            }
            else if (((1 << uiHit.collider.gameObject.layer) & toolLayer.value) != 0)
            {
                SetAllCrosshairs(false, false, true);
                crosshairSet = true;
            }
        }
        if (!crosshairSet)
        {
            SetAllCrosshairs(true, false, false);
        }
        // --- End UI Crosshair System ---

        if (Physics.Raycast(ray, out hitInfo, interactDistance, interactLayer))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
            }
        }
    }

    private void SetAllCrosshairs(bool regular, bool enemy, bool item)
    {
        if (regularCrosshair != null) regularCrosshair.SetActive(regular);
        if (enemyDetectCrosshair != null) enemyDetectCrosshair.SetActive(enemy);
        if (itemSelectCrosshair != null) itemSelectCrosshair.SetActive(item);
    }

    public void OnInteract()
    {
        Debug.Log("OnInteract called - attempting to interact");

        Vector3 raycastOrigin = cam.transform.position + cam.transform.forward * raycastStartOffset;
        Ray ray = new Ray(raycastOrigin, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactDistance, interactLayer))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.Log($"Interacting with {hitInfo.collider.gameObject.name}");
                interactable.BaseInteract(gameObject);
            }
        }
    }

    public void Look(Vector2 lookInput, bool controllerActive)
    {
        if (controllerActive)
        {
            Sensitivity = controllerSensitivity;
        }
        else
        {
            Sensitivity = mouseSensitivity;
        }
        input = lookInput;
    }
}
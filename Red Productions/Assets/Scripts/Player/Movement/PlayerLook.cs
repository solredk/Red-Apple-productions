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
    [SerializeField] private PlayerUI playerUI;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerUI = GetComponent<PlayerUI>();
    }

    private void Update()
    {
        if (playerUI != null)
        {
            playerUI.UpdateText(string.Empty);
        }

        //if the controller is active, use the controller sensitivity
        float mouseX = input.x * Sensitivity * Time.deltaTime;
        float mousey = input.y * Sensitivity * Time.deltaTime;

        //rotate the camera around the y axis
        xRotation -= mousey;

        //clamp the x rotation to prevent the camera from flipping over
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rotate the camera around the x axis
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //rotate the player body around the y axis
        playerBody.Rotate(Vector3.up * mouseX);

        // Visual debug - will show the raycast in the Scene view
        Debug.DrawRay(cam.transform.position, cam.transform.forward * interactDistance, Color.red);

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;

        //create a raycast to check if the player is looking at an interactable object
        if (Physics.Raycast(ray, out hitInfo, interactDistance, interactLayer))
        {
            // Logging for debugging controller issues
          

            //check if the object has an interactable component
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
            }
        }
    }

    public void OnInteract()
    {
        // Debug log to verify the method is called from controller
        Debug.Log("OnInteract called - attempting to interact");
        
        // Immediately check if we're looking at an interactable and trigger it
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactDistance, interactLayer))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                Debug.Log($"Interacting with {hitInfo.collider.gameObject.name}");
                interactable.BaseInteract();
            }
        }
    }

    public void Look(Vector2 lookInput, bool controllerActive)
    {
        if (controllerActive)
        {
            //if the controller is active, use the controller sensitivity
            Sensitivity = controllerSensitivity;
        }
        else
        {
            //if the controller is not active, use the mouse sensitivity
            Sensitivity = mouseSensitivity;
        }

        //save the input to be used in the update function
        input = lookInput;
    }
}
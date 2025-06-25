using UnityEngine;

public class PlayerLook : MonoBehaviour
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

    private bool interacted;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
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


        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;

        //create an raycast to check if the player is looking at an interactable object
        if (Physics.Raycast(ray, out hitInfo, interactDistance, interactLayer))
        {
            //check if the object has an interactable component
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                //if it does put it in an variable
                interactable = hitInfo.collider.GetComponent<Interactable>();
                //if you have interacted with the object call the interact function and put interacted into false again
                if (interacted)
                {
                    interactable.BaseInteract();
                    interacted = false;
                }
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

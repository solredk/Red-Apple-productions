using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Mouse and Controller Sensitivity")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float controllerSensitivity = 100f;
    private float Sensitivity; 

    [SerializeField] private Transform playerBody;

    private Vector2 input;

    private float xRotation = 0f;

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
    }

    public void Look(Vector2 lookInput, bool controllerActive)
    {
        if (controllerActive)
            //if the controller is active, use the controller sensitivity
            Sensitivity = controllerSensitivity;

        else  
            //if the controller is not active, use the mouse sensitivity
            Sensitivity = mouseSensitivity;

        //save the input to be used in the update function
        input = lookInput;
    }
}

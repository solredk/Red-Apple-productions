using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Mouse and Controller Sensitivity")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float controllerSensitivity = 100f;
    [SerializeField] private float Sensitivity; 

    private Vector2 input;
    private float xRotation = 0f;

    [SerializeField] private Transform playerBody;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = input.x * Sensitivity * Time.deltaTime;
        float mousey = input.y * Sensitivity * Time.deltaTime;

        xRotation -= mousey;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
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

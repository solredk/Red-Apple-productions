using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("mouse sensetivity")]

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] private Camera cam;

    private float mouseX;
    private float mouseY;

    private float multiplier = 0.01f;

    private float xRotation;
    private float yRotation;


    private void Update()
    {
        if ( Cursor.lockState != CursorLockMode.Locked)
        {
            //make you unable to see the cursor and lock it in the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //rotate the camera and the player
        if (cam != null)
        {
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }

    }


    public void Look(Vector2 input)
    {
        //putting the mouse movements into variables
        mouseX = input.x;
        mouseY = input.y;

            //rotate the camera and the player
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}


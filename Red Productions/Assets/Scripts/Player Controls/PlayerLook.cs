using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float controllerSensitivity = 200f;
    [SerializeField] private float controllerExponent = 1.5f; // curve-exponent
    [SerializeField] private Camera cam;

    private Vector2 input;
    private bool controllerActive;

    private float xRotation;
    private float yRotation;

    private float smoothX;
    private float smoothY;
    private float smoothVelocityX;
    private float smoothVelocityY;
    [SerializeField] private float smoothTime = 0.05f;

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        float sens = controllerActive ? controllerSensitivity : mouseSensitivity;

        // Smooth damp op input
        smoothX = Mathf.SmoothDamp(smoothX, input.x, ref smoothVelocityX, smoothTime);
        smoothY = Mathf.SmoothDamp(smoothY, input.y, ref smoothVelocityY, smoothTime);

        yRotation += smoothX * sens * Time.deltaTime;
        xRotation -= smoothY * sens * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void Look(Vector2 lookInput, bool controller)
    {
        controllerActive = controller;

        if (controller)
        {
            if (lookInput.magnitude < 0.1f)
                lookInput = Vector2.zero;
            else
            {
                // Apply sensitivity curve
                lookInput.x = Mathf.Pow(Mathf.Abs(lookInput.x), controllerExponent) * Mathf.Sign(lookInput.x);
                lookInput.y = Mathf.Pow(Mathf.Abs(lookInput.y), controllerExponent) * Mathf.Sign(lookInput.y);
            }
        }

        input = lookInput;
    }
}

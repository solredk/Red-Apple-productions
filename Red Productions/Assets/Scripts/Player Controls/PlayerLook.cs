using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
public class PlayerLook : MonoBehaviour
{
    [Header("mouse sensetivity")]

    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] private Camera cam;

    private float mouseX;
    private float mouseY;

    private bool controllerActive;

    private float multiplier = 0.01f;

    private float xRotation;
    private float yRotation;


    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (cam != null)
        {
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        if (!controllerActive)
        {
            sensX = 8;
            sensY = 8;
        }
        else
        {
            sensX = 100;
            sensY = 100;
        }
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }


    public void Look(Vector2 input,bool controller)
    {
        mouseX = input.x;
        mouseY = input.y;
        controllerActive = controller;
    }
}


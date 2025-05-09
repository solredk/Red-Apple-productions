using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 lookInput;

    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private TomatoLauncher tomatoLauncher;
    [SerializeField] private PickupAndDrop pickupAndDrop;
    private Gamepad gamepad;
    private float rumbleDuration = 0.2f; // hoe lang de trilling duurt
    private float rumbleTimer;

    private void Update()
    {
        // Check of rumble aanstaat en update timer
        if (rumbleTimer > 0)
        {
            rumbleTimer -= Time.deltaTime;
            if (rumbleTimer <= 0)
            {
                StopRumble();
            }
        }
    }
    public void DoShooting(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tomatoLauncher.isShooting = true;
            if (context.control.device == Gamepad.current)
            {
                tomatoLauncher.controllerActive = true;
                tomatoLauncher.gamepad = context.control.device as Gamepad;
            }

        }

        if (context.canceled)
        {
            tomatoLauncher.isShooting = false;
        }
    }
    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.performed && pickupAndDrop != null)
        {
            pickupAndDrop.Pickup();
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (context.performed && pickupAndDrop != null)
        {
            pickupAndDrop.Drop();
        }
    }
    public void DoMoving(InputAction.CallbackContext context)
    {

        moveInput = context.ReadValue<Vector2>();
        if (PlayerMovement != null)
        {
            PlayerMovement.ReadMoveVaulue(moveInput);
        }
    }

    public void DoLooking(InputAction.CallbackContext context)
    {


        lookInput = context.ReadValue<Vector2>();
        if (playerLook != null)
        {
            playerLook.Look(lookInput,context.control.device == Gamepad.current);
        }

    }
    private void StartRumble(float lowFrequency, float highFrequency, float duration)
    {
        gamepad = Gamepad.current;
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            rumbleTimer = duration;
        }
    }
    public void DoAdjustDistance(InputAction.CallbackContext context)
    {
        if (context.performed && pickupAndDrop != null)
        {
            float scrollDelta = context.ReadValue<float>();
            pickupAndDrop.AdjustHoldDistance(scrollDelta);
            Debug.Log(scrollDelta);
        }
    }
    private void StopRumble()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
}


using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CurrentObjectType
{
    tomatoLauncher,
    item
}

public class InputManager : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 lookInput;

    public CurrentObjectType currentObjectType;

    private Gamepad gamepad;

    [SerializeField] private PlayerMovement PlayerMovement;

    [SerializeField] private PlayerLook playerLook;

    [SerializeField] private TomatoLauncher tomatoLauncher;

    [SerializeField] private PickupAndDrop pickupAndDrop;

    [SerializeField] private UIManager UIManager;

    [SerializeField] private PlayerInputManager playerInputManager;



    private void Awake()
    {
        UIManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<UIManager>();
    }

    public void DoShooting(InputAction.CallbackContext context)
    {
        Debug.Log(context.control.device);
        if (playerInputManager != null && playerInputManager.playerCount != 2)
            return;
        if (context.performed)
        {
            if (context.control.device == gamepad)
            {
                tomatoLauncher.gamepad = gamepad;
            }
            tomatoLauncher.isShooting = true;
        }

        if (context.canceled)
        {
            tomatoLauncher.isShooting = false;
        }
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2)
            return;
        if (context.performed && pickupAndDrop != null)
        {
            currentObjectType = CurrentObjectType.item;
            pickupAndDrop.Pickup();
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2)
            return;
        if (context.performed && pickupAndDrop != null)
        {
            currentObjectType = CurrentObjectType.tomatoLauncher;
            pickupAndDrop.Drop();
        }
    }
    public void DoMoving(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2)
            return;
        moveInput = context.ReadValue<Vector2>();
        if (PlayerMovement != null)
        {
            PlayerMovement.ReadMoveVaulue(moveInput);
        }
    }

    public void DoLooking(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2)
            return;
        lookInput = context.ReadValue<Vector2>();
        if (playerLook != null)
        {
            playerLook.Look(lookInput,context.control.device == Gamepad.current);
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
    
    public void DoPause(InputAction.CallbackContext context)
    {
        if (context.performed && UIManager != null)
        {
            UIManager.Pause();
        }
    }
}


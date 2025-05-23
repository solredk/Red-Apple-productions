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
    private Vector2 ScrollInput;

    public CurrentObjectType currentObjectType;

    private Gamepad gamepad;

    private GameObject manager;

    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private TomatoLauncher tomatoLauncher;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private Pickup pickup;

    private PlayerInputManager playerInputManager;
    private UIManager UIManager;


    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager");
        UIManager = manager.GetComponent<UIManager>();        
        playerInputManager = manager.GetComponent<PlayerInputManager>();
    }
    public void DoShooting(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2 || playerHealth.playerState == PlayerState.dead)
            return;
        
        if (context.performed)
        {
            if (context.control.device is Gamepad gp)
            {
                gamepad = gp; 
                tomatoLauncher.gamepad = gp;
                tomatoLauncher.controllerActive = true;
            }

            else
                tomatoLauncher.controllerActive = false;
            
            tomatoLauncher.isShooting = true;
        }

        if (context.canceled)
            tomatoLauncher.isShooting = false;
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2 || playerHealth.playerState == PlayerState.dead)
            return;

        if (context.performed && pickup != null)
            pickup.PickuP();
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2 || playerHealth.playerState == PlayerState.dead)
            return;

        if (context.performed && pickup != null)
            pickup.Drop();
    }

    public void DoMoving(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2 || playerHealth.playerState == PlayerState.dead)
            return;

        moveInput = context.ReadValue<Vector2>();
        if (PlayerMovement != null)
            PlayerMovement.ReadMoveVaulue(moveInput);
    }
 
    public void DoLooking(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2 || playerHealth.playerState == PlayerState.dead)
            return;

        lookInput = context.ReadValue<Vector2>();

        if (playerLook != null)
            playerLook.Look(lookInput,context.control.device == Gamepad.current);
    }

    public void DoAdjustDistance(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2 || playerHealth.playerState == PlayerState.dead)
            return;

        if (context.performed)
            ScrollInput = context.ReadValue<Vector2>();
    }
    
    public void DoPause(InputAction.CallbackContext context)
    {
        if (playerInputManager != null && playerInputManager.playerCount != 2 || playerHealth.playerState == PlayerState.dead)
            return;

        if (context.performed && UIManager != null)
            UIManager.Pause();
    }
}


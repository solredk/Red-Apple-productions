using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 lookInput;

    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private TomatoLauncher tomatoLauncher;

    public void DoJump(InputAction.CallbackContext context)
    {
        if (context.performed && PlayerMovement != null)
        {
            PlayerMovement.DoJump();
        }
    }
    public void DoShooting(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tomatoLauncher.isShooting = true;
        }

        if (context.canceled)
        {
            tomatoLauncher.isShooting = false;
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
}

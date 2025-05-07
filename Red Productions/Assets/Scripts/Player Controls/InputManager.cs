using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Vector2 moveInput;
    private Vector2 lookInput;

    [SerializeField] private PlayerMovement PlayerMovement;

    public void DoJump(InputAction.CallbackContext context)
    {
        if (context.performed && PlayerMovement != null)
        {
            PlayerMovement.DoJump();
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
    }
}

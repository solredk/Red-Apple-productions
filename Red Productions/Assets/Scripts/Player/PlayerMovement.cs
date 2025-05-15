using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("playerstats")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.8f;

    [SerializeField] private CharacterController controller;
    
    private Vector3 playerVelocity;

    private bool isGrounded;
    private bool isSprinting;
    
    private float currentSpeed;

    private Vector2 moveInput;

    private void Update()
    {
        Moving();
    }

    public void Moving()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;  
        }

        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        Vector3 moveDirection = new (moveInput.x, 0, moveInput.y);
        controller.Move(currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection));

        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void ReadMoveVaulue(Vector2 input)
    {
        moveInput = input;
    }
}

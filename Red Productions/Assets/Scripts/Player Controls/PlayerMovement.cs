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

    private Vector2 velocity;

    private void Update()
    {
        Moving();
    }


    public void Moving()
    {
        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;


        isGrounded = controller.isGrounded;
    }

    public void ReadMoveVaulue(Vector2 input)
    {
        moveInput = input;
    }

    public void ApplyGravity()
    {
        velocity
    }

    public void DoJump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(2f * -gravity);
        }
    }

}

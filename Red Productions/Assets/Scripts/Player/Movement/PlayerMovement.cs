using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("playerstats")]
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private float gravity = -9.8f;

    [Header("Character Controle")]
    [SerializeField] private CharacterController controller;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private Vector3 playerVelocity;

    private Vector2 moveInput;
    
    private bool isGrounded;


    private void Update()
    {
        // Check if the player is falling below a certain height and reset position 
        if (transform.position.y < -10f)
        {
            transform.position = new Vector3(0, 1, 0);
        }        
        
        //if no input, return imidiately
        if (moveInput == Vector2.zero)
            return;      

        // the player starts the move animation
        animator.SetFloat("Speed", moveInput.magnitude);

        //call the move function
        Moving();
    }

    public void Moving()
    {
        // Check if the player is grounded
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        //checking the direction of the movement by calculating the input vector
        Vector3 moveDirection = new (moveInput.x, 0, moveInput.y);

        //move the player in the direction of the input
        controller.Move(currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection));

        playerVelocity.y += gravity * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void ReadMoveVaulue(Vector2 input)
    {
        moveInput = input;
    }
}

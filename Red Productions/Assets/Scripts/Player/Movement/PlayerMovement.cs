using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("playerstats")]
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private float gravity = -9.8f;

    [SerializeField] private CharacterController characterController;

    [SerializeField] private Animator animator;

    private Vector3 playerVelocity;

    private bool isGrounded;

    private Vector2 moveInput;

    private void Update()
    {
        animator.SetFloat("speed", moveInput.magnitude);

        if (moveInput == Vector2.zero)
            return;


        Moving();

        // Check if the player is falling below a certain height and reset position 
        if (transform.position.y < -10f)
        {
            transform.position = new Vector3(0, 1, 0);
        }
    }

    public void Moving()
    {
        isGrounded = characterController.isGrounded;
        
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;




        //getting the input from the readvalue function
        Vector3 moveDirection = new (moveInput.x, 0, moveInput.y);


        characterController.Move(currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection));

        playerVelocity.y += gravity * Time.deltaTime;

        characterController.Move(playerVelocity * Time.deltaTime);
    }

    public void ReadMoveVaulue(Vector2 input)
    {
        moveInput = input;
    }
}

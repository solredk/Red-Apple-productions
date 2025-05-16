using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("playerstats")]
    [SerializeField] private float currentSpeed = 5f;
    [SerializeField] private float gravity = -9.8f;

    [SerializeField] private CharacterController characterController;
    
    private Vector3 playerVelocity;

    private bool isGrounded;

    private Vector2 moveInput;

    private void Update()
    {
        Moving();
    }

    public void Moving()
    {
        isGrounded = characterController.isGrounded;
        
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;  
        }

        //getting the input from the readvalue function
        Vector3 moveDirection = new (moveInput.x, 0, moveInput.y);

        characterController.Move(currentSpeed * Time.deltaTime * transform.TransformDirection(moveDirection));

        playerVelocity.y += gravity * Time.deltaTime;

        characterController.Move(playerVelocity * Time.deltaTime);
    }

    public void ReadMoveVaulue(Vector2 input)
    {
        //saving the input to be used in the update function
        moveInput = input;
    }
}

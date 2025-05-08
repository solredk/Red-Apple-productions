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


    private float velocity;

    private void Update()
    {
        Moving();
    }


    public void Moving()
    {
        isGrounded = controller.isGrounded;

        // Als hij op de grond staat en naar beneden beweegt → hou hem stabiel op de grond
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;  // kleine negatieve waarde om op de grond te blijven
        }

        // Snelheid kiezen
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Beweging horizontaal
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        // Gravity toepassen
        playerVelocity.y += gravity * Time.deltaTime;

        // Verticale beweging toepassen
        controller.Move(playerVelocity * Time.deltaTime);
    }


    public void ReadMoveVaulue(Vector2 input)
    {
        moveInput = input;
    }



}

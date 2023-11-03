using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float acceleration = 15.0f;
    public float maxWalkSpeed = 4.0f;
    public float maxRunSpeed = 7.8f;
    public float deceleration = 25.0f;
    private CharacterController controller;
    private Vector3 velocity = Vector3.zero;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    public float dashSpeed = 15.0f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private bool hasAirDashed = false;
    public Transform playerCamera;
    public CameraBob cameraBob;

    private enum PlayerState
    {
        Grounded,
        Jumping,
        Dashing
    }

    private PlayerState playerState = PlayerState.Grounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
{
    // Check if the player is grounded at the start of each frame
    if (controller.isGrounded)
    {
        hasAirDashed = false;
        
        if (cameraBob.IsJumping()){
            cameraBob.SetJumping(false);
            hasAirDashed = false;
        }
        if (playerState == PlayerState.Jumping)
        {
            playerState = PlayerState.Grounded;
            cameraBob.DoLandBob();
        }
    }
    else if (playerState != PlayerState.Dashing)
    {
        playerState = PlayerState.Jumping;
    }

    switch (playerState)
    {
        case PlayerState.Grounded:
            HandleGroundedState();
            break;
        case PlayerState.Jumping:
            HandleJumpingState();
            break;
        case PlayerState.Dashing:
            // Dashing is handled in the coroutine
            break;
    }

    // Apply gravity every frame regardless of the state
    if (!controller.isGrounded)
    {
        velocity.y += gravity * Time.deltaTime;
    }
    if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing && (controller.isGrounded || !hasAirDashed)) {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 dashDirection = playerCamera.TransformDirection(input).normalized;
            dashDirection.y = 0; // Ensure we don't dash upwards or downwards
            StartCoroutine(Dash(dashDirection));
            if (!controller.isGrounded) {
                hasAirDashed = true;
            }
        }
    // Move the player
    controller.Move(velocity * Time.deltaTime);

    
}


    private void HandleGroundedState()
    {
        if (controller.isGrounded)
        {
            hasAirDashed = false;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                playerState = PlayerState.Jumping;
                cameraBob.DoJumpBob();
            }
            else
            {
                float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? maxRunSpeed : maxWalkSpeed;
                Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                Vector3 desiredDirection = playerCamera.TransformDirection(input).normalized;
                desiredDirection.y = 0;

                velocity.x = Mathf.MoveTowards(velocity.x, desiredDirection.x * targetSpeed, acceleration * Time.deltaTime);
                velocity.z = Mathf.MoveTowards(velocity.z, desiredDirection.z * targetSpeed, acceleration * Time.deltaTime);

                if (desiredDirection == Vector3.zero)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
                    velocity.z = Mathf.MoveTowards(velocity.z, 0, deceleration * Time.deltaTime);
                }

                velocity.y += gravity * Time.deltaTime; // Apply gravity

                if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing)
                {
                    StartCoroutine(Dash(desiredDirection));
                }
            }
        }
    }

    private void HandleJumpingState()
    {
        if (controller.isGrounded)
        {
            playerState = PlayerState.Grounded;
            cameraBob.DoLandBob();
        }
        else
        {
            velocity.y += gravity * Time.deltaTime; // Apply gravity
        }
    }

    IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        playerState = PlayerState.Dashing;
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            controller.Move(direction * dashSpeed * Time.deltaTime);
            yield return null;
        }
        isDashing = false;
        if (controller.isGrounded) {
            hasAirDashed = false; // Reset air dash when grounded
        }
        velocity = direction * maxWalkSpeed; // Reset velocity to walking speed after dashing
        isDashing = false;
        playerState = PlayerState.Grounded; // Assume the player will be on the ground after dashing
    }
}

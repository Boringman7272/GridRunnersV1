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
    private bool isSprinting = false;
    public Transform playerCamera;
    public CameraBob cameraBob;
    private bool justLanded = false;
    public float landingDeceleration = 100.0f; 

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
    bool wasGrounded = controller.isGrounded;
    // Check if the player is grounded at the start of each frame

    if (!wasGrounded && controller.isGrounded)
        {
            justLanded = true;
        }

    
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


    if (justLanded)
        {
            // Apply rapid deceleration
            velocity.x = Mathf.MoveTowards(velocity.x, 0, landingDeceleration * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, 0, landingDeceleration * Time.deltaTime);
            justLanded = false;
        }
    else{
    // Toggle sprint when the sprint key is pressed
    if (Input.GetKeyDown(KeyCode.LeftShift)) {
        isSprinting = !isSprinting; // Toggle the sprint state
    }

    // Set target speed based on whether the player is sprinting
    float targetSpeed = isSprinting ? maxRunSpeed : maxWalkSpeed;

    if (Input.GetButtonDown("Jump"))
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        playerState = PlayerState.Jumping;
        cameraBob.DoJumpBob();
    }
    else
    {
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

    // Store the player's current momentum
    Vector3 currentMomentum = new Vector3(velocity.x, 0, velocity.z);

    // Calculate the dash force based on the look direction
    Vector3 dashForce = direction * dashSpeed;

    // Blend the current momentum with the dash force
    Vector3 resultingMomentum = currentMomentum + dashForce;

    while (Time.time < startTime + dashDuration)
    {
        // Apply the resulting momentum instead of just the dash direction
        controller.Move(resultingMomentum * Time.deltaTime);
        yield return null;
    }

    // After dashing, you might want to set the player's velocity to the resulting momentum
    // or smoothly transition back to the player's normal movement speed.
    velocity = new Vector3(resultingMomentum.x, velocity.y, resultingMomentum.z);

    isDashing = false;
    if (controller.isGrounded) {
        hasAirDashed = false; // Reset air dash when grounded
    }
    playerState = controller.isGrounded ? PlayerState.Grounded : PlayerState.Jumping;
}
}
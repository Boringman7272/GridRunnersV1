using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float acceleration = 15.0f;
    public float maxWalkSpeed = 5f;
    public float maxRunSpeed = 8.5f;
    public float deceleration = 25.0f;
    private CharacterController controller;
    private Vector3 velocity = Vector3.zero;
    public float jumpHeight = 2.0f;
    public float gravity = -5.5f;
    public float dashSpeed = 15.0f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private bool hasAirDashed = false;
    private bool isSprinting = false;
    public bool coolDownCheck = true;
    public Transform playerCamera;
    public CameraBob cameraBob;
    private bool justLanded = false;
    public float landingDeceleration = 100.0f; 
    public float dashCooldown = 2.0f; // Cooldown duration in seconds
    private float lastDashTime = -Mathf.Infinity; 
    public GameObject dashReadyPopup;   // Initialize to a negative value so the player can dash immediately
    public float airControlFactor = 0.8f;  // Factor to control air maneuverability
    public float airSpeedFactor = 1.25f;
    private float jumpChargeTime = 0f;
    private float maxJumpChargeTime = 1f; // Max charge time of 1 second
    private float airTime = 4f;
    private float maxAirAcceleration = 20f;
    private bool isChargingJump = false;
    public Slider jumpChargeSlider;


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
    Debug.Log("Current Time: " + Time.time);
    Debug.Log("Last Dash Time: " + lastDashTime);
    Debug.Log("Next Dash Available After: " + (lastDashTime + dashCooldown));
    Debug.Log("Cooldown Check: " + (Time.time >= lastDashTime + dashCooldown));

    DashAction();
    DashPopup();
    
}

private void DashPopup(){
    
    if (Time.time >= lastDashTime + dashCooldown)
        {
            if (!dashReadyPopup.activeSelf)
            {
                dashReadyPopup.SetActive(true); // Show the popup
            }
        }
    else{
        if (dashReadyPopup.activeSelf)
        {
        dashReadyPopup.SetActive(false);
        }
    }

}


private void DashAction()
{
    if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashing && Time.time >= lastDashTime + dashCooldown && (controller.isGrounded || !hasAirDashed))
    {
        bool coolDownCheck = Time.time >= lastDashTime + dashCooldown;
        Debug.Log("cooldown check inside dashaction" + coolDownCheck);
        if(coolDownCheck){
            
        
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 dashDirection = playerCamera.TransformDirection(input).normalized;
        dashDirection.y = 0; // Ensure we don't dash upwards or downwards
        StartCoroutine(Dash(dashDirection));
        if (!controller.isGrounded) {
            hasAirDashed = true;
        }
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
            coolDownCheck = true;
            justLanded = false;
        }
    else{
    // Toggle sprint when the sprint key is pressed
    if (Input.GetKeyDown(KeyCode.LeftShift)) {
        isSprinting = !isSprinting; // Toggle the sprint state
    }

    // Set target speed based on whether the player is sprinting
    float targetSpeed = isSprinting ? maxRunSpeed : maxWalkSpeed;

        if (controller.isGrounded && Input.GetButtonDown("Jump"))
    {
        isChargingJump = true;
        jumpChargeTime = 0f;
        jumpChargeSlider.gameObject.SetActive(true);
    }

    if (isChargingJump && Input.GetButton("Jump"))
    {
        jumpChargeTime = Mathf.Min(jumpChargeTime + Time.deltaTime, maxJumpChargeTime);
        jumpChargeSlider.value = jumpChargeTime / maxJumpChargeTime;
    }

    if (isChargingJump && Input.GetButtonUp("Jump"))
    {
        PerformChargedJump();
        isChargingJump = false;
        jumpChargeSlider.gameObject.SetActive(false);
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

    }
}
} 

    private void HandleJumpingState()
    {
        if (!controller.isGrounded)
        {
        velocity.y += gravity * Time.deltaTime; // Apply gravity
        float airAcceleration = Mathf.Min(acceleration * airControlFactor * airTime, maxAirAcceleration);
        float maxAirSpeed = maxWalkSpeed * airSpeedFactor; // airSpeedFactor < 1

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 desiredDirection = playerCamera.TransformDirection(input).normalized;
        desiredDirection.y = 0;

        // Apply air acceleration and limit to max air speed
        velocity.x = Mathf.Clamp(velocity.x + desiredDirection.x * airAcceleration * Time.deltaTime, -maxAirSpeed, maxAirSpeed);
        velocity.z = Mathf.Clamp(velocity.z + desiredDirection.z * airAcceleration * Time.deltaTime, -maxAirSpeed, maxAirSpeed);
    
        }
        else
        {
            
            playerState = PlayerState.Grounded;
            cameraBob.DoLandBob();
        }
    }

IEnumerator Dash(Vector3 direction)
{
    Debug.Log("Dash coroutine started at: " + Time.time);

    if (isDashing)
{
    yield break; // Exit if already dashing
}
    Debug.Log("Dash initiated at: " + Time.time);
    Debug.Log("Next dash available after: " + (lastDashTime + dashCooldown));
    isDashing = true;
    playerState = PlayerState.Dashing;
    lastDashTime = Time.time;

    Debug.Log("Dashtime being set " + lastDashTime);

    // Store the player's current momentum
    Vector3 currentMomentum = new Vector3(velocity.x, 0, velocity.z);

    // Calculate the dash force based on the look direction
    Vector3 dashForce = direction * dashSpeed;

    // Blend the current momentum with the dash force
    Vector3 resultingMomentum = currentMomentum + dashForce;

    while (Time.time < lastDashTime + dashDuration)
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
private void PerformChargedJump()
{
    float jumpForce = Mathf.Sqrt(jumpChargeTime / maxJumpChargeTime * jumpHeight * -2f * gravity);
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

    // Apply jump force while maintaining horizontal momentum
    velocity = horizontalVelocity + new Vector3(0, jumpForce, 0);
    // Set player state to jumping if you have such a state
}

}

using UnityEngine;

public class FloatingEnemy : MonoBehaviour
{
    public float desiredFloatingHeight = 1f; // Desired height above the ground
    public float floatStrength = 5f; // Strength of the floating force
    public float moveSpeed = 5f; // Speed of movement
    public float detectionRange = 10f; // Range within which the player is detected
    public float wanderRadius = 20f; // Radius within which the enemy will wander
    private Rigidbody rb;
    private Transform player;
    private Vector3 wanderPoint;
    public float impactForce = 10f; // The force applied to the player on impact
    public int damage = 10;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ChooseNewWanderPoint();
    }

    void FixedUpdate()
{
    // Check for ground and apply floating force if needed
    if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, desiredFloatingHeight + 0.1f)) // Slightly longer than the desired height
    {
        float heightAboveGround = hit.distance;
        if (heightAboveGround < desiredFloatingHeight)
        {
            float forceStrength = floatStrength * (1f - heightAboveGround / desiredFloatingHeight);
            rb.AddForce(Vector3.up * forceStrength);
        }
    }

    // Determine movement: wander, chase player, or attack downwards based on player's position
    Vector3 targetPoint;
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    if (distanceToPlayer <= detectionRange)
    {
        // Check if player is directly below
        if (IsPlayerDirectlyBelow())
        {
            // Initiate downward attack movement
            targetPoint = new Vector3(player.position.x, transform.position.y - desiredFloatingHeight, player.position.z);
            MoveTowards(targetPoint, true); // True indicates this is an attack move
        }
        else
        {
            // Normal chase behavior
            targetPoint = player.position;
            MoveTowards(targetPoint, false);
        }
    }
    else
    {
        // Wander behavior
        targetPoint = wanderPoint;
        MoveTowards(targetPoint, false);
    }

    // Choose new wander point if close to the current one
    if (Vector3.Distance(transform.position, wanderPoint) < 1f)
    {
        ChooseNewWanderPoint();
    }
}


   void MoveTowards(Vector3 target, bool isAttacking)
{
    Vector3 directionToTarget = (target - transform.position).normalized;

    if (isAttacking)
    {
        // For an attacking move, consider both horizontal and vertical components
        // This allows the enemy to "dive" towards the player from an angle
        directionToTarget.y -= 0.5f; // Adjust this value to control the steepness of the attack angle
        rb.velocity = directionToTarget * moveSpeed * 2f; // Increase speed during attack
    }
    else
    {
        // For normal movement, maintain the desired floating height
        directionToTarget.y = 0; // Ignore vertical component for normal movement
        rb.MovePosition(rb.position + directionToTarget * moveSpeed * Time.fixedDeltaTime);
    }

    // Rotate to face the target direction
    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.fixedDeltaTime));
}


    bool IsPlayerDirectlyBelow()
{
    Vector3 toPlayer = (player.position - transform.position).normalized;
    return Vector3.Dot(toPlayer, Vector3.down) > 0.95; // Using dot product to check if the player is mostly below the enemy
}

    void ChooseNewWanderPoint()
    {
        // Random point within wanderRadius
        wanderPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        wanderPoint.y = transform.position.y; // Keep the y position unchanged to maintain floating height
    }

      void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Check if the collided object is the Player
        {
            // You can apply force, damage, or any other effect upon collision with the player here
            var playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Apply an impact force to the player
                Vector3 forceDirection = collision.contacts[0].point - transform.position;
                forceDirection = -forceDirection.normalized; // Invert direction for pushing away
                playerRb.AddForce(forceDirection * impactForce, ForceMode.Impulse);
            }

            // Apply damage to the player (assuming the player has a method to take damage)
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Optional: Destroy or disable the enemy upon impact
            // Destroy(gameObject); // Uncomment to destroy the enemy on impact
        }
    }
}

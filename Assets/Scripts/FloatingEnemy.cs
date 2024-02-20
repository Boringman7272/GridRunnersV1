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

        // Determine movement: wander or chase player based on detection range
        Vector3 targetPoint = Vector3.Distance(transform.position, player.position) <= detectionRange ? player.position : wanderPoint;
        MoveTowards(targetPoint);

        // Choose new wander point if close to the current one
        if (Vector3.Distance(transform.position, wanderPoint) < 1f)
        {
            ChooseNewWanderPoint();
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        rb.MovePosition(rb.position + directionToTarget * moveSpeed * Time.fixedDeltaTime);

        // Rotate to face the target (optional)
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.fixedDeltaTime));


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

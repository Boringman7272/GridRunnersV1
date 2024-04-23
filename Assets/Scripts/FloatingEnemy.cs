using System.Collections;
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
    public float dashForce = 20f; // The force applied during a dash
    private bool isDashing = false; // To track if the enemy is currently dashing


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ChooseNewWanderPoint();
        StartCoroutine(DashTimer());
        
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, desiredFloatingHeight + 0.1f))
        {
            float heightAboveGround = hit.distance;
            if (heightAboveGround < desiredFloatingHeight)
            {
                float forceStrength = floatStrength * (1f - heightAboveGround / desiredFloatingHeight);
                rb.AddForce(Vector3.up * forceStrength);
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 targetPoint = wanderPoint; // Default to wander point

        if (!isDashing) // Only decide new movement if not currently dashing
        {
            if (distanceToPlayer <= detectionRange)
            {
                if (IsPlayerDirectlyBelow())
                {
                    targetPoint = new Vector3(player.position.x, transform.position.y - desiredFloatingHeight, player.position.z);
                }
                else
                {
                    targetPoint = player.position;
                }
            }

            MoveTowards(targetPoint, IsPlayerDirectlyBelow());
        }

        if (distanceToPlayer > detectionRange || Vector3.Distance(transform.position, wanderPoint) < 1f)
        {
            ChooseNewWanderPoint();
        }
}


   void MoveTowards(Vector3 target, bool isAttacking)
{
    Vector3 directionToTarget = (target - transform.position).normalized;
if (!isDashing) // Only move normally if not in a dash
        {
            directionToTarget = (target - transform.position).normalized;
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

            
        }
    }
    IEnumerator DashTimer()
    {
        Debug.Log("starting dash timer");
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f)); // Wait for a random time between 3-8 seconds

            if (Vector3.Distance(transform.position, player.position) <= detectionRange && !IsPlayerDirectlyBelow())
            {
                // Perform dash
                StartCoroutine(PerformDash());
            }
        }
    }

    IEnumerator PerformDash()
    {
        Debug.Log("starting dash");
        isDashing = true; // Indicate the enemy is currently dashing

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        rb.AddForce(directionToPlayer * dashForce, ForceMode.Impulse); // Apply an impulse force towards the player

        yield return new WaitForSeconds(0.5f); // Duration of the dash effect

        isDashing = false; // Reset dashing state
    }

    public void OnDefeat()
    {
        
        FindObjectOfType<LevelManager>().EnemyDefeated();
    }
}

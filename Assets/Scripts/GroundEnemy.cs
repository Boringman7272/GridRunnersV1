using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GroundEnemy : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public float detectionRange = 10f;

    [Header("Wandering")]
    public float wanderRadius = 20f;
    private Vector3 wanderPoint;

    [Header("Jump Attack")]
    public float jumpTriggerDistance = 20f;
    public Vector3 jumpForce = new Vector3(0, 15f, 30f); // Significantly increased
    public float chargeUpTime = 1f;
    public float impactForce = 10f;
    public int damage = 20;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isJumping = false;
    private bool isCharging = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        ChooseNewWanderPoint();
    }

    void Update()
    {
        if (isJumping) return; // Skip normal behavior if in the middle of a jump

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer > jumpTriggerDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else if (!isCharging)
            {
                StartCoroutine(ChargeAndJump());
            }
        }
        else
        {
            Wander();
        }
    }

    void Wander()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            ChooseNewWanderPoint();
        }
    }

    void ChooseNewWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, -1);
        wanderPoint = hit.position;
        agent.SetDestination(wanderPoint);
    }

    IEnumerator ChargeAndJump()
    {
        isCharging = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(chargeUpTime);

        // Face the player directly before jumping
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Jump towards the player
        rb.AddRelativeForce(jumpForce, ForceMode.Impulse);

        isCharging = false;
        isJumping = true;

        // Wait for a bit after the jump to resume normal behavior
        yield return new WaitForSeconds(2f);
        isJumping = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // This helps in quick recovery and resumption of normal behavior after being affected by external forces.
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    
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

    public void OnDefeat()
    {
        // Other defeat logic...
        FindObjectOfType<LevelManager>().EnemyDefeated();
    }
    
}


using System.Collections;
using UnityEngine;
public class GroundEnemy : MonoBehaviour
{
    [Header("Player Tracking")]
    public Transform player;
    public float detectionRange = 20f;
    public float pursuitSpeed = 5f;

    [Header("Wandering")]
    public float wanderRadius = 20f;
    public float wanderSpeed = 2f;
    private Vector3 wanderPoint;

    [Header("Jump Attack")]
    public float jumpTriggerDistance = 10f;
    public Vector3 jumpForce = new Vector3(0, 5f, 10f);
    public float chargeUpTime = 1f;
    public float impactForce = 10f;
    public int damage = 20;

    private Rigidbody rb;
    private bool isJumping = false;
    private bool isCharging = false;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ChooseNewWanderPoint();
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isJumping && !isCharging)
        {
            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer <= jumpTriggerDistance)
                {
                    StartCoroutine(ChargeAndJump());
                }
                else
                {
                    MoveTowards(player.position, pursuitSpeed);
                }
            }
            else
            {
                Wander();
            }
        }
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    void Wander()
    {
        if (Vector3.Distance(transform.position, wanderPoint) < 2f)
        {
            ChooseNewWanderPoint();
        }
        else
        {
            MoveTowards(wanderPoint, wanderSpeed);
        }
    }

    void ChooseNewWanderPoint()
    {
        wanderPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        wanderPoint.y = transform.position.y;
    }

    IEnumerator ChargeAndJump()
{
    if (isGrounded)
    {
        isCharging = true;
        yield return new WaitForSeconds(chargeUpTime);

        Vector3 direction = (player.position - transform.position).normalized;
        rb.AddForce(new Vector3(direction.x, 1, direction.z) * jumpForce.magnitude, ForceMode.Impulse);

        isCharging = false;
        isJumping = true;
        isGrounded = false; // Enemy is no longer grounded as it has jumped

        yield return new WaitForSeconds(2f); // Time for the enemy to land and resume normal behavior
    }
}


    void OnCollisionEnter(Collision collision)
    {
        // Resetting the velocity upon collision to stop sliding
        if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = true;
        isJumping = false; // Enemy has landed and can jump again
    }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Ensure the enemy can jump again after landing
        if (collision.gameObject.CompareTag("Ground") && isJumping)
        {
            isJumping = false;
        }
    }
    void OnCollisionExit(Collision collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = false; // Enemy is no longer in contact with the ground
    }
}
    public void OnDefeat()
    {
        FindObjectOfType<LevelManager>().EnemyDefeated();
    }
}

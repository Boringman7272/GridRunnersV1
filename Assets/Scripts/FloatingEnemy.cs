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
}

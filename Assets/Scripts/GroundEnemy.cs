using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Make sure to include this namespace for pathfinding

public class GroundEnemy : MonoBehaviour
{
    public Transform player; // Player's transform
    public float detectionRange = 10f; // Range within which the player is detected
    public float attackRange = 2f; // Range within which the enemy can attack
    public float wanderRadius = 20f; // Radius within which the enemy will wander when the player is not detected
    private NavMeshAgent agent; // The NavMeshAgent component for pathfinding
    private Vector3 wanderPoint; // Point to wander to when the player is not in detection range

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        ChooseNewWanderPoint(); // Choose an initial wander point
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position); // Set the player's position as the destination

            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer(); // Call your attack function here
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                ChooseNewWanderPoint(); // Choose a new wander point when the current one is reached
            }
        }
    }

    void ChooseNewWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        wanderPoint = hit.position;

        agent.SetDestination(wanderPoint); // Set the wander point as the new destination
    }

    void AttackPlayer()
    {
        // Implement your attack logic here
        // This could be a simple melee attack, triggering an animation, etc.
        Debug.Log("Attacking the player!");
    }
}

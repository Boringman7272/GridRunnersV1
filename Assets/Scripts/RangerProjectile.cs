using UnityEngine;

public class RangerProjectile : MonoBehaviour
{
    
    public float homingRate = 2f; // How quickly the projectile turns to follow the player
    private Transform target; // Target to home in on
    public float speed = 20f;
    public Rigidbody rb;
    public GameObject explosionEffect;
    public float lifespan = 10f;
    public int damage = 10;
    public float impactForce = 10f;

    public void Initialize(Transform target)
    {
        this.target = target; // Set the target
    }
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifespan);
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized; // Direction towards the target
            Vector3 homingDirection = Vector3.Lerp(transform.forward, direction, homingRate * Time.deltaTime); // Slightly adjust the direction towards the target
            transform.forward = homingDirection; // Set the forward direction of the projectile
        }

        transform.position += transform.forward * speed * Time.deltaTime; // Move the projectile forward
    }

    void OnCollisionEnter(Collision collision){

    
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

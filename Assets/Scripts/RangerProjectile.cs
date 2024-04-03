using UnityEngine;

public class RangerProjectile : MonoBehaviour
{
    public float homingRate = 2f; // How quickly the projectile turns to follow the player
    private Transform target; // Target to home in on
    public float speed = 20f;
    public Rigidbody rb;
    public GameObject explosionEffect; // Explosion effect prefab
    public float lifespan = 10f; // Time before the projectile is automatically destroyed
    public int damage = 10;
    public float impactForce = 10f; // Force applied upon impact

    public void Initialize(Transform target)
    {
        this.target = target;
    }

    void Start()
    {
        // Use Rigidbody for physics-based movement
        rb.velocity = transform.forward * speed;
        
        Destroy(gameObject, lifespan); // Destroy the projectile after a certain time
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, homingRate * Time.fixedDeltaTime); // Smooth rotation towards the target
        }

        rb.velocity = transform.forward * speed; // Continue moving forward
    }

    void OnCollisionEnter(Collision collision)
    {
        // Instantiate explosion effect upon collision
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Optional: Apply force and damage to the hit object if it has relevant components
        Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>();
        if (hitRb != null)
        {
            Vector3 forceDirection = (collision.transform.position - transform.position).normalized;
            hitRb.AddForce(forceDirection * impactForce, ForceMode.Impulse);
        }

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        Destroy(gameObject); // Destroy the projectile on collision with any object
    }
}

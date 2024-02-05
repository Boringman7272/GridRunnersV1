using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{   private float currentSpeed;
    public float initialSpeed = 5f; // Initial speed of the rocket at launch
    public float maxSpeed = 20f; // Maximum speed the rocket can reach
    public float acceleration = 5f; 
    public Rigidbody rb;
    public GameObject explosionEffect;
    public GameObject rocketTrailEffect;
    public float lifespan = 10f;
    public float explosionForce = 500f;
    public float rocketDamage = 50f;
    public float explosionRadius = 5f;
    public Transform rocketTrail;
    private bool hasExploded = false;

    void Start()
    {
        currentSpeed = initialSpeed; // Set the initial speed of the rocket
        rb.velocity = transform.forward * currentSpeed; // Apply the initial velocity
        GameObject fireTrail = Instantiate(rocketTrailEffect, rocketTrail.position, Quaternion.identity);
        fireTrail.transform.SetParent(transform);
         fireTrail.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        Destroy(gameObject, lifespan);
        
    }
    
    void Update()
    {
        
        // Accelerate the rocket until it reaches max speed
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime; // Increase the current speed based on acceleration rate
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed); // Clamp the current speed to maxSpeed
            rb.velocity = transform.forward * currentSpeed; // Update the rocket's velocity
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Area effect damage
        Explode();

        // Explosion effect
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 2f); // Cleanup explosion effect after some time
        Destroy(gameObject); // Destroy rocket on impact
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     // Area effect damage
    //     Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
    //     foreach (Collider hit in colliders)
    //     {
    //         Shootable shootable = hit.GetComponent<Shootable>();
    //         if (shootable != null)
    //         {
    //             // Damage calculation can consider distance from explosion
    //             float distance = Vector3.Distance(hit.transform.position, transform.position);
    //             float damage = Mathf.Max(0, rocketDamage - (distance / explosionRadius) * rocketDamage);
    //             shootable.TakeDamage(damage);
    //         }
    //     }

    //     // Explosion effect
    //     GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
    //     Destroy(explosion, 2f); // Cleanup explosion effect after some time
    //     Destroy(gameObject); // Destroy rocket on impact
    // }
    public void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Find all colliders within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var collider in colliders)
        {
            // Apply force if the object has a Rigidbody
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        Shootable shootable = collider.GetComponent<Shootable>();
            if (shootable != null)
            {
                shootable.TakeDamage(rocketDamage);
            }

        Destroy(gameObject);
    }
}
}
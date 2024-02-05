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
    public Material redMaterial;

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

    public void Explode()
{
        if (hasExploded)
        return;

        hasExploded = true;
        Instantiate(explosionEffect, transform.position - new Vector3(1, 1, 1), Quaternion.identity);

        // Visualize the explosion radius
        GameObject explosionRadiusIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        explosionRadiusIndicator.transform.position = transform.position;
        explosionRadiusIndicator.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, explosionRadius * 2); // The radius is halved because the scale is diameter
        Destroy(explosionRadiusIndicator.GetComponent<Collider>()); // Remove the collider so it doesn't interfere with physics

        // Apply red material 
        redMaterial.color = Color.red;
        explosionRadiusIndicator.GetComponent<Renderer>().material = redMaterial;

    // Destroy the indicator after a short duration
        Destroy(explosionRadiusIndicator, 0.1f); // Adjust duration as needed

    // Explosion force and damage logic
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var collider in colliders)
        {
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
        }

    Destroy(gameObject);

}
}
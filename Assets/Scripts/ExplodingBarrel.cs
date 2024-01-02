using TMPro;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    public GameObject explosionEffect;
    public float explosionRadius = 10f;
    public float explosionForce = 700f;
    public int damage = 50;

    private bool hasExploded = false;

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
                shootable.TakeDamage(damage);
            }

        Destroy(gameObject);
    }
}
}

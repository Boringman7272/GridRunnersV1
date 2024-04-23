using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletScript : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody rb;
    public GameObject explosionEffect;
    public float lifespan = 10f;
    public float ARbulletdmg = 26f;

    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifespan);
    }

    void OnCollisionEnter(Collision collision)
    {
            Shootable shootable = collision.collider.GetComponent<Shootable>();
    if (shootable != null)
    {
        
        shootable.TakeDamage(ARbulletdmg);
    }
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);
        Destroy(gameObject); // Destroy bullet on impact
    }
}

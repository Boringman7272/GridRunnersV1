using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletScript : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody rb;
    public GameObject explosionEffect;

    void Start()
    {
        rb.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Add impact logic here
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 2f);
        Destroy(gameObject); // Destroy bullet on impact
    }
}

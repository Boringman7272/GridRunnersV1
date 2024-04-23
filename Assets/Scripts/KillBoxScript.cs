using UnityEngine;
using System.Collections;

public class KillBoxScript : MonoBehaviour
{
    private int damage = 10000000;
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            
            playerHealth.TakeDamage(damage);
        }
    
        else
        {
        Destroy(other.gameObject);
            
        }
}
}


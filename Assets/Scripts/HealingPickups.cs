using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class HealingPickups : MonoBehaviour
{
    public int healing = 50;

    void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player")) // Check if the collided object is the Player
            {
                // You can apply force, damage, or any other effect upon collision with the player here
                var playerRb = collision.gameObject.GetComponent<Rigidbody>();
                
                // Apply damage to the player (assuming the player has a method to take damage)
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    if(playerHealth.GetPlayerHealth() < playerHealth.maxHealth){
                    playerHealth.HealDamage(healing);
                    Destroy(gameObject);
                }
                }
                

            // Optional: Destroy or disable the enemy upon impact
            // Destroy(gameObject); // Uncomment to destroy the enemy on impact
        }
    }

}
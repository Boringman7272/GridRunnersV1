using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public int damage = 20; // The damage dealt to the player

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a PlayerHealth component
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // If it's the player, deal damage
            playerHealth.TakeDamage(damage);
        }
    }
}

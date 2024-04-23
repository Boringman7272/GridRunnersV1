using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shootable : MonoBehaviour
{
    public float health = 100f;
    public GameObject damageNumberPrefab;  
    public Transform canvasTransform;
     private bool isDead = false; 
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        health -= damage;
       

        ShowDamageNumber(damage);
        ExplodingBarrel explodingBarrel = GetComponent<ExplodingBarrel>();
        if (explodingBarrel != null && health <= 0)
        {
            explodingBarrel.Explode(); // Call Explode method if it's an exploding barrel
        }
        else if (health <= 0)
        {
            isDead = true; 
            Die(); // Normal death for non-exploding objects
        }

    }

    private void Die()
{
    
    // Get the FloatingEnemy component from the same GameObject
    FindObjectOfType<LevelManager>().EnemyDefeated();
    // Destroy the GameObject
    Destroy(gameObject);
    
}

    private void ShowDamageNumber(float damage)
    {
        GameObject dmgNumber = Instantiate(damageNumberPrefab, canvasTransform.position, Quaternion.identity);
        dmgNumber.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        // Parent it to the canvas if needed
        dmgNumber.transform.SetParent(canvasTransform, false);
    }
}

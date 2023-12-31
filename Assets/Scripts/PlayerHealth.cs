using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Image healthBarFill;
    public TextMeshProUGUI currentHp;
    public Slider HpSlider;
    void Start()
    {
        // Initialize player's health
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Public method to allow other scripts to cause damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Check if health falls below zero
        if (currentHealth <= 0)
        {
            Die();
        }

        // Optional: Add logic to update the UI with the new health value
        UpdateHealthUI();
    }

    private void Die()
    {
        // Add logic for what happens when the player dies
        Debug.Log("Player died!");
    }
    
    private void UpdateHealthUI()
    {
        // Update the health bar's fill amount
        currentHp.text = "Health: " + currentHealth.ToString() + "/" + maxHealth;
        //Old Hp bar before slider was set up
        //healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        HpSlider.value = (float)currentHealth / maxHealth;
    }
}

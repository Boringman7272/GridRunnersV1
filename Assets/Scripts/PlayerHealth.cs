using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth;
    public Image healthBarFill;
    public TextMeshProUGUI currentHp;
    public Slider HpSlider;
    public int predictedHealth;
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
    public void HealDamage(int healing)
    {
        predictedHealth = currentHealth + healing;

        bool isHealthAboveMax = predictedHealth >= maxHealth;

        switch (isHealthAboveMax)
        {
            case true:
                currentHealth = maxHealth;
            break; 

            case false:
                currentHealth = predictedHealth;
            break; 
        }
        
        //if(predictedHealth >= 100 ){

       // }
        //if (currentHealth <= 100)
        //{
       // currentHealth += healing;

       // }

        // Optional: Add logic to update the UI with the new health value
        UpdateHealthUI();
    }

    private void Die()
    {
        Debug.Log("Player died!");
        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        // Optionally add a delay to show a death animation or a fade out
        yield return new WaitForSeconds(2); // Wait for 2 seconds before changing the scene

        // Load the main menu scene
        SceneManager.LoadScene("Menus");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void UpdateHealthUI()
    {
        // Update the health bar's fill amount
        currentHp.text = "Health: " + currentHealth.ToString() + "/" + maxHealth;
        //Old Hp bar before slider was set up
        //healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        HpSlider.value = (float)currentHealth / maxHealth;
    }
    public int GetPlayerHealth()
    {
        return currentHealth;
    }
}

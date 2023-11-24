using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shootable : MonoBehaviour
{
    public float health = 100;
    public GameObject damageNumberPrefab;  
    public Transform canvasTransform;
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }

        ShowDamageNumber(damage);
    }

    private void Die()
    {
        // Add logic for what happens when the object is destroyed
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

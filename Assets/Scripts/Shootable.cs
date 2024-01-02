using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shootable : MonoBehaviour
{
    public float health = 100f;
    public GameObject damageNumberPrefab;  
    public Transform canvasTransform;
    public void TakeDamage(float damage)
    {
        health -= damage;
       

        ShowDamageNumber(damage);
        ExplodingBarrel explodingBarrel = GetComponent<ExplodingBarrel>();
        if (explodingBarrel != null && health <= 0)
        {
            explodingBarrel.Explode(); // Call Explode method if it's an exploding barrel
        }
        else if (health <= 0)
        {
            Die(); // Normal death for non-exploding objects
        }

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

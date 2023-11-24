using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class DamageNumber : MonoBehaviour
{
    public float moveSpeed = 0.1f;  // Adjusted for slower movement
    public float fadeDuration = 0.7f;  // Duration of fade effect
    private float elapsedTime = 0f;  // Tracks the elapsed time since creation

    public TextMeshProUGUI textComponent; 

    void Start()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Move up
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0), Space.World);

        // Fade out over the duration
        if (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
        }
        else
        {
            // Destroy when fade duration is exceeded
            Destroy(gameObject);
        }
    }
}


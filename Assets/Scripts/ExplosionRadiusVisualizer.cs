using UnityEngine;

public class ExplosionRadiusVisualizer : MonoBehaviour
{
    public float explosionRadius = 10f;
    
    void OnDrawGizmosSelected()
    {
        // Use the color and transparency you prefer
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        // Draw a sphere representing the explosion radius
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
using UnityEngine;

public class Ranger : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public GameObject projectilePrefab; // The projectile prefab
    public Transform firePoint; // The point from where the projectiles are shot
    public float shootInterval = 2f; // Time between each shot
    public float coneAngle = 30f;
    public float shootingRange = 300f;  // Maximum deviation angle from the forward direction for the projectiles

    private float shootTimer = 0f; // Timer to keep track of shooting cooldown

    void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; // Find player by tag
        }
    }

    void Update()
    {
        FacePlayer();
        
        CorrectModelOrientation();
        DrawFiringCone();
        Debug.DrawRay(firePoint.position, firePoint.forward * 10, Color.red, 2f);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= shootingRange)
        {
            HandleShooting();
        }

    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the rotation only on the Y axis
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Smoothly rotate towards the player
    }

    void HandleShooting()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0f;
        }
    }

    void Shoot()
    {
        // Calculate a random deviation angle within the cone
        float deviation = Random.Range(-coneAngle / 2, coneAngle / 2);

        // Apply the deviation to the projectile's forward direction
        Quaternion rotationDeviation = Quaternion.Euler(0, deviation, 0); // Deviation around the Y axis
        Quaternion finalRotation = firePoint.rotation * rotationDeviation;

        // Instantiate the projectile with the final rotation
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, finalRotation);
        
        // Here, you can set up the projectile's behavior further, such as its homing target or velocity
        RangerProjectile projectileScript = projectile.GetComponent<RangerProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(player); // Initialize the projectile with the player's transform
        }
    }
    void DrawFiringCone()
    {
        int numberOfRays = 10; // Number of rays to draw within the cone
        float step = coneAngle / numberOfRays; // Angle step between each ray

        for (float angle = -coneAngle / 2; angle <= coneAngle / 2; angle += step)
        {
            Quaternion rotationDeviation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotationDeviation * firePoint.forward; // Apply deviation to the forward direction
            Debug.DrawRay(firePoint.position, direction * 10, Color.yellow); // Draw ray in the editor
        }
    }
    void CorrectModelOrientation()
{
    // Assuming the model is a child of the Ranger GameObject
    Transform modelTransform = transform; // Get the model transform; adjust the index if necessary

    // Set the model's local rotation to be corrected by -90 degrees on the X axis
    modelTransform.localEulerAngles = new Vector3(90, modelTransform.localEulerAngles.y, modelTransform.localEulerAngles.z);
}
public void OnDefeat()
    {
        // Other defeat logic...
        FindObjectOfType<LevelManager>().EnemyDefeated();
    }

    
}

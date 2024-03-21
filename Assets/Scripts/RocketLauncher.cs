using System.Collections;
using UnityEngine;

public class RocketLauncher : Gun
{
    
    public GameObject shootEffect;
    public RectTransform reticleUI;
    public float knockbackRadius = 10f; // Radius of the knockback effect
    public float knockbackForce = 700f;
    
    //private bool gunEnabled = false;

    protected override void Start()
    {
        base.Start();
        GunTransform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    protected override void Update()
    {
        if (Gunenabled)
        {
        Aiming();
        }
        switch (state)
        {
            case GunState.Ready:
                if (!isReloading) // Add this check to prevent shooting while reloading
                {
                HandleShooting();
                }
                break;
            case GunState.Reloading:
            
                StartCoroutine(Reload());
                break;
        }
        

            if (state == GunState.Ready && ShouldReload())
            {
                state = GunState.Reloading;
            }
    }

    

    protected override void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && !isReloading)
        {
            Shoot();
        }
        if(Input.GetButtonDown("Fire2") && currentAmmo > 0 && !isReloading)
        {
            AltShoot();
        }
    }
    protected override void Shoot()
    {
        base.Shoot();
        GameObject explosion = Instantiate(shootEffect, firePoint.position, Quaternion.identity);
        Destroy(explosion, 1f);
    }
    private void AltShoot()
    {
        // Instantiate the explosion effect at the fire point
        GameObject explosionEffect = Instantiate(shootEffect, firePoint.position, Quaternion.identity);
        Destroy(explosionEffect, 2f); // Destroy the effect after 2 seconds

        // Find all colliders within the knockback radius
        Collider[] colliders = Physics.OverlapSphere(firePoint.position, knockbackRadius);
        foreach (Collider hit in colliders)
        {

            CharacterController characterController = hit.GetComponent<CharacterController>();

            if (characterController != null)
            {
            // Calculate the direction of the force
            Vector3 direction = (characterController.transform.position - firePoint.position).normalized;
            
            // upward force
            direction.y += 0.5f;
            
            // Calculate the strength of the force based on distance from the explosion center
            float distance = Vector3.Distance(firePoint.position, characterController.transform.position);
            float force = Mathf.Lerp(knockbackForce, 0, distance / knockbackRadius);
            
            
            PlayerMovement playerMovement = characterController.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyExternalForce(direction * force); // Implement this method in your PlayerMovement script
            }
            }
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log("Applying knockback force to " + knockbackForce + rb.gameObject.name);
                // Apply knockback force to each rigidbody
                rb.AddExplosionForce(knockbackForce, firePoint.position, knockbackRadius, 5f, ForceMode.Impulse);
            }
        

        currentAmmo--; // Consume one ammo
        UpdateAmmoDisplay(); // Update the ammo display
    }
    }   
}
using System.Collections;
using UnityEngine;

public class ShotGun : Gun
{
    
    public GameObject shootEffect;
    public RectTransform reticleUI;
   

    private Coroutine reloadCoroutine;
    
    //private bool gunEnabled = false;

    protected override void Start()
    {
        maxAmmo = 8;
        base.Start();
        GunTransform.localRotation = Quaternion.Euler(180, 0, 270);
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
                reloadCoroutine = StartCoroutine(Reload());
                break;
        }
        

            if (state == GunState.Ready && ShouldReload())
            {
                if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine); // Stop any existing reload coroutine
            }
        
            state = GunState.Reloading;
            }
    }



    protected override void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
            UpdateAmmoDisplay();
        }
    }

    protected override void Shoot()
    {
        //base.Shoot();

    int pelletsCount = 10; // Number of pellets in a shotgun blast
    float spreadAngle = 4f;
    float horizspreadAngle = 1.5f; // Max angle variation for the spread
    float bulletSpeed = 20f; // Speed at which each pellet moves
    float bulletLifeTime = 3f; // How long the bullet exists before being destroyed

    for (int i = 0; i < pelletsCount; i++)
    {
        // Create a rotation with a random spread based on the spreadAngle
        float spreadX = Random.Range(-horizspreadAngle, horizspreadAngle);
        float spreadY = Random.Range(-spreadAngle, spreadAngle);
        Quaternion pelletRotation = Quaternion.Euler(firePoint.eulerAngles.x + spreadX, firePoint.eulerAngles.y + spreadY, 0);

        // Instantiate the bullet
        GameObject pellet = Instantiate(bulletPrefab, firePoint.position, pelletRotation);

        // If your bullet has a Rigidbody component, you can use it to set the velocity
        Rigidbody rb = pellet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = pellet.transform.forward * bulletSpeed;
        }

        // Optionally, destroy the bullet after some time to simulate limited range
        Destroy(pellet, bulletLifeTime);
    }

    // Instantiate the shoot effect (muzzle flash, smoke, etc.)
    GameObject explosion = Instantiate(shootEffect, firePoint.position, Quaternion.identity);
    Destroy(explosion, 1f);

    // Decrease ammo count
    currentAmmo--;
    UpdateAmmoDisplay();
    }

  

}

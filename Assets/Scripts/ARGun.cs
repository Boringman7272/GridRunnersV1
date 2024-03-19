using System.Collections;
using UnityEngine;

public class ARGun : Gun
{
    public GameObject shootEffect;
    public RectTransform reticleUI;
    private float fireRate = 7f; // Bullets per second
    private float lastShotTime;
    private float burstFireRate = 15f;
    private int burstCount = 5; // Number of bullets per burst
    
    private float burstCooldown = 5f; // Cooldown time between bursts in seconds
    private int shotsFiredInBurst = 0; // Shots fired in the current burst
    private float lastBurstShotTime; 
    
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
            continousburst();
    }

    

    protected override void HandleShooting()
    {
        if (Input.GetButton("Fire1") && currentAmmo > 0 && Time.time >= lastShotTime + 1f / fireRate)
    {
        Shoot();
        lastShotTime = Time.time; // Update the time of the last shot
    }
        if (Input.GetButtonDown("Fire2") && currentAmmo >= burstCount && shotsFiredInBurst == 0 && Time.time >= lastBurstShotTime + burstCooldown)
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
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        currentAmmo--;
        
        shotsFiredInBurst++;
        UpdateAmmoDisplay();
    }

    public void continousburst(){
        
        if (shotsFiredInBurst > 0 && Time.time >= lastBurstShotTime + 1f / burstFireRate )
        {
            if (shotsFiredInBurst < burstCount)
            {
                AltShoot();
            }
            else
            {
                lastBurstShotTime = Time.time;
                shotsFiredInBurst = 0; // Reset burst counter after completing a burst
            }
        }
    }
    }



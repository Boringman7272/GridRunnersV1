using System.Collections;
using UnityEngine;

public class RocketLauncher : Gun
{
    
    public GameObject shootEffect;
    public RectTransform reticleUI;
    
    //private bool gunEnabled = false;

    protected override void Start()
    {
        base.Start();
        GunTransform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    protected override void Update()
    {
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

    void LateUpdate()
    {
        if (Gunenabled)
        {
        Aiming();
        }
    }

    protected override void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && !isReloading)
        {
            Shoot();
        }
    }
    protected override void Shoot()
    {
        base.Shoot();
        GameObject explosion = Instantiate(shootEffect, firePoint.position, Quaternion.identity);
        Destroy(explosion, 1f);
    }

    
}
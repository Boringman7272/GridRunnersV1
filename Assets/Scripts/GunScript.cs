using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform firePoint; // The point from which bullets are fired
    public GameObject bulletPrefab; // The bullet prefab
    public int maxAmmo = 20;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    private enum GunState { Ready, Reloading }
    private GunState state = GunState.Ready;
    public GameObject shootEffect;

    void Start()
    {
        currentAmmo = maxAmmo; // Initialize ammo on start
    }

    void Update()
    {
        switch (state)
        {
            case GunState.Ready:
                HandleShooting();
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

    private void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
        }
    }

    private bool ShouldReload()
    {
        return currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R);
    }

    void Shoot()
    {
        // Instantiate bullet at the fire point
        GameObject explosion = Instantiate(shootEffect, firePoint.position, Quaternion.identity);
        Destroy(explosion, 1f);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        currentAmmo--;
    }

    IEnumerator Reload()
    {
        state = GunState.Reloading;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        state = GunState.Ready;
    }
}

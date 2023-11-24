using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Transform firePoint; // The point from which bullets are fired
    public Transform GunTransform;
    public GameObject bulletPrefab; // The bullet prefab
    public int maxAmmo = 20;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    private enum GunState { Ready, Reloading }
    private GunState state = GunState.Ready;
    public GameObject shootEffect;
    public RectTransform reticleUI; 
    public Camera playerCamera;

    void Start()
    {
        GunTransform.localRotation = Quaternion.Euler(-90, 0, 0);
        currentAmmo = maxAmmo; // Initialize ammo on start
    }

    void Update()
    {

        Aiming();
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
    void Aiming()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // If the ray doesn't hit anything, use a point far away in the direction of the ray
        targetPoint = ray.GetPoint(1000);
        }
        
        Debug.DrawLine(playerCamera.transform.position, targetPoint, Color.red);
        GunTransform.LookAt(targetPoint);
        Vector3 targetDirection = targetPoint - GunTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        GunTransform.rotation = targetRotation * Quaternion.Euler(-90, 0, 0);
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

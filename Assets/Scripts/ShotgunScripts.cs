using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : MonoBehaviour
{
    public Transform firePoint; // The point from which bullets are fired
    public Transform ShotGunTransform;
    public GameObject bulletPrefab; // The bullet prefab
    public int maxAmmoShotgun = 20;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    private enum GunState { Ready, Reloading }
    private GunState state = GunState.Ready;
    public GameObject shootEffect;
    public RectTransform reticleUI; 
    public Camera playerCamera;
    private bool Gunenabled = false;



    void Start()
    {
        if (Gunenabled == true){
        ShotGunTransform.localRotation = Quaternion.Euler(0, 161, 90);
        currentAmmo = maxAmmoShotgun; // Initialize ammo on start
    }
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

void LateUpdate(){

    if (Gunenabled == true){
        Aiming();
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
        int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Particles")) | (1 << LayerMask.NameToLayer("UI"));
        layerMask = ~layerMask; 
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            float minDistance = 3f;
        if (Vector3.Distance(hit.point, ShotGunTransform.position) > minDistance)
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }
    }
    else
    {
        targetPoint = ray.GetPoint(1000);
    }
        
        Debug.DrawLine(playerCamera.transform.position, targetPoint, Color.red);
        ShotGunTransform.LookAt(targetPoint);
        Vector3 targetDirection = targetPoint - ShotGunTransform.position;
        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        ShotGunTransform.rotation = Quaternion.Slerp(ShotGunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        ShotGunTransform.localRotation = Quaternion.Euler(0, 161, 90);
    // Maintain a specific local rotation offset
        
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
        currentAmmo = maxAmmoShotgun;
        state = GunState.Ready;
    }




    public void EnableGun()
{
    Gunenabled = true;
}
}

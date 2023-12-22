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
    private bool Gunenabled = false;



    void Start()
    {
        if (Gunenabled == true){
        GunTransform.localRotation = Quaternion.Euler(-90, 0, 0);
        currentAmmo = maxAmmo; // Initialize ammo on start
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
        if (Vector3.Distance(hit.point, GunTransform.position) > minDistance)
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
        GunTransform.LookAt(targetPoint);
        Vector3 targetDirection = targetPoint - GunTransform.position;
        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        GunTransform.rotation = Quaternion.Slerp(GunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    // Maintain a specific local rotation offset
        GunTransform.localRotation *= Quaternion.Euler(-90, 0, 0);
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




    public void EnableGun()
{
    Gunenabled = true;
}
}

using System.Collections;
using UnityEngine;

public class ShotGun : Gun
{
    public Transform GunTransform;
    public GameObject shootEffect;
    public RectTransform reticleUI;
    public Camera playerCamera;

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
        switch (state)
        {
            case GunState.Ready:
                HandleShooting();
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

    void LateUpdate()
    {
        if (Gunenabled)
        {
        Aiming();
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
        GunTransform.localRotation *= Quaternion.Euler(180, 0, 270);
}   

}

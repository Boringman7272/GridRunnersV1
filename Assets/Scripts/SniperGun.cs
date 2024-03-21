using System.Collections;
using UnityEngine;

public class SniperGun : Gun
{
    public GameObject shootEffect;
    
    public GameObject hitEffect; // Effect to show where the ray hits
    public RectTransform reticleUI;
    public LayerMask hitLayers;
    public float sniperDamage = 120f;
    public float chargedsniperDamage = 500f;
    public float shotCooldown = 1.2f;
    public float ChargedshotCooldown = 2.5f;
    private bool canShoot = true;
    //private bool gunEnabled = false;
    private Coroutine reloadCoroutine;
    [SerializeField] private LineRenderer lineRenderer;
    protected override void Start()
    {
        maxAmmo = 4;
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
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && canShoot && !isReloading)
        {
            Shoot();
            UpdateAmmoDisplay();
            StartCoroutine(ShootCooldown());
            
        }
        if (Input.GetButtonDown("Fire2") && currentAmmo >= 3 && canShoot && !isReloading)
        {
            AltShoot();
            UpdateAmmoDisplay();
            StartCoroutine(ChargedShootCooldown());
            
        }
    }

    protected override void Shoot()
    {
        Vector3 rayStart = playerCamera.transform.position + playerCamera.transform.forward * 0.3f;
        RaycastHit hit;
        

         if (shootEffect != null)
        {
            Instantiate(shootEffect, firePoint.position, Quaternion.identity);
        }

        // Perform raycast from the center of the camera view
         // Adjust 0.1f as needed
        
        if (Physics.Raycast(rayStart, playerCamera.transform.forward, out hit, Mathf.Infinity, hitLayers))
        {
            StartCoroutine(ShowRaycastLine(firePoint.position, hit.point));

            // Optionally, show hit effect at the point of impact
            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Deal damage if the hit object can take damage
            Shootable shootable = hit.collider.GetComponent<Shootable>();
            if (shootable != null)
            {
                shootable.TakeDamage(sniperDamage);
            }
            else
            {
                 StartCoroutine(ShowRaycastLine(firePoint.position, rayStart + playerCamera.transform.forward * 100f)); // Adjust the max distance as needed
            }
        }

        currentAmmo--;
        UpdateAmmoDisplay();
    
    }

    private void AltShoot()
{
    Vector3 rayStart = playerCamera.transform.position + playerCamera.transform.forward * 0.3f;
    RaycastHit[] hits;

    if (shootEffect != null)
    {
        Instantiate(shootEffect, firePoint.position, Quaternion.identity);
    }

    // Use Physics.RaycastAll to get all hits from the ray
    hits = Physics.RaycastAll(rayStart, playerCamera.transform.forward, Mathf.Infinity, hitLayers);

    // Check if the ray hit any objects
    if (hits.Length > 0)
    {
        // Process all hits
        foreach (RaycastHit hit in hits)
        {
            // Show hit effect at each point of impact
            if (hitEffect != null)
            {
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Deal damage if the hit object can take damage
            Shootable shootable = hit.collider.GetComponent<Shootable>();
            if (shootable != null)
            {
                shootable.TakeDamage(chargedsniperDamage);
            }
        }
        
        // Optionally, show the last hit with the raycast line for visualization
        StartCoroutine(ShowRaycastLine(firePoint.position, hits[hits.Length - 1].point));
    }
    else
    {
        // If nothing was hit, show the ray going off into the distance
        StartCoroutine(ShowRaycastLine(firePoint.position, rayStart + playerCamera.transform.forward * 100f)); // Adjust the max distance as needed
    }

    currentAmmo -= 3;
    UpdateAmmoDisplay();
}
 


IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shotCooldown);
        canShoot = true;
    }
IEnumerator ChargedShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(ChargedshotCooldown);
        canShoot = true;
    }

IEnumerator ShowRaycastLine(Vector3 start, Vector3 end)
{
    lineRenderer.SetPosition(0, start);
    lineRenderer.SetPosition(1, end);
    lineRenderer.enabled = true;

    yield return new WaitForSeconds(0.02f); // How long the line is visible

    lineRenderer.enabled = false;
}
}



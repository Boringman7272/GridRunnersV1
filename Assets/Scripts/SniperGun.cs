using System.Collections;
using UnityEngine;

public class SniperGun : Gun
{
    public Transform GunTransform;
    public GameObject shootEffect;
    
    public GameObject hitEffect; // Effect to show where the ray hits
    public RectTransform reticleUI;
    public Camera playerCamera;
    public LayerMask hitLayers;
    public float sniperDamage = 120f;
    public float shotCooldown = 1.2f;
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
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && canShoot && !isReloading)
        {
            Shoot();
            UpdateAmmoDisplay();
            StartCoroutine(ShootCooldown());
            
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
        
        //Debug.DrawLine(playerCamera.transform.position, targetPoint, Color.red);
        GunTransform.LookAt(targetPoint);
        Vector3 targetDirection = targetPoint - GunTransform.position;
        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        GunTransform.rotation = Quaternion.Slerp(GunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    // Maintain a specific local rotation offset
        GunTransform.localRotation *= Quaternion.Euler(-90, 0, 0);
}   


IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shotCooldown);
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



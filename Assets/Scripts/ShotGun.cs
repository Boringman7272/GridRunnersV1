using System.Collections;
using UnityEngine;

public class ShotGun : Gun
{
    
    public GameObject shootEffect;
    public RectTransform reticleUI;
   
    public LayerMask hitLayers;
    private Coroutine reloadCoroutine;
    public PlayerMovement playerMovement;
    [SerializeField] private LineRenderer lineRenderer;
    
    //private bool gunEnabled = false;

    protected override void Start()
    {
        maxAmmo = 8;
        base.Start();
        GunTransform.localRotation = Quaternion.Euler(0, 180, 0);
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
        if(Input.GetButtonDown("Fire2") && currentAmmo > 0)
        {
            AltShoot();
            UpdateAmmoDisplay();
        }
    }
        protected override void Aiming()
    {
        int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Particles")) | (1 << LayerMask.NameToLayer("UI"));
        layerMask = ~layerMask; 
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            float minDistance = 3f;
            targetPoint = Vector3.Distance(hit.point, GunTransform.position) > minDistance ? hit.point : ray.GetPoint(1000);
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }
        if (Input.GetAxis("Vertical") < 0) 
        {
        
        targetPoint = ray.GetPoint(200); 
        }
        smoothedTargetPoint = Vector3.Lerp(smoothedTargetPoint, targetPoint, Time.deltaTime * 0.5f); // Adjust smoothing speed as needed
        Debug.DrawLine(playerCamera.transform.position, smoothedTargetPoint, Color.red);
        smoothedTargetPoint.y = Mathf.Clamp(smoothedTargetPoint.y, GunTransform.position.y - 1f, GunTransform.position.y + 1f); // Adjust vertical limits
        GunTransform.LookAt(targetPoint);
        Vector3 targetDirection = smoothedTargetPoint - GunTransform.position;
        float rotationSpeed = 5f;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    
        
        
        GunTransform.rotation = Quaternion.Slerp(GunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        
        GunTransform.localRotation *= Quaternion.Euler(0, 180, 0);
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


    private void AltShoot()
    {
        hitLayers = ~LayerMask.GetMask("UI");
        Vector3 rayStart = playerCamera.transform.position + playerCamera.transform.forward * 0.3f;
        RaycastHit hit;
        Debug.Log("RayStart: " + rayStart + ", Direction: " + playerCamera.transform.forward);


        // Perform raycast from the center of the camera view
        if (Physics.Raycast(rayStart, playerCamera.transform.forward, out hit, Mathf.Infinity, hitLayers))
        {
            StartCoroutine(ShowRaycastLine(firePoint.position, hit.point));
        // Access PlayerMovement script attached to the player
        Debug.DrawRay(rayStart, hit.point, Color.green, 2f); // The ray will be green and last for 2 seconds

        // Log the name of the object hit by the raycast
        Debug.Log("Hit: " + hit.collider.gameObject.name);
            PlayerMovement playerMovement = playerCamera.GetComponentInParent<PlayerMovement>();
            if (playerMovement != null)
            {
            // Calculate the force direction towards the hit point
                Vector3 forceDirection = (hit.point - transform.position).normalized;
                float grapplingForceMagnitude = 50f; // Define the force magnitude
                playerMovement.ApplyGrapplingHookForce(forceDirection * grapplingForceMagnitude); // Apply force in the PlayerMovement script
            }
        }
        else
    {
        // If nothing was hit, show the ray going off into the distance and log "No hit"
        Debug.DrawRay(rayStart, rayStart + playerCamera.transform.forward * 100f, Color.red, 2f); // The ray will be red and last for 2 seconds
        Debug.Log("No hit");
    }
        currentAmmo--;
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

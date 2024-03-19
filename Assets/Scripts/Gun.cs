using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public abstract class Gun : MonoBehaviour
{
    public Transform GunTransform;
    public Camera playerCamera;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public int maxAmmo = 30;
    protected int currentAmmo;
    public float reloadTime = 2f;
    protected bool isReloading = false;

    protected enum GunState { Ready, Reloading }
    protected GunState state = GunState.Ready;
    public bool Gunenabled = false;
    public TextMeshProUGUI ammoDisplay;

    protected virtual void Start()
    {
        if (Gunenabled == true){
        currentAmmo = maxAmmo; // Initialize ammo on start
    }
    UpdateAmmoDisplay(); 
    }

    protected virtual void Update()
    {
        if (state == GunState.Ready)
        {
            Debug.Log("Gun Is Ready");
            UpdateAmmoDisplay(); 
            if (!isReloading) // Add this check to prevent shooting while reloading
        {
            HandleShooting();
        }
            
        }

        if (state == GunState.Ready && ShouldReload() && isReloading == false)
        {
            Debug.Log("Reloading Coroutine called");
            state = GunState.Reloading;
            if(isReloading == false){
                StartCoroutine(Reload());
            }
            isReloading = true;
            
        }
    }

    protected abstract void HandleShooting();

    protected virtual bool ShouldReload()
    {
        return currentAmmo <= 0 || Input.GetKeyDown(KeyCode.R);
    }

    protected virtual void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        currentAmmo--;
        UpdateAmmoDisplay(); 
    }

    protected IEnumerator Reload()
    {
        if (isReloading) // Early exit if already reloading
            yield break;

        Debug.Log("Started Coroutine");
        isReloading = true; // Set isReloading to true at the start of the coroutine
        state = GunState.Reloading; // Ensure state is set to Reloading here to prevent re-entry
        ammoDisplay.text = "Reloading";
    
        yield return new WaitForSeconds(reloadTime);
    
        currentAmmo = maxAmmo;
        UpdateAmmoDisplay();
        state = GunState.Ready; // Reset state back to Ready
        isReloading = false; // Reset isReloading flag
        Debug.Log("Finished Reloading");
}
    protected void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null) // Check if the text component is assigned and the gun is ready
        {
            ammoDisplay.text = "Ammo: " + currentAmmo + "/" + maxAmmo; // Update the text to show current ammo
        }
    }
    public void DeactivateGun()
    {
        // Disable the gun mechanics
        Gunenabled = false;

        // Optionally hide the gun model
        this.gameObject.SetActive(false);

        // Clear the UI for this gun
        if (ammoDisplay != null)
        {
            ammoDisplay.text = "Empty";
        }
    }
    public void ActivateGun(TextMeshProUGUI uiElement)
    {
        // Assign the UI Text element to this gun
        this.ammoDisplay = uiElement;

        // Enable the gun mechanics
        EnableGun();

        // Update the UI for this gun
        UpdateAmmoDisplay();
    }
    public Vector3 smoothedTargetPoint;
    protected virtual void Aiming()
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

        
        GunTransform.localRotation *= Quaternion.Euler(-90, 0, 0);
}
    public virtual void EnableGun()
    {
        Gunenabled = true;
        this.enabled = true;
        gameObject.SetActive(true);
        UpdateAmmoDisplay();
        this.Start();
    }
}

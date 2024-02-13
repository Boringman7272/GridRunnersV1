using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public abstract class Gun : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public int maxAmmo = 20;
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
    public virtual void EnableGun()
    {
        Gunenabled = true;
        this.enabled = true;
        gameObject.SetActive(true);
        UpdateAmmoDisplay();
        this.Start();
    }
}

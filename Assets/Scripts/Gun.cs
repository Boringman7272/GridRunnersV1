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
    public static TextMeshProUGUI ammoDisplay;

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
            HandleShooting();
            UpdateAmmoDisplay(); 
        }

        if (state == GunState.Ready && ShouldReload())
        {
            
            StartCoroutine(Reload());
            state = GunState.Reloading;
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
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        state = GunState.Ready;
        ammoDisplay.text = "Reloading";
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
        ammoDisplay = uiElement;

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

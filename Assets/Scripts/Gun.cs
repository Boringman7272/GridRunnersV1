using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public Text ammoDisplay;

    protected virtual void Start()
    {
        if (Gunenabled == true){
        currentAmmo = maxAmmo; // Initialize ammo on start
    }
    }

    protected virtual void Update()
    {
        if (state == GunState.Ready)
        {
            HandleShooting();
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
    }

    protected IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        state = GunState.Ready;
    }
    public virtual void EnableGun()
    {
        Gunenabled = true;
    }
}

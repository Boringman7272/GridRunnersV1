using System.Collections;
using UnityEngine;

public class RocketLauncher : Gun
{
    public Transform GunTransform;
    public GameObject shootEffect;
    public RectTransform reticleUI;
    public Camera playerCamera;
    //private bool gunEnabled = false;

    protected override void Start()
    {
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
                StartCoroutine(Reload());
                break;
        }
        

            if (state == GunState.Ready && ShouldReload())
            {
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
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && !isReloading)
        {
            Shoot();
        }
    }
    protected override void Shoot()
    {
        base.Shoot();
        GameObject explosion = Instantiate(shootEffect, firePoint.position, Quaternion.identity);
        Destroy(explosion, 1f);
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
}
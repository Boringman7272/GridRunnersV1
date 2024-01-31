using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    private Gun currentGun;
    public Transform weaponSocket;
    public TextMeshProUGUI ammoDisplay;
    public void PickupGun(GameObject gunPrefab)
    {
        
        if (currentGun != null)
        {
            // Handle dropping or switching the current gun
            // For simplicity, we'll just destroy the current gun
            currentGun.DeactivateGun();
            Destroy(currentGun);
        }

        // Instantiate the gun prefab and attach it to the player
        GameObject gunObject = Instantiate(gunPrefab, weaponSocket.position, Quaternion.identity);
        gunObject.transform.SetParent(weaponSocket); // Set the player as the gun's parent
        gunObject.transform.localPosition = Vector3.zero; // Adjust the position relative to the player
        gunObject.transform.localRotation = Quaternion.identity; // Adjust the rotation relative to the player

        Collider gunCollider = gunObject.GetComponent<Collider>();
        if (gunCollider != null)
        {
            gunCollider.isTrigger = false;
        }
    

        Gun gunScript = gunObject.GetComponent<Gun>();
        if (gunScript != null)
        {
            gunScript.EnableGun(); // Enable the gun script
            gunScript.ActivateGun(ammoDisplay); // Activate the gun and assign UI element
        }

        currentGun = gunScript;
    }
}

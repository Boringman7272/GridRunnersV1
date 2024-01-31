using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private GameObject currentGun;
    public Transform weaponSocket;
    public void PickupGun(GameObject gunPrefab)
    {
        if (currentGun != null)
        {
            // Handle dropping or switching the current gun
            // For simplicity, we'll just destroy the current gun
            Destroy(currentGun);
        }

        // Instantiate the gun prefab and attach it to the player
        GameObject gun = Instantiate(gunPrefab, weaponSocket.position, Quaternion.identity);
        gun.transform.SetParent(weaponSocket); // Set the player as the gun's parent
        gun.transform.localPosition = Vector3.zero; // Adjust the position relative to the player
        gun.transform.localRotation = Quaternion.identity; // Adjust the rotation relative to the player

        Collider gunCollider = gun.GetComponent<Collider>();
        if (gunCollider != null)
        {
            gunCollider.isTrigger = false;
        }
    

        currentGun = gun;

        // You may want to disable the gun script until it's equipped
        Gun gunScript = gun.GetComponent<Gun>();
        if (gunScript != null)
        {
            gunScript.EnableGun();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunpickup : MonoBehaviour
{
    public GameObject gunPrefab; // The gun object to equip

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Assuming the player has a script with a method to handle picking up the gun
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.PickupGun(gunPrefab);
                // Optionally, deactivate the gun object in the scene
                Collider collider = GetComponent<Collider>();
                if (collider != null)
                {
                    collider.isTrigger = false;
                    GetComponent<Collider>().enabled = false;
                };
                gameObject.SetActive(false);
            }
            
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DoorLevelComp : MonoBehaviour
{

    
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            
            FindObjectOfType<LevelManager>().doorPassed = true;
            FindObjectOfType<LevelManager>().CheckLevelCompletion();
        }
    }


}
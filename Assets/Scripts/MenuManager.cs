using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelectionMenu;

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        levelSelectionMenu.SetActive(false);
    }

    public void ShowLevelSelectionMenu()
    {
        mainMenu.SetActive(false);
        levelSelectionMenu.SetActive(true);
    }

    public void StartDevLevel()
    {
        SceneManager.LoadScene("Dev"); // Replace "Dev" with the exact name of your scene
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    
}

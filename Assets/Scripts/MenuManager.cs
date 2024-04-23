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
        Cursor.visible = true;  // Make the cursor visible
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(true);
        levelSelectionMenu.SetActive(false);
        
    }

    public void ShowLevelSelectionMenu()
    {
        Cursor.visible = true;  // Make the cursor visible
        Cursor.lockState = CursorLockMode.None;
        mainMenu.SetActive(false);
        levelSelectionMenu.SetActive(true);
        
    }

    public void StartDevLevel()
    {
        SceneManager.LoadScene("Dev"); 
    }
    public void StartMenus()
    {
        SceneManager.LoadScene("Menus"); 
        Cursor.visible = true;  // Make the cursor visible
        Cursor.lockState = CursorLockMode.None;
    }
    public void StartLevelOne()
    {
        SceneManager.LoadScene("Level1"); 
    }
    public void StartLevelTwo()
    {
        SceneManager.LoadScene("Level2"); 
    }
    public void StartLevelThree()
    {
        SceneManager.LoadScene("Level3"); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    
}

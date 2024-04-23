using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreboardMenuManager : MonoBehaviour
{
        public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menus"); // Replace "Dev" with the exact name of your scene
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    
}

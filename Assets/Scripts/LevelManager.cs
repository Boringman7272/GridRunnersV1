using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public float elapsedTime { get; private set; } // Elapsed time since level start
    private float startTime;
    public int activeEnemies;
    private bool levelCompleted = false;
    public bool doorPassed = false;
    public GameObject scoreboardPanel;
    public GameObject otherUIElements;
    public TextMeshProUGUI completionTimeText;
    public TextMeshProUGUI playerNameText;
    public int LevelNumber = 1; 
    public string playerName = "SNOWY";
    public GameObject gameplayUI;

    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        startTime = Time.time; // Record the start time
    }

    void Update()
    {
        // Update elapsed time only if the level is ongoing
        if (!levelCompleted)
        {
            elapsedTime = Time.time - startTime;
        }
    }
    public void EnemyDefeated()
    {
        activeEnemies--;
        CheckLevelCompletion();
    }

    public void CheckLevelCompletion()
    {
        // Check if all enemies are defeated and the player has passed through the door
        if(activeEnemies <= 0 && doorPassed)
        {
            CompleteLevel();
        }
    }
    

    public void CompleteLevel()
    {
        Debug.Log("Level completed, showing scoreboard.");
        HideOtherUI(); // Hide all other UI elements
        SlowDownGame();
        levelCompleted = true; // Mark level as completed to stop the timer
        elapsedTime = Time.time - startTime; // Final time calculation
        ShowScoreboard(elapsedTime); // Display the scoreboard
    }

    public void ShowScoreboard(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        completionTimeText.text = $"Time: {formattedTime}";
        playerNameText.text = $"Player: {PlayerPrefs.GetString("PlayerName", "Default")}";
    
        scoreboardPanel.SetActive(true);
    }
    public void SaveLevelTime(float time, int levelNumber)
    {
        PlayerPrefs.SetFloat($"Level{levelNumber}Time", time);
        // Assuming you have some way to set or get the playerName
        PlayerPrefs.SetString("PlayerName", playerName);
    }

    public void LoadLevelTime(int levelNumber)
    {
        float levelTime = PlayerPrefs.GetFloat($"Level{LevelNumber}Time", 0);
        string playerName = PlayerPrefs.GetString("PlayerName", "DefaultPlayer");
        // Use these values to display on the UI or for other logic
    }

    public void SetPlayerName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
    }
    public void HideOtherUI()
    {
        gameplayUI.SetActive(false);
    }
    

    public void SlowDownGame()
    {
    StartCoroutine(SlowDownRoutine());
    }

    IEnumerator SlowDownRoutine()
{
    while (Time.timeScale > 0.1f)
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, 0.1f); // Adjust the lerp speed as needed
        yield return null; // Wait for the next frame
    }

    Time.timeScale = 0; // Pause the game
}

public void RestartLevelOrGoToNext()
{
    Time.timeScale = 1; // Resume normal game speed
    // Your code to restart the level or go to the next one
}

public void GoToMenu()
    {
        SceneManager.LoadScene("Menus"); // Replace "Dev" with the exact name of your scene
    }


}
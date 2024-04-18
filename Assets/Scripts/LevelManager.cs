using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;


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
    public GameObject EntryPrefab; // Assign in the inspector
    public Transform playerlistParent; // Parent GameObject where time entries will be added
    public Transform timelistParent;
    public GameObject completionPopup;
    private List<float> levelTimes = new List<float>();
    private List<string> levelNames = new List<string>();

    void Start()
    {
        StartLevel();
    }

    
    public void StartLevel()
    {
        startTime = Time.time; // Record the start time
    }

    public void AddTimeToList(float time)
    {
        // Add time to the list
        levelTimes.Add(time);

        // Instantiate a new time entry prefab and set it as a child of timeListParent
        GameObject newTimeEntry = Instantiate(EntryPrefab, timelistParent);

        // Get the Text component of the new entry and set its text to the formatted time
        TextMeshProUGUI timeText = newTimeEntry.GetComponent<TextMeshProUGUI>();
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }

    public void AddNameToList(string name)
    {
        // Add Name to the list
        
        levelNames.Add(name);

        // Instantiate a new name entry prefab and set it as a child of playerlistParent
        GameObject newNameEntry = Instantiate(EntryPrefab, playerlistParent);

        // Get the Text component of the new entry and set its text to the formatted time
        TextMeshProUGUI nameText = newNameEntry.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = $"Player: {PlayerPrefs.GetString("PlayerName", "Default")}";;
        }
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
        if (activeEnemies <= 0){
            completePopup();
        }
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
        name = $"Player: {PlayerPrefs.GetString("PlayerName", "Default")}";
        AddTimeToList(elapsedTime);
        AddNameToList(PlayerPrefs.GetString("PlayerName", "Default"));
        ShowScoreboard(elapsedTime); // Display the scoreboard
        Debug.Log("Scoreboard shown should be called");
    }

    public void ShowScoreboard(float time)
    {
        Debug.Log("Sscoreboard called");
        Cursor.visible = true;

        // Unlock the cursor, allowing the player to move it freely
        Cursor.lockState = CursorLockMode.None;
        //int minutes = Mathf.FloorToInt(time / 60F);
        //int seconds = Mathf.FloorToInt(time - minutes * 60);
        //string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        //completionTimeText.text = $"Time: {formattedTime}";
        //playerNameText.text = $"Player: {PlayerPrefs.GetString("PlayerName", "Default")}";
    
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
private void completePopup(){
    
    
    completionPopup.SetActive(true); // Show the popup
            


}

public void GoToMenu()
    {
        SceneManager.LoadScene("Menus"); // Replace "Dev" with the exact name of your scene
    }


}
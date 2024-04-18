using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class PlayerTimeEntry
{
    public string playerName;
    public float time;
    public int levelNumber; 
}

[Serializable]
public class PlayerTimeList
{
    public List<PlayerTimeEntry> playerTimes = new List<PlayerTimeEntry>();
}

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
    public int levelNumber = 1; 
    public string playerName = "SNOWY";
    public GameObject gameplayUI;
    public GameObject EntryPrefab; // Assign in the inspector
    public Transform playerlistParent; // Parent GameObject where time entries will be added
    public Transform timelistParent;
    public GameObject completionPopup;
    private List<float> levelTimes = new List<float>();
    private List<string> levelNames = new List<string>();
    private string filePath;
    private PlayerTimeList playerTimes = new PlayerTimeList();
    public Transform timesListParent; // Parent for dynamically created time entries
    public GameObject timeEntryPrefab;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerTimes.json");
        LoadTimes();
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
        
        HideOtherUI(); // Hide all other UI elements
        elapsedTime = Time.time - startTime;
        playerTimes.playerTimes.Add(new PlayerTimeEntry { playerName = playerName, time = elapsedTime });
        SaveTimes();
        SavePlayerTime(elapsedTime);
        ShowScoreboard();
        SlowDownGame();
        levelCompleted = true;
    }

    private void ShowScoreboard()
{

     Cursor.visible = true;  // Make the cursor visible
    Cursor.lockState = CursorLockMode.None;
    
    int currentLevel = levelNumber; // Current level identifier
    foreach (Transform child in timesListParent.transform)
    {
        Destroy(child.gameObject); // Clear previous entries
    }

    foreach (var entry in playerTimes.playerTimes)
    {
        if (entry.levelNumber == currentLevel)  // Filter to show only current level times
        {
            GameObject go = Instantiate(timeEntryPrefab, timesListParent);
            go.GetComponent<TextMeshProUGUI>().text = $"{entry.playerName}: {entry.time:0.00} (Level {entry.levelNumber})";
        }
    }

    scoreboardPanel.SetActive(true);
}


     private void LoadTimes()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            playerTimes = JsonUtility.FromJson<PlayerTimeList>(json);
        }
    }

    private void SaveTimes()
    {
        string json = JsonUtility.ToJson(playerTimes);
        File.WriteAllText(filePath, json);
    }
    

    public void SetPlayerName(string newName)
    {
        playerName = newName;
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
    
}
private void completePopup(){
    
    
    completionPopup.SetActive(true); // Show the popup
            


}
public void SavePlayerTime(float time)
{
    string playerName = PlayerPrefs.GetString("PlayerName", "DefaultPlayer");
    int currentLevel = levelNumber; // Assume this is set based on the level being played
    SaveToJson(playerName, time, currentLevel);
}


private void SaveToJson(string playerName, float time, int levelNumber)
{
    PlayerTimeEntry newEntry = new PlayerTimeEntry
    {
        playerName = playerName,
        time = time,
        levelNumber = levelNumber  // Save level number with the time entry
    };

    playerTimes.playerTimes.Add(newEntry);
    string json = JsonUtility.ToJson(playerTimes);
    File.WriteAllText(filePath, json);
}


public void GoToMenu()
    {
        SceneManager.LoadScene("Menus"); // Replace "Dev" with the exact name of your scene
    }


}
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float elapsedTime { get; private set; } // Elapsed time since level start
    private float startTime;
    public int activeEnemies;
    private bool levelCompleted = false;
    public bool doorPassed = false;
    public GameObject scoreboardPanel;
    public Text timeText;

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
        levelCompleted = true; // Mark level as completed to stop the timer
        elapsedTime = Time.time - startTime; // Final time calculation
        ShowScoreboard(elapsedTime); // Display the scoreboard
    }

    public void ShowScoreboard(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        timeText.text = niceTime; // Update the UI element
        scoreboardPanel.SetActive(true); // Show the scoreboard
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
}
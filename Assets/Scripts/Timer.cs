using UnityEngine;
using TMPro;  // Make sure to include this if using TextMeshPro

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Reference to the Text element
    private float startTime;
    private bool timerActive = false;

    void Start()
    {
        // Optionally start the timer on start or set timerActive to true when needed
        StartTimer();
    }

    public void StartTimer()
    {
        startTime = Time.time;
        timerActive = true;
    }

    void Update()
    {
        if (timerActive)
        {
            float t = Time.time - startTime;

            string minutes = ((int)t / 60).ToString("00");
            string seconds = (t % 60).ToString("00");

            timerText.text = minutes + ":" + seconds;
        }
    }

    public void StopTimer()
    {
        timerActive = false;
    }
}

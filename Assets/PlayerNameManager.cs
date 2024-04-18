using UnityEngine;
using TMPro;
using UnityEngine.UI; // Make sure you have the TextMeshPro namespace


public class PlayerNameManager : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public Button submitButton;

    void Start()
    {
        // Add a listener to your submit button to call the SubmitName function when clicked
        submitButton.onClick.AddListener(SubmitName);
    }

    private void SubmitName()
    {
        string playerName = nameInputField.text;
        if (!string.IsNullOrWhiteSpace(playerName))
        {
            PlayerPrefs.SetString("PlayerName", playerName);  // Save the name using PlayerPrefs or your preferred method
            Debug.Log("Player name set to: " + playerName);
        }
        else
        {
            Debug.Log("Player name is empty or only whitespace. Not setting name.");
        }
    }
}

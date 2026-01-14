using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    // Time conversion constants (in milliseconds)
    private const int millisecondsPerMinute = 60000;
    private const int millisecondsPerSecond = 1000;

    // List of times for 1…4 stars (in milliseconds) per level
    public Dictionary<string, List<int>> timesDict = new Dictionary<string, List<int>>();

    private string currentLevelName;
    private bool hasNextLevel = false;

    void Awake()
    {
        // Time thresholds for stars (in milliseconds) - faster = more stars
        timesDict["Tutorial"] = new List<int> { 75000, 55000, 45000, 30000 };
        timesDict["Level 1"] = new List<int> { 60000, 45000, 35000, 30000 };
        timesDict["Level 2"] = new List<int> { 90000, 70000, 55000, 45000 };
        timesDict["Level 3"] = new List<int> { 120000, 90000, 60000, 45000 };
    }

    // Initializes win menu display with level completion data
    public void Initialize(string levelName, int timeInMilliseconds)
    {
        currentLevelName = levelName;
        DisplayTime(timeInMilliseconds);

        int stars = CalculateStars(levelName, timeInMilliseconds);
        DisplayNextStar(stars, levelName);
        SetStarColors(stars);

        // Save progress to GameSaveManager
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.CompleteLevel(levelName, timeInMilliseconds, stars);
        }

        // Check if there's a next level
        hasNextLevel = GetNextLevelName() != null;
        SetupNextLevelButton();

        Cursor.lockState = CursorLockMode.None; // Unlocks the cursor
        Cursor.visible = true;                  // Makes the cursor visible
    }

    // Displays the elapsed time in MM:SS:mmm format
    void DisplayTime(int timeInMilliseconds)
    {
        int minutes = timeInMilliseconds / millisecondsPerMinute;
        int seconds = (timeInMilliseconds % millisecondsPerMinute) / millisecondsPerSecond;
        int milliseconds = timeInMilliseconds % millisecondsPerSecond;

        string timeText = string.Format("{0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);

        Transform panelTransform = transform.GetChild(0); // Panel
        Transform timeTransform = panelTransform.Find("Time");
        if (timeTransform != null)
        {
            TextMeshProUGUI textComponent = timeTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
                textComponent.text = timeText;
        }
    }

    // Calculates number of stars earned based on time threshold
    int CalculateStars(string levelName, int timeInMilliseconds)
    {
        if (!timesDict.ContainsKey(levelName))
        {
            Debug.LogWarning($"Level '{levelName}' not found in timesDict!");
            return 0;
        }

        List<int> thresholds = timesDict[levelName];
        int stars = 0;

        for (int i = 0; i < thresholds.Count; i++)
        {
            if (timeInMilliseconds <= thresholds[i])
                stars++;
        }
        Debug.Log(stars);

        return stars;
    }

    // Displays the time required for the next star threshold
    public void DisplayNextStar(int starsEarned, string levelName)
    {
        // Get panel
        if (transform.childCount == 0)
            return;

        Transform panelTransform = transform.GetChild(0); // Panel
        Transform nextTimeTransform = panelTransform.Find("NextTime");
        if (nextTimeTransform == null)
        {
            Debug.LogWarning("Child 'NextTime' not found under Panel!");
            return;
        }

        TextMeshProUGUI nextTimeText = nextTimeTransform.GetComponent<TextMeshProUGUI>();
        if (nextTimeText == null)
        {
            Debug.LogWarning("TextMeshProUGUI component not found on NextTime!");
            return;
        }

        // Check if level exists in dictionary
        if (!timesDict.ContainsKey(levelName))
        {
            nextTimeText.text = "Time to next star: None";
            return;
        }

        List<int> thresholds = timesDict[levelName];

        // The "next" star is at index starsEarned (0 - based)
        if (starsEarned < thresholds.Count)
        {
            int nextThreshold = thresholds[starsEarned];
            // Convert milliseconds to seconds:minutes if desired
            int minutes = nextThreshold / millisecondsPerMinute;
            int seconds = (nextThreshold % millisecondsPerMinute) / millisecondsPerSecond;
            int milliseconds = nextThreshold % millisecondsPerSecond;

            string formatted = string.Format("{0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);
            nextTimeText.text = "Time to next star: " + formatted;
        }
        else
        {
            nextTimeText.text = "Time to next star: None";
        }
    }

    // Updates star visual colors based on stars earned
    void SetStarColors(int starsEarned)
    {
        Transform panelTransform = transform.GetChild(0); // Panel
        Transform starsParent = panelTransform.Find("Stars");
        if (starsParent == null) return;

        int totalStars = starsParent.childCount;

        for (int i = 4; i > starsEarned; i--)
        {
            Transform star = starsParent.GetChild(4-i);
            if (star == null) continue;

            RawImage starImage = star.GetComponent<RawImage>();
            if (starImage != null)
            {
                starImage.color = (i < starsEarned) ? Color.white : Color.black;
            }
        }
    }

    public void OnReplayClicked()
    {
        // Find the object with the Main script
        Main mainScript = FindObjectOfType<Main>();

        if (mainScript != null)
        {
            string currentLevel = mainScript.level; // Get the current level
            mainScript.loadLevel(currentLevel, true);     // Call LoadLevel with that level
        }
        else
        {
            Debug.LogWarning("Main script not found in the scene!");
        }
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    private string GetNextLevelName()
    {
        string[] levels = GameSaveManager.AvailableLevels;
        for (int i = 0; i < levels.Length - 1; i++)
        {
            if (levels[i] == currentLevelName)
            {
                return levels[i + 1];
            }
        }
        return null; // No next level (last level)
    }

    private void SetupNextLevelButton()
    {
        if (!hasNextLevel) return;

        Transform panelTransform = transform.GetChild(0);

        // Find MainMenu button and replace it with Next Level
        Transform menuBtn = panelTransform.Find("MainMenu");
        if (menuBtn == null) return;

        // Update text to show next level
        TextMeshProUGUI btnText = menuBtn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
        {
            btnText.text = "Next: " + GetNextLevelName();
        }

        // Replace click event
        Button btn = menuBtn.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnNextLevelClicked);
        }
    }

    public void OnNextLevelClicked()
    {
        string nextLevel = GetNextLevelName();
        if (nextLevel == null) return;

        Main mainScript = FindObjectOfType<Main>();
        if (mainScript != null)
        {
            // Use MainMenu singleton
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.OnLevelStarted();
            }

            // Ensure time is running
            Time.timeScale = 1f;

            // Hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Destroy WinMenu
            Destroy(gameObject);

            // Load next level
            mainScript.loadLevel(nextLevel, true);
        }
    }
}

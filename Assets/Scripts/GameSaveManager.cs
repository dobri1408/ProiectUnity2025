using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public string levelName;
    public bool isUnlocked;
    public bool isCompleted;
    public int bestTimeMs;
    public int bestStars;

    public LevelData(string name, bool unlocked = false)
    {
        levelName = name;
        isUnlocked = unlocked;
        isCompleted = false;
        bestTimeMs = -1;
        bestStars = 0;
    }
}

[Serializable]
public class GameSaveData
{
    public List<LevelData> levels = new List<LevelData>();
    public int totalStars;
    public string lastPlayedLevel;
    public float masterVolume;
    public float mouseSensitivity;

    public GameSaveData()
    {
        masterVolume = 1f;
        mouseSensitivity = 300f;
        totalStars = 0;
        lastPlayedLevel = "Tutorial";
    }
}

public class GameSaveManager : MonoBehaviour
{
    private static GameSaveManager _instance;
    public static GameSaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameSaveManager");
                _instance = go.AddComponent<GameSaveManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private const string SAVE_KEY = "GameSaveData";
    public GameSaveData saveData;

    // Define available levels (can be extended)
    public static readonly string[] AvailableLevels = { "Tutorial", "Level1", "Level2", "Level3" };

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
        }
        else
        {
            saveData = new GameSaveData();
            InitializeDefaultLevels();
        }

        // Ensure all levels exist in save data
        EnsureAllLevelsExist();

        // Apply saved settings
        AudioListener.volume = saveData.masterVolume;
    }

    void InitializeDefaultLevels()
    {
        saveData.levels.Clear();
        for (int i = 0; i < AvailableLevels.Length; i++)
        {
            LevelData levelData = new LevelData(AvailableLevels[i], i == 0); // Only first level unlocked
            saveData.levels.Add(levelData);
        }
    }

    void EnsureAllLevelsExist()
    {
        foreach (string levelName in AvailableLevels)
        {
            if (GetLevelData(levelName) == null)
            {
                saveData.levels.Add(new LevelData(levelName, levelName == "Tutorial"));
            }
        }
    }

    public void SaveGame()
    {
        saveData.masterVolume = AudioListener.volume;
        saveData.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 300f);

        string json = JsonUtility.ToJson(saveData, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Game saved!");
    }

    public LevelData GetLevelData(string levelName)
    {
        return saveData.levels.Find(l => l.levelName == levelName);
    }

    public void CompleteLevel(string levelName, int timeMs, int stars)
    {
        LevelData levelData = GetLevelData(levelName);
        if (levelData == null) return;

        levelData.isCompleted = true;

        // Update best time (only if better or first completion)
        if (levelData.bestTimeMs < 0 || timeMs < levelData.bestTimeMs)
        {
            levelData.bestTimeMs = timeMs;
        }

        // Update best stars
        if (stars > levelData.bestStars)
        {
            levelData.bestStars = stars;
        }

        // Unlock next level
        int currentIndex = Array.IndexOf(AvailableLevels, levelName);
        if (currentIndex >= 0 && currentIndex < AvailableLevels.Length - 1)
        {
            string nextLevel = AvailableLevels[currentIndex + 1];
            LevelData nextLevelData = GetLevelData(nextLevel);
            if (nextLevelData != null)
            {
                nextLevelData.isUnlocked = true;
            }
        }

        // Update total stars
        RecalculateTotalStars();

        saveData.lastPlayedLevel = levelName;
        SaveGame();
    }

    void RecalculateTotalStars()
    {
        int total = 0;
        foreach (LevelData level in saveData.levels)
        {
            total += level.bestStars;
        }
        saveData.totalStars = total;
    }

    public bool IsLevelUnlocked(string levelName)
    {
        LevelData data = GetLevelData(levelName);
        return data != null && data.isUnlocked;
    }

    public int GetTotalStars()
    {
        return saveData.totalStars;
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        saveData = new GameSaveData();
        InitializeDefaultLevels();
        SaveGame();
        Debug.Log("All progress reset!");
    }

    public string FormatTime(int timeMs)
    {
        if (timeMs < 0) return "--:--:---";
        int minutes = timeMs / 60000;
        int seconds = (timeMs % 60000) / 1000;
        int milliseconds = timeMs % 1000;
        return string.Format("{0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);
    }
}

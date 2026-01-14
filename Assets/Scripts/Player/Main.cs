using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Main : MonoBehaviour
{
    public string level;
    private MainMenu mainMenu;
    private LoadingScreen loadingScreen;

    public void StartGame()
    {
        loadLevel("Tutorial");
    }

    public void loadLevel(string name, bool forced = false)
    {
        GameObject existingLevel = GameObject.Find(name + "(Clone)");
        if (existingLevel != null && !forced)
        {
            return; // Exit early if level already exists
        }

        // Check if loading screen is busy
        if (loadingScreen != null && loadingScreen.IsLoading())
        {
            return;
        }

        ClearLevel();
        this.level = name;

        // If loading screen is not ready, use synchronous loading
        if (loadingScreen == null)
        {
            LoadLevelSync(name);
            return;
        }

        // Use asynchronous loading
        loadingScreen.LoadLevelAsync(name, (levelPrefab, playerPrefab, uiPrefab) =>
        {
            // Instantiate after everything is loaded
            Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
            GameObject playerInstance = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            
            // Reset player physics to prevent carryover from previous level
            Rigidbody playerRb = playerInstance.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }

            GameObject uiInstance = Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);
            uiInstance.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = name;

            // Set game state after loading
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.OnLevelStarted();
            }
        });
    }

    // Fallback for synchronous loading
    private void LoadLevelSync(string name)
    {
        GameObject levelPrefab = Resources.Load<GameObject>("Levels/" + name);
        Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);

        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        GameObject playerInstance = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        
        // Reset player physics to prevent carryover from previous level
        Rigidbody playerRb = playerInstance.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        GameObject uiPrefab = Resources.Load<GameObject>("UIs/UI");
        GameObject uiInstance = Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);
        uiInstance.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = name;

        // Set game state
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.OnLevelStarted();
        }
    }

    void ClearLevel()
    {
        // First, clear all checkpoints completely
        GameObject checkpoints = GameObject.Find("Checkpoints");
        if (checkpoints != null)
        {
            Destroy(checkpoints);
        }

        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (GameObject obj in rootObjects)
        {
            // Skip the Main script itself
            if (obj == gameObject) continue;

            // Check by tag
            if (obj.CompareTag("Player") ||
                obj.CompareTag("Level")  ||
                obj.CompareTag("UI"))
            {
                toDestroy.Add(obj);
                continue;
            }

            // Also check for level/player/UI clones by name pattern (backup for missing tags)
            if (obj.name.Contains("(Clone)"))
            {
                string lowerName = obj.name.ToLower();
                if (lowerName.StartsWith("tutorial") ||
                    lowerName.StartsWith("level") ||
                    lowerName.StartsWith("player") ||
                    lowerName.StartsWith("ui"))
                {
                    toDestroy.Add(obj);
                    continue;
                }
            }
        }

        // Destroy all marked objects
        foreach (GameObject obj in toDestroy)
        {
            Destroy(obj);
        }
    }

    void enableCheated() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.Find("Body").GetComponent<Player>().cheated = true;
        GameObject.FindGameObjectWithTag("UI").transform.Find("Practice").gameObject.SetActive(true);
    }

    void CreateCheckpoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        enableCheated();

        GameObject root = GetCheckpointRoot();

        GameObject checkpoint = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        checkpoint.name = "Checkpoint";

        // Slight Y offset so it doesn't clip into the ground
        checkpoint.transform.position = player.transform.Find("Body").position + Vector3.up * 0.05f;

        Destroy(checkpoint.GetComponent<Collider>());

        checkpoint.transform.SetParent(root.transform);
    }

    bool TeleportToLastCheckpoint()
    {
        GameObject root = GameObject.Find("Checkpoints");
        if (root == null || root.transform.childCount == 0)
            return false;

        Transform lastCheckpoint = root.transform.GetChild(root.transform.childCount - 1);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        player.transform.Find("Body").position = lastCheckpoint.position;
        return true;
    }

    GameObject GetCheckpointRoot()
    {
        GameObject root = GameObject.Find("Checkpoints");
        if (root == null)
        {
            root = new GameObject("Checkpoints");
        }
        return root;
    }

    void DeleteLastCheckpoint()
    {
        GameObject root = GameObject.Find("Checkpoints");
        if (root == null) return;

        Transform t = root.transform;
        if (t.childCount == 0) return;

        Transform last = t.GetChild(t.childCount - 1);
        Destroy(last.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CreateCheckpoint();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            DeleteLastCheckpoint();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!TeleportToLastCheckpoint())
            {
                // If no checkpoint exists, reload level
                loadLevel(level, true);
            }
        }
    }

    void Start()
    {
        // Create music manager
        GameObject musicObj = new GameObject("MusicManager");
        musicObj.AddComponent<MusicManager>();

        // Create loading screen
        GameObject loadingObj = new GameObject("LoadingScreen");
        loadingScreen = loadingObj.AddComponent<LoadingScreen>();
        DontDestroyOnLoad(loadingObj);

        // Create main menu
        GameObject menuObj = new GameObject("MainMenu");
        mainMenu = menuObj.AddComponent<MainMenu>();
    }
}

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

        // Verifica daca loading screen e ocupat
        if (loadingScreen != null && loadingScreen.IsLoading())
        {
            return;
        }

        ClearLevel();
        this.level = name;

        // Daca loadingScreen nu e gata, foloseste incarcare sincrona
        if (loadingScreen == null)
        {
            LoadLevelSync(name);
            return;
        }

        // Foloseste incarcare asincrona
        loadingScreen.LoadLevelAsync(name, (levelPrefab, playerPrefab, uiPrefab) =>
        {
            // Ascunde meniul principal (in caz ca e vizibil)
            GameObject mainMenuCanvas = GameObject.Find("MainMenuCanvas");
            if (mainMenuCanvas != null)
            {
                mainMenuCanvas.SetActive(false);
            }

            // Instantiaza dupa ce totul s-a incarcat
            Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
            Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);

            GameObject uiInstance = Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);
            uiInstance.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = name;

            // Asigura-te ca jocul ruleaza si cursorul e blocat
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        });
    }

    // Fallback pentru incarcare sincrona
    private void LoadLevelSync(string name)
    {
        GameObject levelPrefab = Resources.Load<GameObject>("Levels/" + name);
        Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);

        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);

        GameObject uiPrefab = Resources.Load<GameObject>("UIs/UI");
        GameObject uiInstance = Instantiate(uiPrefab, Vector3.zero, Quaternion.identity);
        uiInstance.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = name;
    }

    void ClearLevel()
    {
        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.CompareTag("Player") || 
                obj.CompareTag("Level")  || 
                obj.CompareTag("UI"))
            {
                Destroy(obj);
            }

            // Clear all checkpoints but keep the root object
            if (obj.name == "Checkpoints")
            {
                Transform t = obj.transform;
                for (int i = t.childCount - 1; i >= 0; i--)
                {
                    Destroy(t.GetChild(i).gameObject);
                }
            }
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

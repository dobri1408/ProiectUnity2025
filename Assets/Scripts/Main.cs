using UnityEngine;
using TMPro;

public class Main : MonoBehaviour
{
    public string level;
    private MainMenu mainMenu;

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

        ClearLevel();
        this.level = name;

        GameObject level = Resources.Load<GameObject>(name);
        Instantiate(level, Vector3.zero, Quaternion.identity);

        GameObject player = Resources.Load<GameObject>("Player");
        Instantiate(player, new Vector3(0, 1, 0), Quaternion.identity);

        GameObject ui = Resources.Load<GameObject>("UI");
        GameObject uiInstance = Instantiate(ui, Vector3.zero, Quaternion.identity);
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
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            loadLevel(level, true);
        }
    }

    void Start()
    {
        // Create main menu
        GameObject menuObj = new GameObject("MainMenu");
        mainMenu = menuObj.AddComponent<MainMenu>();
    }
}

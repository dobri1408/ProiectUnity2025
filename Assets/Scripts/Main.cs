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

    void loadLevel(string name)
    {
        this.level = name;

        GameObject level = Resources.Load<GameObject>(name);
        Instantiate(level, Vector3.zero, Quaternion.identity);

        GameObject player = Resources.Load<GameObject>("Player");
        Instantiate(player, new Vector3(0, 1, 0), Quaternion.identity);

        GameObject ui = Resources.Load<GameObject>("UI");
        GameObject uiInstance = Instantiate(ui, Vector3.zero, Quaternion.identity);
        uiInstance.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = name;
    }

    void RestartLevel()
    {
        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            // Keep this manager AND the time-of-day object
            if (obj != this.gameObject && obj.tag != "Time")
                Destroy(obj);
        }

        loadLevel(level);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    void Start()
    {
        // Create main menu
        GameObject menuObj = new GameObject("MainMenu");
        mainMenu = menuObj.AddComponent<MainMenu>();
    }
}

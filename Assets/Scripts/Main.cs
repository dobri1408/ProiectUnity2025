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
            loadLevel(level);
        }
    }

    void Start()
    {
        // Create main menu
        GameObject menuObj = new GameObject("MainMenu");
        mainMenu = menuObj.AddComponent<MainMenu>();
    }
}

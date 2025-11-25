using UnityEngine;
using TMPro;

public class Main : MonoBehaviour
{
    public string level;

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
        // Get all root objects in the scene
        GameObject[] rootObjects = gameObject.scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj != this.gameObject) // keep the Main object
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
        loadLevel("Tutorial");
    }
}

using UnityEngine;

public class Main : MonoBehaviour
{
    private GameObject currentLevel;

    void Start()
    {
        GameObject level = Resources.Load<GameObject>("Level");
        currentLevel = Instantiate(level, Vector3.zero, Quaternion.identity);

        GameObject player = Resources.Load<GameObject>("Player");
        Instantiate(player, new Vector3(0, 1, 0), Quaternion.identity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            changeLevel();
        }
    }

    public void changeLevel()
    {
        if (currentLevel != null)
        {
            currentLevel.SetActive(false);
        }

        GameObject level = Resources.Load<GameObject>("Level2");
        Instantiate(level, Vector3.zero, Quaternion.identity);

    }
}

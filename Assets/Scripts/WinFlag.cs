using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    TimerUI timer;

    void Start()
    {
        GameObject timerObject = GameObject.Find("Timer");
        if (timerObject != null)
        {
            timer = timerObject.GetComponent<TimerUI>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (timer == null)
        {
            timer = FindObjectOfType<TimerUI>();
            if (timer == null) return; // Timer still doesn't exist
        }

        timer.StopTimer();
        int elapsedTime = timer.GetElapsedMilliseconds();

        // Get level name from Main manager
        string levelName = "UnknownLevel";
        Main main = FindObjectOfType<Main>();
        if (main != null)
            levelName = main.level;

        GameObject winMenuPrefab = Resources.Load<GameObject>("WinMenu");
        if (winMenuPrefab != null)
        {
            GameObject winMenuInstance = Instantiate(winMenuPrefab);
            WinMenu winMenuScript = winMenuInstance.GetComponent<WinMenu>();
            if (winMenuScript != null)
            {
                winMenuScript.Initialize(levelName, elapsedTime);
            }
        }

        GetComponent<Collider>().enabled = false;
    }

}

using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    TimerUI timer; // Time used for star reward.

    void Start()
    {
        GameObject timerObject = GameObject.Find("Timer");
        if (timerObject != null)
        {
            timer = timerObject.GetComponent<TimerUI>();
        }
    }

    // On player collision
    // Ignore if checkpoints are used.
    // Creates a WinMenu obj and gives level, time as arg
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Player component is on Body child, not root
        Player playerComponent = other.GetComponent<Player>();
        if (playerComponent == null)
            playerComponent = other.GetComponentInChildren<Player>();
        if (playerComponent == null)
            playerComponent = other.GetComponentInParent<Player>();

        if (playerComponent == null) return;

        bool cheated = playerComponent.cheated;
        if(cheated) {
            Debug.Log("Cheated run");            
            return;
        }

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

        GameObject winMenuPrefab = Resources.Load<GameObject>("UIs/WinMenu");
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

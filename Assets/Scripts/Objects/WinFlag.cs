using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    TimerUI timer; // Time used for star reward.
    Main mainScript; // Cached Main instance

    // Initializes the win flag by finding the timer component in the scene.
    void Start()
    {
        GameObject timerObject = GameObject.Find("Timer");
        if (timerObject != null)
        {
            timer = timerObject.GetComponent<TimerUI>();
        }
        mainScript = FindObjectOfType<Main>();
    }

    // Handles player collision with the win flag. Displays the win menu and records level completion time.
    // Ignores if player cheated or if timer component is not found.
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
        if (mainScript != null)
            levelName = mainScript.level;

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

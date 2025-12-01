using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    float t;
    bool active = true;

    void Update()
    {
        if (!active) return;

        t += Time.deltaTime;

        int m = (int)(t / 60);
        int s = (int)(t % 60);
        int ms = (int)((t * 100) % 100);

        GetComponent<TextMeshProUGUI>().text = $"{m:00}:{s:00}.{ms:00}";
    }

    // Call this function to stop the timer
    public void StopTimer()
    {
        active = false;
    }

    public int GetElapsedMilliseconds()
    {
        return Mathf.RoundToInt(t * 1000f);
    }
}

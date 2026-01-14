using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    // Magic numbers as constants
    private const int secondsPerMinute = 60;
    private const int millisecondsMultiplier = 100;
    private const int millisecondsPerSecond = 1000;

    private float t;
    private bool active = true;
    private TextMeshProUGUI textComponent; // Cached for performance

    void Start()
    {
        // Cache TextMeshProUGUI component to avoid GetComponent calls in Update
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!active) return;

        t += Time.deltaTime;

        int m = (int)(t / secondsPerMinute);
        int s = (int)(t % secondsPerMinute);
        int ms = (int)((t * millisecondsMultiplier) % millisecondsMultiplier);

        textComponent.text = $"{m:00}:{s:00}.{ms:00}";
    }

    // Call this function to stop the timer
    public void StopTimer()
    {
        active = false;
    }

    public int GetElapsedMilliseconds()
    {
        return Mathf.RoundToInt(t * millisecondsPerSecond);
    }
}

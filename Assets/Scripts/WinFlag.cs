using System.Threading;
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
        if (other.CompareTag("Player") && timer != null)
        {
            timer.StopTimer();
        }
    }
}

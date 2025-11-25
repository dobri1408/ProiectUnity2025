using UnityEngine;
using TMPro;

public class SimpleTimer : MonoBehaviour
{
    float t;

    void Update()
    {
        t += Time.deltaTime;

        int m = (int)(t / 60);
        int s = (int)(t % 60);
        int ms = (int)((t * 100) % 100);

        GetComponent<TextMeshProUGUI>().text = $"{m:00}:{s:00}.{ms:00}";
    }
}

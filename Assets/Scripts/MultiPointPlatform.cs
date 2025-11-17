using UnityEngine;

public class MultiPointPlatform : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;
    public bool loop = true;

    [Tooltip("Timpul de pauză la fiecare punct (secunde)")]
    public float waitTime = 10f;

    private int index = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Update()
    {
        if (points.Length == 0) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;

                index++;
                if (index >= points.Length)
                {
                    if (loop)
                    {
                        index = 0;
                    }
                    else
                    {
                        enabled = false; 
                        return;
                    }
                }
            }
            else
            {
                return;
            }
        }

        Transform targetPoint = points[index];
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
    }
}

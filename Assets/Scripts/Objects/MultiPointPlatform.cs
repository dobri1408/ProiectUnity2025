using UnityEngine;

public class MultiPointPlatform : MonoBehaviour
{
    public Transform[] points; // path of object
    public float speed = 2f; // linear velocity of movement
    public bool loop = true; // one-shot if false or loop if true

    [Tooltip("Timpul de pauză la fiecare punct (secunde)")]
    public float waitTime = 10f; // wait time once target point is reached

    private int index = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;

    void Start()
    {
        // Move instantly to the first point
        if (points.Length > 0)
        {
            transform.position = points[0].position;
        }
    }
    void Update()
    {
        // Without points, stand still.
        if (points.Length == 0) return;
        
        // After wait is over, move to next point
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

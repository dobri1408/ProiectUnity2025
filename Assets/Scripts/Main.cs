using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        GameObject level = Resources.Load<GameObject>("Level");
        Instantiate(level, Vector3.zero, Quaternion.identity);

        GameObject player = Resources.Load<GameObject>("Player");
        Instantiate(player, new Vector3(0, 1, 0), Quaternion.identity);
    }
}

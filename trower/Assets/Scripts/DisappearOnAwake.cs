using UnityEngine;

public class DisappearOnAwake : MonoBehaviour
{
    private void Awake()
    {
        transform.position = new Vector3(10000, 10000);
    }
}

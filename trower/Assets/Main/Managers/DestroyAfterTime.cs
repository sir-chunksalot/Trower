using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public void DestroyMeAfter(float seconds)
    {
        StartCoroutine(KillMe(seconds));
    }

    private IEnumerator KillMe(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}

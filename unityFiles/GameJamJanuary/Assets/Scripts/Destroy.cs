using System.Collections;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] float lifeSpan;
    void Start()
    {
        StartCoroutine(Life(gameObject));
    }
    private IEnumerator Life(GameObject skullObject)
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(skullObject);
    }
}

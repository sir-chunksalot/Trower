using System.Collections;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField] GameObject activation;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            activation.SetActive(true);
            StopCoroutine(Deactivate());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            StartCoroutine(Deactivate());
        }
    }

    private IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(.5f);
        activation.SetActive(false);
    }

}

using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] float rateOfFire;
    bool delay;
    private void Awake()
    {
        delay = false;
    }

    private void OnEnable()
    {
        delay = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("collided fire" + collision.tag);
        if (collision.tag == "Enemy" && !delay)
        {
            //collision.gameObject.GetComponent<Hero>().ChangeHealth(-1, true);
            delay = true;
            if (gameObject.activeSelf)
            {
                StartCoroutine(FireTick());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            delay = false;
        }
    }
    private IEnumerator FireTick()
    {
        yield return new WaitForSeconds(rateOfFire);
        delay = false;
    }
}

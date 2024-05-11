using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] bool isRight;
    [SerializeField] float arrowSpeed;
    [SerializeField] float lifeSpan;
    [SerializeField] float gravity;
    Vector3 startPos;
    private void Awake()
    {
        Vector2 dir;
        Debug.Log(gameObject.transform.rotation.y + "ZORO");
        if (gameObject.transform.rotation.y == 1) //for some reason .rotation.y returns 1 when y is equal to 180?? whatever it works
        {
            dir = Vector2.right;
        }
        else
        {
            dir = Vector2.left;
        }

        gameObject.GetComponent<Rigidbody2D>().AddForce(dir * arrowSpeed);
        gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.down * gravity);

        StartCoroutine(LifeSpan());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //collision.gameObject.GetComponent<Hero>().ChangeHealth(-10, false);
        }
    }

    private IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }

}

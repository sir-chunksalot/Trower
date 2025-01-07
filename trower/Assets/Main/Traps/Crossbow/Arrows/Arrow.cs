using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float arrowSpeed;
    [SerializeField] float lifeSpan;
    [SerializeField] float gravity;
    Vector3 startPos;
    BoxCollider2D col;
    Animator anim;
    Rigidbody2D rb;
    Trap trap;
    private void Awake()
    {
        Vector2 dir;
        trap = gameObject.GetComponentInParent<Trap>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        col = gameObject.GetComponent<BoxCollider2D>();
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
        if (collision.gameObject.layer == 6)
        {
            Hero hero = collision.gameObject.GetComponent<Hero>();
            if (hero != null)
            {
                trap.TryDamage(hero.gameObject);
            }

        }
        else if (collision.gameObject.tag == "EnemyWall")
        {
            rb.velocity = Vector2.zero;
            anim.enabled = false;
            col.enabled = false;
        }
        else if (collision.gameObject.tag == "EnemyFloor")
        {
            rb.velocity = Vector2.zero;
            anim.SetTrigger("HitFloor");
            col.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }

}

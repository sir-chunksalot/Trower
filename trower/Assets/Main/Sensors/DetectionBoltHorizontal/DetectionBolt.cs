using System.Collections;
using UnityEngine;

public class DetectionBolt : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] ParticleSystem deathParticle;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    float dir;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        dir = 1;
        Debug.Log("FLAN" + gameObject.transform.parent.rotation.y);
        if (gameObject.transform.parent.rotation.y == 1)
        {
            dir = -1;
            sprite.flipX = true;
        }
        Move();
        StartCoroutine(DieAfterTime(20));
    }
    public void Move()
    {
        rb.velocity = Vector2.right * speed * dir;
    }

    private IEnumerator DieAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.tag == "TrapRoom")
    //    {
    //        if (collision.GetComponent<Trap>().GetCurrentCooldown() <= 0)
    //        {
    //            rb.velocity = Vector2.zero;
    //            transform.position = new Vector3(transform.position.x + 3, transform.position.y, transform.position.z);
    //            sprite.enabled = false;
    //            deathParticle.Play();
    //            DieAfterTime(4);
    //        }
    //    }
    //}

}

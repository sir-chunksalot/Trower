using System.Collections.Generic;
using UnityEngine;

public class FloorDestroy : MonoBehaviour
{
    [SerializeField] bool instaKill;
    List<Hero> heroes;
    List<GameObject> heroObj;
    Rigidbody2D rb;
    BoxCollider2D col;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        col = gameObject.GetComponent<BoxCollider2D>();
        heroes = new List<Hero>();
        heroObj = new List<GameObject>();
    }
    public void DestroyFloor()
    {
        if(instaKill)
        {
            Destroy(gameObject);
        }
        if (col != null)
        {
            col.enabled = false;
        }


        if(heroes != null)
        {
            foreach (Hero hero in heroes)
            {
                Debug.Log("floor made hero super fall wacum");
                hero.GetComponent<Hero>().StartSuperFall(-1, false);
            }

        }

        gameObject.layer = 31;
        transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z + 10);

        rb.gravityScale = 1;
        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.angularVelocity = Random.Range(-179, 180);
        rb.velocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("wacum + hero was added to list");
            Hero hero = collision.gameObject.GetComponent<Hero>();
            if (hero != null)
            {
                heroObj.Add(collision.gameObject);
                heroes.Add(hero);
            }

        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("hero was removed from list");
            if (!heroObj.Contains(collision.gameObject)) { return; }
            int index = heroObj.IndexOf(collision.gameObject);
            heroes.RemoveAt(index);
            heroObj.RemoveAt(index);
        }
    }
}

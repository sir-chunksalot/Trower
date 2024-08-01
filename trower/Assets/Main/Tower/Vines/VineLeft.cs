using UnityEngine;

public class VineLeft : MonoBehaviour
{
    Animator anim;
    Vine vine;
    SpriteRenderer sprite;
    void Start()
    {
        anim = gameObject.GetComponentInParent<Animator>();
        vine = gameObject.GetComponentInParent<Vine>();
        sprite = gameObject.GetComponentInParent<SpriteRenderer>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("VINE COLLISION" + collision.gameObject);
        if (!vine.GetActive() && collision.gameObject.GetComponentInParent<Vine>().GetActive()) //if this vine is inactive and the colliding vine is active do:
        {
            Debug.Log("VINEALERT!!" + sprite.sprite);
            sprite.flipX = false;
            sprite.enabled = true;

            anim.enabled = true;
            vine.SetActiveOn();

            collision.gameObject.SetActive(false);
        }
    }


}

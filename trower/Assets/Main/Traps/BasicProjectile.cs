using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Animator anim;
    Vector2 velocity;
    float dir;
    bool isMoving;

    GridSpace gridSpace;

    public void SetTargetCell(GridSpace cell)
    {
        if(cell.GetPos().x < transform.position.x)
        {
            sprite.flipX = true;
            dir = -1;
        }
        else { dir = 1; }


        gridSpace = cell;
        isMoving = true;
    }
    private void Update()
    {
        if(!isMoving) { return; }

        Vector2Int currentPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        anim.SetBool("Moving", true);
        velocity = (Vector2.right * speed * dir * Time.deltaTime);
        transform.Translate(velocity);

        if (Mathf.Abs((currentPos - gridSpace.GetPos()).magnitude) <= .2f) //finish moving
        {
            anim.SetBool("Fall", true);
            dir = 0;
            isMoving = false;
            StartCoroutine(KillAfterTime());
        }
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }

    private IEnumerator KillAfterTime()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}

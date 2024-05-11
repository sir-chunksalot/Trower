using System.Collections.Generic;
using UnityEngine;

public class TrapHitbox : MonoBehaviour
{
    Trap trap;
    List<GameObject> enemies;
    private void Start()
    {
        trap = gameObject.GetComponentInParent<Trap>();
        enemies = new List<GameObject>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.activeSelf == false) return;
        if (collision.tag == "Enemy")
        {
            if (enemies.Count == 0)
            {
                trap.Activate();
            }
            enemies.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeSelf == false)
        {
            trap.Deactivate();
            return;
        }
        if (collision.tag == "Enemy")
        {
            enemies.Remove(collision.gameObject);
            if (enemies.Count == 0)
            {
                trap.Deactivate();
            }
        }
    }

}

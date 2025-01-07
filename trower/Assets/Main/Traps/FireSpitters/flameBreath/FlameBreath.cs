using System.Collections;
using UnityEngine;

public class FlameBreath : MonoBehaviour
{
    [SerializeField] BoxCollider2D col;
    float cooldown = .3f;
    bool canDamage;
    Trap trap;

    public void SetPapaTrap(Trap trap)
    {
        this.trap = trap;
    }
    private void OnDisable()
    {
        if (col != null)
        {
            col.enabled = false;
        }
        canDamage = false;
    }

    private void OnEnable()
    {
        StartCoroutine(FireDelay());
        canDamage = true;
    }

    private IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(.3f);
        if (col != null)
        {
            col.enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (trap == null || !canDamage) return;
        trap.TryDamage(collision.gameObject);
    }
}

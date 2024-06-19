using System;
using System.Collections;
using UnityEngine;

public class Stab : MonoBehaviour
{
    Collider2D col;
    [SerializeField] GameObject dad;
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] Animator anim;

    HeroManager heroManager;
    Trap trap;
    private bool up;

    private void Start()
    {
        trap = dad.GetComponent<Trap>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager.GetComponent<TrapManager>().onSpacePressed += SpearUp;
        heroManager = gameManager.GetComponent<HeroManager>();
        col = gameObject.GetComponent<BoxCollider2D>();
        col.enabled = false;
    }
    public void SpearUp(object sender, EventArgs e) //this is to move spear collider up along with spears
    {
        Debug.Log("COMPONENT");
        if (trap.GetCanAttack())
        {
            StartCoroutine(ColDelay());
            anim.SetBool("TrapActivated", true);
            col.offset = new Vector3(offsetX, offsetY + 3);
            Debug.Log("sanja piggu");

        }
    }

    private void FixedUpdate()
    {
        if (up && trap.GetCurrentCooldown() <= 0)
        {
            SpearDown();
        }
    }

    public void SpearDown()
    {
        Debug.Log("trap deactivated");
        up = false;
        col.enabled = false;
        anim.SetBool("TrapActivated", false);
        col.offset = new Vector3(offsetX, offsetY - 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject + "sunflower");
        if (collision.gameObject.layer == 6) //hero  layer
        {
            Debug.Log("i forgor you sunflower");
            if (up)
            {
                if (collision.gameObject.GetComponent<Hero>().GetFallingStatus())
                {
                    heroManager.KillHero(collision.gameObject);
                }
            }
            else
            {
                Debug.Log("sunfloewr miss");
                heroManager.KillHero(collision.gameObject);
            }
        }
    }
    private IEnumerator ColDelay()
    {
        yield return new WaitForSeconds(.4f);
        col.enabled = true;
        StartCoroutine(LengthOfAttack());
    }

    private IEnumerator LengthOfAttack()
    {
        yield return new WaitForSeconds(.2f);
        up = true;
        trap.CooldownOn();
    }
}

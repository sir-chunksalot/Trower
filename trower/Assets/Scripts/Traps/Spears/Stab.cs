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
    private int parentID;
    private bool reset;

    private void Start()
    {
        trap = dad.GetComponent<Trap>();
        trap.onActivate += SpearUp;
        trap.onCooldownOver += SpearDown;
        col = gameObject.GetComponent<BoxCollider2D>();
        col.enabled = false;
        parentID = gameObject.transform.parent.gameObject.GetInstanceID();
        Debug.Log("(stab) gameObject parent " + gameObject.transform.parent.gameObject + "id" + parentID);
        reset = true;
    }
    public void SpearUp(object sender, EventArgs e) //this is to move spear collider up along with spears
    {
        if((int)sender != parentID) { return; } 
        if(!reset) { return; }
        Debug.Log("COMPONENT");
        StartCoroutine(ColDelay());
        anim.SetBool("TrapActivated", true);
        col.offset = new Vector3(offsetX, offsetY + 3);
        Debug.Log("sanja piggu");
        reset = false;
    }
    public void SpearDown(object sender, EventArgs e)
    {
        if ((int)sender != parentID)
        {
            return;
        }
        Debug.Log("trap deactivated");
        up = false;
        col.enabled = false;
        anim.SetBool("TrapActivated", false);
        col.offset = new Vector3(offsetX, offsetY - 3);
        reset = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject + "sunflower");
        if (collision.gameObject.layer == 6) //hero  layer
        {
            Debug.Log("i forgor you sunflower");
            Hero hero = collision.gameObject.GetComponent<Hero>();
            if(hero == null) { return; }
            if (up)
            {
                if (hero.GetFallingStatus())
                {
                    hero.KillMe();
                }
            }
            else
            {
                Debug.Log("sunfloewr miss");
                hero.KillMe();
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

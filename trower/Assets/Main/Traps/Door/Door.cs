using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{

    [SerializeField] GameObject dad;
    [SerializeField] Animator anim;
    [SerializeField] Image chargeUI;
    [SerializeField] float timeToCharge;

    Trap trap;
    Vector3 chargeSpawnSpot;
    RectTransform chargeTransform;
    Color chargeColor;
    private int parentID;
    private float chargeCount;
    bool canOpen;
    bool smacking;
    float chargeTime;
    void Start()
    {
        canOpen = true;
        trap = dad.GetComponent<Trap>();
        parentID = gameObject.transform.parent.gameObject.GetInstanceID();
        trap.onActivate += StartCharge;
        trap.onDeactivate += Smack;
        trap.onMouseLeave += Smack;
        trap.onCooldownOver += CloseDoor;

        chargeUI.gameObject.transform.parent.GetComponent<Canvas>().worldCamera = Camera.main;
        chargeSpawnSpot = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        chargeTransform = chargeUI.GetComponent<RectTransform>();
        chargeColor = chargeUI.color;
        chargeTime = timeToCharge / 100;
    }

    public void StartCharge(object sender, EventArgs e)
    {
        Debug.Log("open door");
        if ((int)sender != parentID) { return; }
        if (!canOpen) { return; }
        chargeTransform.transform.position = chargeSpawnSpot;
        chargeUI.color = chargeColor;
        StartCoroutine(Charge());
        Debug.Log("fuckcum Start charge!");

    }

    public void Smack(object sender, EventArgs e)
    {
        if ((int)sender != parentID) { return; }
        if (!canOpen) { return; }
        if (chargeCount == 0) { return; }
        if (chargeCount == 1)
        {
            //super attack
        }


        Debug.Log("OPENED DOOR!");
        anim.ResetTrigger("Close");
        anim.SetTrigger("Open");
        StopAllCoroutines();
        StartCoroutine(LengthOfAttack());
        Debug.Log("smacked ! fuckcum");
    }

    public void CloseDoor(object sender, EventArgs e)
    {
        if ((int)sender != parentID)
        {
            return;
        }
        if (chargeCount <= 0) { return; }
        chargeCount = 0;
        Debug.Log("CLOSED DOOR!");
        anim.SetTrigger("Close");
        anim.ResetTrigger("Open");
        canOpen = true;
    }

    private IEnumerator Charge()
    {
        yield return new WaitForSeconds(chargeTime);
        chargeCount += .01f;
        chargeUI.fillAmount = chargeCount;
        Debug.Log(chargeCount + "COOLDOWN");
        if (chargeCount >= 1)
        {
            //play sound
            chargeUI.color = Color.yellow;
        }
        else
        {
            StartCoroutine(Charge());
        }

    }

    private IEnumerator LengthOfAttack()
    {
        if (chargeCount >= 1)
        {
            //play cool sound
        }
        else
        {
            //play slightly less cool sound
        }
        canOpen = false;
        smacking = true;
        yield return new WaitForSeconds(.4f);
        smacking = false;
        chargeUI.fillAmount = 0;
        trap.CooldownOn();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("FARTDOOR hit something");
        if (collision.gameObject.layer == 6 && smacking) //hero  layer
        {
            Debug.Log("FARTDOOR hit a hero");


            if (chargeCount >= 1)
            {
                collision.GetComponent<Hero>().StartSuperFall(chargeCount * 3, true);
            }
            else if (chargeCount > .50)
            {
                collision.GetComponent<Hero>().StartSuperFall(chargeCount * 2, false);
            }
            else if (chargeCount > .10)
            {
                collision.GetComponent<Hero>().StartSuperFall(-1, false);
            }
            else
            {
                collision.GetComponent<Hero>().Stun(chargeCount * 2);
            }

        }
    }
}

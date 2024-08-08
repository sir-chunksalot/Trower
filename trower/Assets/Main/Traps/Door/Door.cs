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
    void Awake()
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

    public void MoveDoor(Vector3 pos)
    {
        if (chargeTransform == null) return;
        Vector3 newChargeSpawn = new Vector3(pos.x + transform.parent.localPosition.x, pos.y + 3, pos.z);
        chargeSpawnSpot = newChargeSpawn;
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
        if (collision.gameObject.layer == 6 && smacking) //hero  layer
        {
            Hero hero = collision.GetComponent<Hero>();
            if (hero == null) return;

            if (chargeCount >= 1)
            {
                hero.StartSuperFall(chargeCount * 3, true);
            }
            else if (chargeCount > .50)
            {
                hero.StartSuperFall(chargeCount * 2, false);
            }
            else if (chargeCount > .10)
            {
                hero.StartSuperFall(-1, false);
            }
            else
            {
                hero.Stun(chargeCount * 2);
            }

        }
    }
}

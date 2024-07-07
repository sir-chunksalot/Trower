using System;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] GameObject dad;
    [SerializeField] Animator anim;

    Trap trap;
    private int parentID;
    bool canOpen;
    bool smacking;
    void Start()
    {
        canOpen = true;
        trap = dad.GetComponent<Trap>();
        parentID = gameObject.transform.parent.gameObject.GetInstanceID();
        trap.onActivate += OpenDoor;
        trap.onDeactivate += CloseDoor;
        trap.onMouseLeave += CloseDoor;
        trap.onCooldownOver += CanOpen;
    }

    public void OpenDoor(object sender, EventArgs e)
    {
        Debug.Log("open door");
        if ((int)sender != parentID) {
            return; }
        if(!canOpen) {
            return;
        }
        Debug.Log("OPENED DOOR!");
        anim.ResetTrigger("Close");
        anim.SetTrigger("Open");
        StartCoroutine(LengthOfAttack());
        trap.CooldownOn();
    }

    public void CloseDoor(object sender, EventArgs e)
    {
        if ((int)sender != parentID) { 
            return; }
        Debug.Log("CLOSED DOOR!");
        anim.SetTrigger("Close");
    }
    
    public void CanOpen(object sender, EventArgs e)
    {
        canOpen = true;
    }

    private IEnumerator LengthOfAttack()
    {
        smacking = true;
        yield return new WaitForSeconds(.4f);
        smacking = false;
        canOpen = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("FARTDOOR hit something");
        if (collision.gameObject.layer == 6 && smacking) //hero  layer
        {
            Debug.Log("FARTDOOR hit a hero");
            collision.GetComponent<Hero>().SuperFall();
        }
    }

    //public void Activate(bool isRight, bool buttonRelease = false)
    //{
    //    if (isRight)
    //    {
    //        if (buttonRelease)
    //        {
    //            smackRight.UseDoor(false);
    //        }
    //        else
    //        {
    //            smackRight.UseDoor(true);
    //        }

    //    }
    //    else
    //    {
    //        if (buttonRelease)
    //        {
    //            smackLeft.UseDoor(false);
    //        }
    //        else
    //        {
    //            smackLeft.UseDoor(true);
    //        }

    //    }
    //}
}

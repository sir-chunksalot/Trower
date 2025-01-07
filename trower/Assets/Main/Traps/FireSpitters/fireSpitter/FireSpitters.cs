using System;
using System.Collections;
using UnityEngine;

public class FireSpitters : MonoBehaviour
{
    [SerializeField] GameObject dad;
    [SerializeField] GameObject fire;
    [SerializeField] Animator anim;
    [SerializeField] float flameDelay;
    [SerializeField] bool facingRight;
    int parentID;
    Trap trap;
    GameObject myFire;
    bool spitting;
    private void Start()
    {
        trap = dad.GetComponent<Trap>();
        //trap.onActivate += OpenMouth;
        //trap.onMouseLeave += CloseMouth;
        //trap.onDeactivate += CloseMouth;
        parentID = dad.GetInstanceID();

        if (myFire == null)
        {
            if (gameObject.transform.rotation.y == 1) { facingRight = true; }
            float fireSpawnX = 3f;
            if (facingRight) { fireSpawnX = -3f; }
            Vector3 fireSpawnpos = new Vector3(transform.position.x + fireSpawnX, transform.position.y - .2f, gameObject.transform.position.z - 1);
            myFire = Instantiate(fire, fireSpawnpos, Quaternion.identity);
            if (facingRight) { myFire.GetComponent<SpriteRenderer>().flipX = true; }
            myFire.SetActive(false);
            FlameBreath flameBreath = myFire.GetComponent<FlameBreath>();
            flameBreath.SetPapaTrap(trap);
        }
    }

    private void OpenMouth(object sender, EventArgs e)
    {
        if ((int)sender != parentID) { return; }
        spitting = true;
        StartCoroutine(ToggleFire());
        anim.ResetTrigger("EndFire");
        anim.SetTrigger("StartFire");

    }

    private IEnumerator ToggleFire()
    {
        yield return new WaitForSeconds(flameDelay);
        if (spitting)
        {
            myFire.SetActive(true);
        }
    }

    private void CloseMouth(object sender, EventArgs e)
    {
        if ((int)sender != parentID) { return; }
        if (myFire != null) { myFire.SetActive(false); };
        spitting = false;
        anim.ResetTrigger("StartFire");
        anim.SetTrigger("EndFire");
    }
}

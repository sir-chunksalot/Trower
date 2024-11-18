using System;
using System.Collections;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    [SerializeField] GameObject dad;
    [SerializeField] GameObject arrow;
    [SerializeField] float shootDelay;
    [SerializeField] Animator anim;
    Vector3 spawnPos;
    int parentID;
    bool reset;
    Trap trap;

    private void Start()
    {
        spawnPos = new Vector3(gameObject.transform.position.x, 2.5f + gameObject.transform.position.y, gameObject.transform.position.z);
        trap = dad.GetComponent<Trap>();
        trap.onActivate += Shoot;
        trap.onCooldownOver += Rearm;
        parentID = dad.GetInstanceID();
        reset = true;
    }

    public void Shoot(object sender, EventArgs e) 
    {
        if ((int)sender != parentID) { return; }
        if (!reset) { return; }

        StartCoroutine(ShootDelay());
        anim.SetBool("TrapActivated", true);
        reset = false;
        
    }
    public void Rearm(object sender, EventArgs e)
    {
        anim.SetBool("TrapActivated", false);
        reset = true;
    }


    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(1.25f);

        float yRot = 0;
        if(transform.rotation.y != 1) { yRot = 180; }

        Instantiate(arrow, spawnPos, Quaternion.Euler(0, yRot, 0), transform.parent.transform);
        trap.CooldownOn();
    }

}

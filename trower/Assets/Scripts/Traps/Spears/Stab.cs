using System;
using System.Collections;
using UnityEngine;

public class Stab : MonoBehaviour
{
    Collider2D col;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject dad;
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] Animator anim;

    Trap trap;
    
    private void Start()
    {
        trap = dad.GetComponent<Trap>();
        gameManager.GetComponent<TrapManager>().onActivateTraps += SpearUp;
        col = gameObject.GetComponent<BoxCollider2D>();
        col.enabled = false;
    }
    public void SpearUp(object sender, EventArgs e) //this is to move spear collider up along with spears
    {
        Debug.Log("COMPONENT");
        if(trap.GetCanAttack())
        {
            StartCoroutine(ColDelay());
            anim.SetTrigger("TrapActivated");
            col.offset = new Vector3(offsetX, offsetY + 3);
            Debug.Log("sanja piggu");
        }
        
    }

    private IEnumerator ColDelay()
    {
        yield return new WaitForSeconds(.4f);
        col.enabled = true;
    }
}

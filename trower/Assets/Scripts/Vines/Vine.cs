using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    [SerializeField] bool isActive;
    [SerializeField] GameObject leftVine;
    [SerializeField] GameObject rightVine;
    
    bool active;
    private void Start()
    {
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        Animator anim = gameObject.GetComponent<Animator>();
        if(isActive)
        {
            sprite.enabled = true;
            anim.enabled = true;
        }
        else
        {
            sprite.enabled = false;
            anim.enabled = false;
        }

    }
    public void SetActiveOn()
    {
        Debug.Log("VINE ENABLED");
        active = true;

        StartCoroutine(WaitForNextVine());
    }

    private IEnumerator WaitForNextVine()
    {
        yield return new WaitForSeconds(.1f);
        rightVine.GetComponent<BoxCollider2D>().enabled = false;
        rightVine.GetComponent<BoxCollider2D>().enabled = true;

    }
    public bool GetActive()
    {
        return active;
    }
}

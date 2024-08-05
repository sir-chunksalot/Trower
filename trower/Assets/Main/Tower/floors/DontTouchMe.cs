using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontTouchMe : MonoBehaviour
{
    [SerializeField] GameObject target;
    public float strength;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DontTouchMe otherObj = collision.transform.GetComponent<DontTouchMe>();
        if (otherObj == null) return;
        if(otherObj.strength < this.strength)
        {
            otherObj.Kill();
        }
    }

    public void Kill()
    {
        if(target != null)
        {
            Debug.Log("Dont touch me i died");
            StartCoroutine(ShadowRealm(target));
        }
    }

    private IEnumerator ShadowRealm(GameObject target)
    {
        target.transform.position = new Vector3(999, 999, 999);
        yield return new WaitForSeconds(1);
        Destroy(target);
    }
}

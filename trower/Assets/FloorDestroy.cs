using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDestroy : MonoBehaviour
{
    private void Start()
    {
        GameObject parent = gameObject.transform.parent.transform.parent.gameObject;
        Floor floor = parent.GetComponentInParent<Floor>();
        Debug.Log("DIRECTCUM" + gameObject.transform.parent.transform.parent.gameObject.name);
        floor.GetTowerBuilder().onTowerSell += DestroyFloor;
    }
    public void DestroyFloor(object floor, EventArgs empty) 
    {
        if ((GameObject)floor != gameObject.transform.parent.transform.parent.gameObject) { return; }
        gameObject.layer = 5;
        transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z + 1);
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.angularVelocity = UnityEngine.Random.Range(0, 180);
        rb.velocity = new Vector2(UnityEngine.Random.Range(0, 1.0f), UnityEngine.Random.Range(0, 1.0f));

    }
}

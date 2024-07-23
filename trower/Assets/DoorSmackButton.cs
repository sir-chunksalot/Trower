using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSmackButton : MonoBehaviour
{
    [SerializeField] GameObject door;
    TowerBuilder towerBuilder;
    GameObject closestFloor;
    Animator anim;

    bool canClick;
    private void Start()
    {
        towerBuilder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TowerBuilder>();
        canClick = true;
        anim = gameObject.GetComponent<Animator>();
    }

    private GameObject FindClosestFloor()
    {
        Vector3 floorPos = towerBuilder.ClosestTo(gameObject.transform.position, towerBuilder.GetPlacedFloors().ToArray(), false);
        return towerBuilder.GetFloor(floorPos);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && canClick) //hero  layer
        {
            closestFloor = FindClosestFloor();
            anim.SetBool("Click", true);
            towerBuilder.SellFloor(closestFloor);
            closestFloor = FindClosestFloor();

            if (door.transform.position.y - 5.5 != 0)
            {
                door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y - 5, door.transform.position.z);
                gameObject.transform.parent.position = new Vector3(gameObject.transform.parent.position.x, gameObject.transform.parent.position.y - 5, gameObject.transform.parent.position.z);
            }
            else
            {
                door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y - 5, door.transform.position.z);
                //do stuff
            }
            StartCoroutine(Immunity());
        }
    }

    private IEnumerator Immunity()
    {
        canClick = false;
        yield return new WaitForSeconds(3);
        canClick = true;
        anim.SetBool("Click", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSmackButton : MonoBehaviour
{
    [SerializeField] GameObject door;
    TowerBuilder towerBuilder;
    GameObject closestFloor;
    private void Start()
    {
        towerBuilder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TowerBuilder>();
        closestFloor = FindClosestFloor();

    }

    private GameObject FindClosestFloor()
    {
        Vector3 floorPos = towerBuilder.ClosestTo(gameObject.transform.position, towerBuilder.GetPlacedFloors().ToArray(), false);
        return towerBuilder.GetFloor(floorPos);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) //hero  layer
        {
            gameObject.GetComponent<Animator>().SetTrigger("Click");
            towerBuilder.SellFloor(closestFloor);
            closestFloor = FindClosestFloor();
            gameObject.transform.parent.position = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z);
            door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y - 5, door.transform.position.z);
        }
    }
}

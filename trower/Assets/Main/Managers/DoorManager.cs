using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] float doorZ;
    public bool useDoor;

    Door[] doorScript = new Door[2];

    private GameManager gameManager;
    private TowerBuilder towerBuilder;

    private Vector2 highestFloor;
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        towerBuilder.onTowerPlace += UpdateDoor;
        towerBuilder.onTowerStart += DoorStart;

        doorScript = door.GetComponentsInChildren<Door>();
    }

    public void OnSceneLoad(object sender, EventArgs e)
    {
        useDoor = gameManager.GetCurrentLevelDetails().useDoor;
        highestFloor = Vector2.zero;
        door.SetActive(useDoor);
    }

    public void DoorStart(object sender, EventArgs e)
    {
        Debug.Log("fartttt");
        door.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        door.GetComponentInChildren<Canvas>().sortingLayerName = "Floor";
        MoveDoor(FindHighest(towerBuilder.GetPlacedFloorsPos().ToArray()));
    }
    public void UpdateDoor(object floor, EventArgs e)
    {
        GameObject buildDad = (GameObject)floor;
        Vector3[] newPos = new Vector3[buildDad.transform.childCount];
        int count = 0;
        foreach(Transform kid in buildDad.transform)
        {
            newPos[count] = kid.transform.position;
            count++;
        }
        Vector2 doorSpawnPos = FindHighest(newPos);
        if (doorSpawnPos != (Vector2)door.transform.position)
        {
            MoveDoor(doorSpawnPos);
        }
    }

    private Vector2 FindHighest(Vector3[] posistions)
    {
        foreach (Vector2 pos in posistions)
        {
            if (pos.y > highestFloor.y)
            {
                highestFloor = pos;
            }
            else if (pos.x == highestFloor.x && pos.y == highestFloor.y)
            {
                highestFloor = pos;
            }
        }
        return highestFloor;
    }

    private void MoveDoor(Vector2 pos)
    {
        door.transform.position = new Vector3(pos.x, pos.y, doorZ);
        Debug.Log("a is clear fellas" + doorScript[0] + doorScript[0].gameObject);
        Debug.Log("a is clear fellas" + doorScript[1] + doorScript[1].gameObject);
        doorScript[0].MoveDoor(pos);
        doorScript[1].MoveDoor(pos);
    }
}

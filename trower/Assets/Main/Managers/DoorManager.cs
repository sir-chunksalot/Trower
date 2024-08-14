using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject potentialDoor;
    [SerializeField] float doorZ;
    public bool useDoor;
    public bool canMoveDoor;

    Door[] doorScript = new Door[2];

    private GameManager gameManager;
    private TowerBuilder towerBuilder;
    private TrapManager trapManager;
    private WaveManager waveManager;

    private List<Vector2> highestFloors = new List<Vector2>();
    private List<GameObject> potentialDoors = new List<GameObject>();
    private List<Vector2> potentialDoorsPos = new List<Vector2>();

    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        trapManager = gameObject.GetComponent<TrapManager>();
        waveManager = gameObject.GetComponent<WaveManager>();
        towerBuilder.onTowerPlace += UpdateDoor;
        towerBuilder.onTowerStart += DoorStart;
        trapManager.onSpacePressed += TransferDoor;
        waveManager.OnAttackPhaseStart += AttackPhase;
        waveManager.OnDefensePhaseStart += DefensePhase;
        doorScript = door.GetComponentsInChildren<Door>();

    }

    public void OnSceneLoad(object sender, EventArgs e)
    {
        useDoor = gameManager.GetCurrentLevelDetails().useDoor;
        door.SetActive(useDoor);
        bool enemiesCanWin = gameManager.GetCurrentLevelDetails().enemiesCanWin;
        //doorScript[0].(pos);
        //doorScript[1].MoveDoor(pos);
    }

    public void AttackPhase(object sender, EventArgs e)
    {
        canMoveDoor = false;
        ClearPotDoors();
    }

    public void DefensePhase(object sender, EventArgs e)
    {
        canMoveDoor = true;
        PotentialDoors();
    }

    public void TransferDoor(object sender, EventArgs e)
    {
        if (!canMoveDoor) return;
        GameObject[] doors = (GameObject[])potentialDoors.ToArray().Clone();
        foreach (GameObject door in doors)
        {
            if(door.GetComponent<Trap>().GetSpaceEffect().activeInHierarchy)
            {
                int index = potentialDoors.IndexOf(door);
                MoveDoor(potentialDoorsPos[index]);
                PotentialDoors();
            }
        }
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
        Vector2 doorSpawnPos = FindHighest((Vector3[])towerBuilder.GetPlacedFloorsPos().ToArray().Clone());
        if (doorSpawnPos != (Vector2)door.transform.position)
        {
            MoveDoor(doorSpawnPos);
        }
        if (canMoveDoor)
        {
            PotentialDoors();
        }
        
    }

    private void ClearPotDoors()
    {
        GameObject[] doors = (GameObject[])potentialDoors.ToArray().Clone();
        foreach (GameObject door in doors)
        {
            int index = potentialDoors.IndexOf(door);
            potentialDoors.RemoveAt(index);
            potentialDoorsPos.RemoveAt(index);

            Destroy(door);
        }
    }

    private void PotentialDoors()
    {
        if (!gameManager.GetCurrentLevelDetails().canMoveDoor) return;
        ClearPotDoors();
        foreach(Vector2 floor in highestFloors)
        {
            if (!potentialDoorsPos.Contains(floor) && (Vector2)door.transform.position != floor) {
                GameObject newPotDoor = Instantiate(potentialDoor, new Vector3(floor.x, floor.y, doorZ), Quaternion.identity);
                potentialDoors.Add(newPotDoor);
                potentialDoorsPos.Add(floor);
            }
        }
    }

    private Vector2 FindHighest(Vector3[] posistions)
    {
        highestFloors.Clear();
        float highestPoint = towerBuilder.GetHighestPoint();
        foreach (Vector2 pos in posistions)
        {
            if (pos.y == highestPoint)
            {
                highestFloors.Add(pos);
            }

        }
        Vector2 closestToZero = new Vector2(999, 999);
        foreach(Vector2 floor in highestFloors)
        {
            if(Mathf.Abs(floor.x) < closestToZero.x)
            {
                closestToZero = floor;
            }
        }
        return closestToZero;
    }

    public Vector3 GetDoorPos()
    {
        return door.transform.position;
    }

    private void MoveDoor(Vector2 pos)
    {
        door.transform.position = new Vector3(pos.x, pos.y, doorZ);
        doorScript[0].MoveDoor(pos);
        doorScript[1].MoveDoor(pos);
    }
}

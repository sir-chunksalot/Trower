using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAdditions : MonoBehaviour
{
    [SerializeField] GameObject woodenTriangle;
    [SerializeField] GameObject roofExtension;
    [SerializeField] GameObject bridge;
    TowerBuilder towerBuilder;
    List<GameObject> addedFrills;
    List<Vector3> bridgesPos;
    List<GameObject> bridgesBeginFloor;
    List<GameObject> bridges;
    GenerateViableFloors genViableFloors;
    void Start()
    {
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        genViableFloors = gameObject.GetComponent<GenerateViableFloors>();
        towerBuilder.onTowerPlace += CreateAdditions;
        addedFrills = new List<GameObject>();
        bridgesPos = new List<Vector3>();
        bridges = new List<GameObject>();
        bridgesBeginFloor = new List<GameObject>();
    }

    private Vector3[] GetFloorLocations()
    {
        List<GameObject> placedFloors = towerBuilder.GetPlacedFloors();
        Vector3[] floorLocations = new Vector3[placedFloors.Count];

        int i = 0;
        foreach (GameObject floors in placedFloors)
        {
            floorLocations[i] = floors.transform.position;
            i++;
        }

        return floorLocations;
    }

    private void CreateAdditions(object sender, EventArgs e)
    {
        Debug.Log("THE LISTS" + "bridgePosCount" + bridgesPos.Count + "bridges count " + bridges.Count + "startFloors count" + bridgesBeginFloor.Count);
        GameObject[] placedFloors = towerBuilder.GetPlacedFloors().ToArray();
        int index = 0;
        foreach (Vector3 floorPos in GetFloorLocations())
        {

            WoodenTriangle(floorPos, placedFloors[index]);
            RoofExtensions(floorPos, placedFloors[index]);
            index++;
        }
        FindBridgeSpots();

        foreach (Vector3 floorPos in GetFloorLocations())
        {
            foreach (GameObject additions in addedFrills)
            {
                if (additions == null)
                {
                    addedFrills.Remove(additions);
                    break;
                }
                else if (additions.transform.position == floorPos)
                {
                    addedFrills.Remove(additions);
                    Destroy(additions);
                    break;
                }
                else if (additions.tag == "roofExtension" && Mathf.Abs(floorPos.y - additions.transform.position.y) < 2)
                {
                    Debug.Log("COYT");
                    Destroy(additions);
                    addedFrills.Remove(additions);
                    break;
                }
            }
        }
        foreach(GameObject bridge in bridges)
        {
            foreach (GameObject additions in addedFrills)
            {
                if(Vector2.Distance(additions.transform.position, bridge.transform.position) < 5)
                {
                    Destroy(additions);
                }
            }
        }
    }

    private void FindBridgeSpots()
    {
        List<GameObject> validBridges = new List<GameObject>();
        List<Vector3> validBridgePos = new List<Vector3>();
        List<GameObject> validBridgeStarts = new List<GameObject>();
        List<GameObject> deadBridges = new List<GameObject>();
        foreach (GameObject bridge in bridges)
        {
            int bridgeIndex = bridges.IndexOf(bridge);
            if (bridge == null || towerBuilder.GetPlacedFloorsPos().Contains(bridge.transform.position)) //bridge overlaps a build
            {
                deadBridges.Add(bridgesBeginFloor[bridgeIndex]);
            }
        }
        foreach(GameObject bridge in bridges)
        {
            int bridgeIndex = bridges.IndexOf(bridge);
            if (deadBridges.Contains(bridgesBeginFloor[bridgeIndex])) //connected bridge was destroyed
            {
                Destroy(bridges[bridgeIndex]);
            }
            else //keep the bridge
            {
                validBridges.Add(bridges[bridgeIndex]);
                validBridgeStarts.Add(bridgesBeginFloor[bridgeIndex]);
                validBridgePos.Add(bridges[bridgeIndex].transform.position);
            }
        }

        bridges = validBridges;
        bridgesBeginFloor = validBridgeStarts;
        bridgesPos = validBridgePos;

        StartCoroutine(CheckValidBridgeSpawns());
    }

    private IEnumerator CheckValidBridgeSpawns()
    {
        yield return new WaitForSeconds(.5f);
        foreach (GameObject badFloor in genViableFloors.GetInaccessibleFloors())
        {
            Debug.Log("naht FOUND INACCESSIBLE FLOOR");
            Vector3 endPointLeft = new Vector3(-100, 0, 0);
            Vector3 endPointRight = new Vector3(100, 0, 0);
            foreach (Vector3 floorPos in GetFloorLocations())
            {
                Debug.Log("naht getting floors");
                if (floorPos.y == badFloor.transform.position.y)
                {
                    if (floorPos.x < badFloor.transform.position.x) //left bridge
                    {
                        Debug.Log("naht FOUND INACCESSIBLE FLOOR LEFT");
                        if (badFloor.transform.position.x - floorPos.x < badFloor.transform.position.x - endPointLeft.x)
                        {
                            endPointLeft = floorPos;
                        }
                    }
                    else if (floorPos.x > badFloor.transform.position.x) //right bridge
                    {
                        Debug.Log("naht FOUND INACCESSIBLE RIGHT");
                        if (badFloor.transform.position.x - floorPos.x > badFloor.transform.position.x - endPointRight.x)
                        {
                            endPointRight = floorPos;
                        }
                    }
                }
            }


            if (endPointLeft.x != -100) { MakeBridge(badFloor.transform.position, endPointLeft, true, badFloor); }
            if (endPointRight.x != 100) { MakeBridge(badFloor.transform.position, endPointRight, false, badFloor); }
        }
    }
    private void MakeBridge(Vector3 startFloor, Vector3 endFloor, bool goLeft, GameObject currentFloor)
    {
        bool bridgeComplete = false;
        Vector3 spawnPos = startFloor;
        float count = 0;
        float dir = -1;
        Debug.Log("naht TRIED TO MAKE BRIDGE" + "endfloor" + endFloor + "startfloor" + startFloor + "isLeft" + goLeft);
        if (goLeft) dir = 1;
        while (!bridgeComplete && count < 10)
        {
            if (endFloor.x + (towerBuilder.GetRoomBounds().x * dir) == spawnPos.x) bridgeComplete = true;
            count++;
            spawnPos -= new Vector3(towerBuilder.GetRoomBounds().x, 0, 0) * dir;
            foreach (Vector3 pos in bridgesPos)
            {
                Debug.Log("there is a bridge at " + pos + "naht");
            }
            if (bridgesPos.Contains(spawnPos) || towerBuilder.GetPlacedFloorsPos().Contains(spawnPos))
            {
                Debug.Log("naht there is a build already at" + spawnPos);
                continue;
            }
            else
            {
                Debug.Log("naht spawned bridge" + "spawnPos" + spawnPos + "count" + count);
                GameObject newBridge = Instantiate(bridge, spawnPos, Quaternion.identity, currentFloor.transform);
                bridgesPos.Add(spawnPos);
                bridges.Add(newBridge);
                bridgesBeginFloor.Add(currentFloor);
            }
        }
        Debug.Log("naht end");
    }


    private void RoofExtensions(Vector3 floorPos, GameObject currentFloor)
    {
        Vector3[] floorLocations = GetFloorLocations();

        float yOffSet = 3.07f;
        float xOffSet = 5;
        bool canPlace = false;
        Vector3 spawnPos = new Vector3();
        foreach (Vector3 pos in floorLocations)
        {
            if (floorPos.y == pos.y && floorPos.x == pos.x + 10)
            {
                canPlace = true;
                spawnPos = new Vector3(floorPos.x - xOffSet, floorPos.y + yOffSet, floorPos.z + .5f);
            }
            if (floorPos.y == pos.y && floorPos.x == pos.x - 10)
            {
                canPlace = true;
                spawnPos = new Vector3(floorPos.x + xOffSet, floorPos.y + yOffSet, floorPos.z + .5f);
            }
        }

        foreach (GameObject frills in addedFrills)
        {
            if (frills != null && frills.transform.position == spawnPos)
            {
                canPlace = false;
            }
        }

        if (canPlace)
        {
            GameObject newBuild = Instantiate(roofExtension, spawnPos, Quaternion.identity, currentFloor.transform);
            GameObject newBuild2 = Instantiate(roofExtension, spawnPos - new Vector3(0, 6.14f), Quaternion.identity, currentFloor.transform);
            newBuild.GetComponent<SpriteRenderer>().color = currentFloor.GetComponentInChildren<SpriteRenderer>().color;
            newBuild2.GetComponent<SpriteRenderer>().color = currentFloor.GetComponentInChildren<SpriteRenderer>().color;
            newBuild2.GetComponent<SpriteRenderer>().flipY = true;
            addedFrills.Add(newBuild);
            addedFrills.Add(newBuild2);
        }
    }

    private void WoodenTriangle(Vector3 floorPos, GameObject currentFloor)
    {
        Vector3[] floorLocations = GetFloorLocations();

        float xDistance = towerBuilder.GetRoomBounds().x;
        float yDistance = towerBuilder.GetRoomBounds().y;

        Vector3 newTriangle = new Vector3(floorPos.x, floorPos.y - yDistance, floorPos.z);
        bool canPlace = true;
        bool nextToLeftWall = false;
        bool nextToRightWall = false;

        foreach (Vector3 pos in floorLocations)
        {
            if (newTriangle.x == pos.x)
            {
                if (newTriangle.y == pos.y) //triangle is in the same spot as another room
                {
                    canPlace = false;
                }
                if (newTriangle.y - yDistance == pos.y) //triangle is right above another room
                {
                    canPlace = false;
                }
            }

            if (newTriangle.y == pos.y)
            {
                if (newTriangle.x - xDistance == pos.x)//only generates if its next to left wall
                {
                    nextToLeftWall = true;
                    currentFloor = towerBuilder.GetFloor(new Vector3(pos.x, pos.y, pos.z));
                }
                if (newTriangle.x + xDistance == pos.x)//only generates if next to right wall
                {
                    nextToRightWall = true;
                    currentFloor = towerBuilder.GetFloor(new Vector3(pos.x, pos.y, pos.z));
                }
            }
        }

        float firstFloorOffsetX = 0;
        if (newTriangle.y == 0)
        {
            firstFloorOffsetX = 2;
        }

        if (canPlace && nextToLeftWall)
        {
            newTriangle = newTriangle - new Vector3(1 + firstFloorOffsetX, 0, -3);
            if (!AlreadyThere(newTriangle))
            {
                GameObject newBuild = Instantiate(woodenTriangle, newTriangle, Quaternion.identity, currentFloor.transform);
                newBuild.GetComponent<SpriteRenderer>().flipX = true;
                newBuild.GetComponent<SpriteRenderer>().color = currentFloor.GetComponentInChildren<SpriteRenderer>().color;
                addedFrills.Add(newBuild);
            }
        }
        else if (canPlace && nextToRightWall)
        {
            newTriangle = newTriangle + new Vector3(1 + firstFloorOffsetX, 0, 3);
            if (!AlreadyThere(newTriangle))
            {
                GameObject newBuild = Instantiate(woodenTriangle, newTriangle, Quaternion.identity, currentFloor.transform);
                newBuild.GetComponent<SpriteRenderer>().color = currentFloor.GetComponentInChildren<SpriteRenderer>().color;
                addedFrills.Add(newBuild);
            }
        }

    }

    private bool AlreadyThere(Vector3 pos)
    {
        foreach (GameObject frills in addedFrills)
        {
            if (frills != null && pos.x == frills.transform.position.x && pos.y == frills.transform.position.y)
            {
                return true;
            }
        }
        return false;
    }
}

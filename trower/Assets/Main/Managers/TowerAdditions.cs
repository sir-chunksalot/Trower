using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAdditions : MonoBehaviour
{
    [SerializeField] GameObject woodenTriangle;
    [SerializeField] GameObject roofExtension;
    [SerializeField] GameObject bridge;
    [SerializeField] GameObject smokeEffect;
    TowerBuilder towerBuilder;
    GameManager gameManager;
    GameObject buildDaddy;
    List<GameObject> addedFrills;
    List<Vector3> bridgesPos;
    List<GameObject> bridgesBeginFloor;
    List<GameObject> bridgeEndFloor;
    List<GameObject> bridges;
    List<GameObject> debugThings;
    GenerateViableFloors genViableFloors;
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        genViableFloors = gameObject.GetComponent<GenerateViableFloors>();
        towerBuilder.onTowerPlace += CreateAdditions;
        towerBuilder.onTowerSell += CreateAdditions;
        genViableFloors.onFinishedScan += CreateBridges;
        addedFrills = new List<GameObject>();
        bridgesPos = new List<Vector3>();
        bridges = new List<GameObject>();
        bridgesBeginFloor = new List<GameObject>();
        debugThings = new List<GameObject>();
        bridgeEndFloor = new List<GameObject>();
    }

    private void OnSceneLoad(object sender, EventArgs e)
    {
        addedFrills.Clear();
        bridgesPos.Clear();
        bridgesBeginFloor.Clear();
        bridgeEndFloor.Clear();
        bridges.Clear();
        buildDaddy = gameManager.GetCurrentLevelDetails().GetBuildDaddy();
    }
    private void CreateBridges(object sender, EventArgs e)
    {
        DeleteOldBridges();
    }
    private void CreateAdditions(object sender, EventArgs e)
    {
        StopAllCoroutines();
        GameObject[] placedFloors = CopyToArray(towerBuilder.GetPlacedFloors());
        int index = 0;
        Vector3[] placedFloorsPos = CopyToArray(towerBuilder.GetPlacedFloorsPos());
        foreach (Vector3 floorPos in placedFloorsPos)
        {

            WoodenTriangle(floorPos, placedFloors[index]);
            RoofExtensions(floorPos, placedFloors[index]);
            index++;
        }
        //DeleteOldBridges();
        //StartCoroutine(CheckValidBridgeSpawns());

        foreach (Vector3 floorPos in placedFloorsPos)
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

    }

    private void DeleteOldBridges()
    {
        List<GameObject> validBridges = new List<GameObject>();
        List<Vector3> validBridgePos = new List<Vector3>();
        List<GameObject> validBridgeStarts = new List<GameObject>();
        List<GameObject> validBridgeEnds = new List<GameObject>();
        List<GameObject> deadBridges = new List<GameObject>();
        foreach(GameObject floor in genViableFloors.GetInaccessibleFloors())
        {
            Debug.Log("armageddon" + floor.name);
        }
        foreach (GameObject bridge in bridges)
        {
            int bridgeIndex = bridges.IndexOf(bridge);
            if (bridge == null || towerBuilder.GetPlacedFloorsPos().Contains(bridge.transform.position)) //bridge overlaps a build
            {
                deadBridges.Add(bridges[bridgeIndex]);
            }
            else if (genViableFloors.IsFloorAccessible(bridgeEndFloor[bridgeIndex].transform.position) && genViableFloors.IsFloorAccessible(bridgesBeginFloor[bridgeIndex].transform.position)) //bridge is no longer needed
            {
                Debug.Log("japan lol" + genViableFloors.IsFloorAccessible(bridgeEndFloor[bridgeIndex].transform.position) + genViableFloors.IsFloorAccessible(bridgesBeginFloor[bridgeIndex].transform.position));
                deadBridges.Add(bridges[bridgeIndex]);
            }

        }
        foreach (GameObject bridge in bridges)
        {
            int bridgeIndex = bridges.IndexOf(bridge);
            if (deadBridges.Contains(bridge)) //connected bridge was destroyed
            {
                Destroy(bridges[bridgeIndex]);
            }
            else //keep the bridge
            {
                validBridges.Add(bridges[bridgeIndex]);
                validBridgeStarts.Add(bridgesBeginFloor[bridgeIndex]);
                validBridgeEnds.Add(bridgeEndFloor[bridgeIndex]);
                validBridgePos.Add(bridges[bridgeIndex].transform.position);
            }
        }

        bridges = validBridges;
        bridgesBeginFloor = validBridgeStarts;
        bridgesPos = validBridgePos;
        bridgeEndFloor = validBridgeEnds;
        CheckValidBridgeSpawns();
        MurderStuffNearBridge();
    }

    private void CheckValidBridgeSpawns()
    {
        foreach (GameObject badFloor in genViableFloors.GetInaccessibleFloors())
        {
            Debug.Log("naht FOUND INACCESSIBLE FLOOR");
            Vector3 endPointLeft = new Vector3(-100, 0, 0);
            Vector3 endPointRight = new Vector3(100, 0, 0);
            Vector3[] placedFloorsPos = CopyToArray(towerBuilder.GetPlacedFloorsPos());
            foreach (Vector3 floorPos in placedFloorsPos)
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
            //StartCoroutine(DeleteOldBridges(UnityEngine.Random.Range(0, 100)));

        }
    }
    private void MakeBridge(Vector3 startFloor, Vector3 endFloor, bool goLeft, GameObject currentFloor)
    {

        bool bridgeComplete = false;
        Vector3 spawnPos = startFloor;
        float count = 0;
        float bridgeCount = 0;
        float dir = -1;
        GameObject endFloorObj = towerBuilder.GetPlacedFloor(endFloor);

        if (goLeft) dir = 1;
       
        while (!bridgeComplete && count < 10)
        {
            Debug.Log("log that shiz" + (Mathf.Floor(endFloor.x + (towerBuilder.GetRoomBounds().x * dir)) == Mathf.Floor(spawnPos.x) || Mathf.Floor(endFloor.x - (towerBuilder.GetRoomBounds().x * dir)) == Mathf.Floor(spawnPos.x)));
            if(count > 8)
            {
                Debug.Log("naht TRIED TO MAKE BRIDGE" + "endfloor" + endFloor + "startfloor" + startFloor + "isLeft" + goLeft + "fart" + (towerBuilder.GetRoomBounds().x * dir) + "shit" + (Mathf.Floor(endFloor.x + (towerBuilder.GetRoomBounds().x * dir)) == Mathf.Floor(spawnPos.x)) + "cum" + spawnPos.x);
            }
            if (count == 0)
            {
                Debug.Log("naht TRIED TO MAKE BRIDGE FIRSTFIRSTFIRST" + "endfloor" + endFloor + "startfloor" + startFloor + "isLeft" + goLeft + "fart" + (towerBuilder.GetRoomBounds().x * dir) + "shit" + (Mathf.Floor(endFloor.x + (towerBuilder.GetRoomBounds().x * dir)) == Mathf.Floor(spawnPos.x)) + "cum" + spawnPos.x);
            }
            if ((Mathf.Floor(endFloor.x + (towerBuilder.GetRoomBounds().x * dir)) == Mathf.Floor(spawnPos.x) || Mathf.Floor(endFloor.x - (towerBuilder.GetRoomBounds().x * dir)) == Mathf.Floor(spawnPos.x)))
            {
                bridgeComplete = true;
                continue;
            }
            count++;
            spawnPos -= new Vector3(towerBuilder.GetRoomBounds().x, 0, 0) * dir;
            if (bridgesPos.Contains(spawnPos) || towerBuilder.GetPlacedFloorsPos().Contains(spawnPos))
            {
                Debug.Log("naht there is a build already at" + spawnPos);
                continue;
            }
            else
            {
                bridgesPos.Add(spawnPos);
                bridgeCount++;
                InstantiateBridge(spawnPos, currentFloor, endFloorObj);
                Debug.Log("naht spawned bridge" + "spawnPos" + spawnPos + "count" + count);
            }
        }


        Debug.Log("naht end");

    }

    private void InstantiateBridge(Vector3 spawnPos, GameObject startFloor, GameObject endFloor)
    {
        Instantiate(smokeEffect, spawnPos, Quaternion.identity);
        GameObject newBridge = Instantiate(bridge, spawnPos, Quaternion.identity, buildDaddy.transform);
        endFloor.transform.parent.GetComponent<Floor>().AddFollower(newBridge);
        startFloor.transform.parent.GetComponent<Floor>().AddFollower(newBridge);
        bridges.Add(newBridge);
        bridgesBeginFloor.Add(startFloor);
        bridgeEndFloor.Add(endFloor);
        MurderStuffNearBridge();
    }


    private void MurderStuffNearBridge()
    {
        foreach (GameObject bridge in bridges)
        {
            if (bridge == null) continue;
            GameObject[] currentFrills = CopyToArray(addedFrills);
            foreach (GameObject additions in currentFrills)
            {
                if (additions == null) continue;
                if (Vector2.Distance(additions.transform.position, bridge.transform.position) < 5.5f)
                {
                    Destroy(additions);
                    Debug.Log("bridge murdered frill");
                    addedFrills.Remove(additions);
                }
            }
        }
    }

    public void DebugSpawnAdditions()
    {
        Debug.Log("ronald chan counted " + addedFrills.Count);
        if(debugThings.Count > 0)
        {
            Debug.Log("ronald chan killed the frills");
            foreach (GameObject additions in debugThings)
            {
                Destroy(additions);
            }
            debugThings.Clear();
        }
        else
        {
            foreach (GameObject additions in addedFrills)
            {
                if (additions == null) continue;
                Debug.Log("ronald chan made frill at " + additions.transform.position);
                GameObject debugThing = Instantiate(roofExtension, new Vector3(additions.transform.position.x, additions.transform.position.y, Camera.main.transform.position.z + 1), Quaternion.identity) ;
                debugThing.GetComponent<SpriteRenderer>().color = Color.black;
                debugThings.Add(debugThing);
            }
        }

    }


    private void RoofExtensions(Vector3 floorPos, GameObject currentFloor)
    {
        Vector3[] floorLocations = CopyToArray(towerBuilder.GetPlacedFloorsPos());
        
        float yOffSet = 3.07f;
        float xOffSet = 5;
        Vector3 spawnPos = new Vector3();
        Vector3 sideFloorPos = new Vector3();
        foreach (Vector3 pos in floorLocations)
        {
            if (floorPos.y == pos.y && floorPos.x == pos.x + 10)
            {
                sideFloorPos = pos;
                GameObject sideFloor = towerBuilder.GetPlacedFloor(sideFloorPos);
                spawnPos = new Vector3(floorPos.x - xOffSet, floorPos.y + yOffSet, floorPos.z + .5f);

                PlaceRoofExtension(currentFloor, sideFloor, spawnPos);
            }
            if (floorPos.y == pos.y && floorPos.x == pos.x - 10)
            {
                sideFloorPos = pos;
                GameObject sideFloor = towerBuilder.GetPlacedFloor(sideFloorPos);
                spawnPos = new Vector3(floorPos.x + xOffSet, floorPos.y + yOffSet, floorPos.z + .5f);

                PlaceRoofExtension(currentFloor, sideFloor, spawnPos);
            }
        }
    }

    private void PlaceRoofExtension(GameObject currentFloor, GameObject sideFloor, Vector3 spawnPos)
    {
        foreach (GameObject frills in addedFrills)
        {
            if (frills != null && frills.transform.position == spawnPos)
            {
                return;
            }
        }
        Floor floor1 = currentFloor.transform.parent.GetComponent<Floor>();
        Floor floor2 = sideFloor.transform.parent.GetComponent<Floor>();
        GameObject newBuild = Instantiate(roofExtension, spawnPos, Quaternion.identity, buildDaddy.transform);
        Debug.Log("SPAWNPOS FOR ROOF EXTENSION" + spawnPos);
        floor1.AddFollower(newBuild);
        floor2.AddFollower(newBuild);

        newBuild.GetComponent<SpriteRenderer>().color = currentFloor.GetComponentInChildren<SpriteRenderer>().color;

        addedFrills.Add(newBuild);

        if (spawnPos.y - 6.14f > 0)
        {
            GameObject newBuild2 = Instantiate(roofExtension, spawnPos - new Vector3(0, 6.14f), Quaternion.identity, buildDaddy.transform);
            Debug.Log("SPAWNPOS FOR ROOF EXTENSION" + (spawnPos - new Vector3(0, 6.14f)));
            floor1.AddFollower(newBuild2);
            floor2.AddFollower(newBuild2);

            newBuild2.GetComponent<SpriteRenderer>().color = currentFloor.GetComponentInChildren<SpriteRenderer>().color;
            newBuild2.GetComponent<SpriteRenderer>().flipY = true;

            addedFrills.Add(newBuild2);
        }
    }

    private void WoodenTriangle(Vector3 floorPos, GameObject currentFloor)
    {
        Vector3[] floorLocations = CopyToArray(towerBuilder.GetPlacedFloorsPos());

        float xDistance = towerBuilder.GetRoomBounds().x;
        float yDistance = towerBuilder.GetRoomBounds().y;

        Vector3 newTriangle = new Vector3(floorPos.x, floorPos.y - yDistance, floorPos.z);
        bool canPlace = true;
        bool nextToLeftWall = false;
        bool nextToRightWall = false;
        GameObject roofRoom = towerBuilder.GetPlacedFloor(floorPos);

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
                    currentFloor = towerBuilder.GetPlacedFloor(new Vector3(pos.x, pos.y, pos.z));
                }
                if (newTriangle.x + xDistance == pos.x)//only generates if next to right wall
                {
                    nextToRightWall = true;
                    currentFloor = towerBuilder.GetPlacedFloor(new Vector3(pos.x, pos.y, pos.z));
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
                GameObject newBuild = Instantiate(woodenTriangle, newTriangle, Quaternion.identity, buildDaddy.transform);
                roofRoom.transform.parent.GetComponent<Floor>().AddFollower(newBuild);
                currentFloor.transform.parent.GetComponent<Floor>().AddFollower(newBuild);
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
                GameObject newBuild = Instantiate(woodenTriangle, newTriangle, Quaternion.identity, buildDaddy.transform);
                roofRoom.transform.parent.GetComponent<Floor>().AddFollower(newBuild);
                currentFloor.transform.parent.GetComponent<Floor>().AddFollower(newBuild);
                newBuild.GetComponent<SpriteRenderer>().color = currentFloor.GetComponentInChildren<SpriteRenderer>().color;
                addedFrills.Add(newBuild);
            }
        }

    }

    public T[] CopyToArray<T>(List<T> genList) 
    {
        return (T[])genList.ToArray().Clone();

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

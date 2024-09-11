using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapBuilder : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] GameObject[] traps;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject moveTrapEffect;
    [SerializeField] GameObject debugTrap;
    [SerializeField] float reqMouseDistanceToPlace;
    [SerializeField] float opacity;

    GameObject trapDaddy;

    public event EventHandler onTrapPlace;
    private GameObject currentBuild;
    private Trap currentTrap;
    TowerBuilder towerBuilder;
    TrapManager trapManager;
    GenerateViableFloors genViableFloors;
    UIManager uiManager;
    //reg spawns
    private List<Vector2> totalSpawnLocations;
    private List<GameObject> debugTrapTotalSpawnLocations;
    private List<GameObject> placedTraps;
    private List<Vector2> placedTrapsPos;
    private List<Vector2> validTrapSpawnLocations;
    private List<Vector2> floorHolder;
    //wall spawns
    private List<Vector2> totalWallSpawns;
    private List<Vector2> validWallSpawns;
    private List<bool> isLeftWallSpawn;
    //placed walls
    private List<Vector2> placedWallSpawns;
    private List<GameObject> wallTraps;

    private bool placingTrap;
    private bool facingLeft;
    private string trapName;
    private bool isLarge;
    private bool isWallTrap;
    private bool rotateTrap;
    private Vector2 mousePos;
    private float frontZ;
    private Vector3 offSet;

    Vector3 closestRoom;
    private void Awake()
    {
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        genViableFloors = gameObject.GetComponent<GenerateViableFloors>();
        towerBuilder.onTowerSell += RemoveTrapSpawns;
        trapManager = gameObject.GetComponent<TrapManager>();
        uiManager = gameObject.GetComponent<UIManager>();

        gameManager = gameObject.GetComponent<GameManager>();
        towerBuilder.onTowerPlace += NewRoom;
        towerBuilder.onTowerStart += TowerLoaded;
        towerBuilder.onTowerPlaceLast += GenWallSpawns;

    }
    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (placingTrap)
        {

            if (isLarge)
            {
                closestRoom = towerBuilder.ClosestTo(mousePos, towerBuilder.GetPlacedFloors().ToArray(), false);
            }
            else if (isWallTrap)
            {
                closestRoom = towerBuilder.ClosestTo(mousePos, validWallSpawns, false);
                int index = totalWallSpawns.IndexOf(closestRoom);
                Debug.Log("INDEXING" + index);
                if (index != -1)
                {
                    if (isLeftWallSpawn[index]) { currentTrap.Rotate(false); }
                    else { currentTrap.Rotate(true); }
                }
                
            }
            else
            {
                closestRoom = towerBuilder.ClosestTo(mousePos, validTrapSpawnLocations, false);
            }

            if (Vector2.Distance(closestRoom, mousePos) < reqMouseDistanceToPlace)
            {
                currentBuild.transform.position = new Vector3(closestRoom.x, closestRoom.y + offSet.y, frontZ);
            }
            else
            {
                currentBuild.transform.position = new Vector3(mousePos.x, mousePos.y, frontZ);
            }
            Debug.Log("PLACING" + closestRoom + " " + currentBuild.transform.position);

        }
    }
    public void TowerLoaded(object sender, EventArgs e)
    {
        validTrapSpawnLocations = new List<Vector2>();
        totalWallSpawns = new List<Vector2>();
        isLeftWallSpawn = new List<bool>();
        validWallSpawns = new List<Vector2>();
        placedWallSpawns = new List<Vector2>();
        wallTraps = new List<GameObject>();
        floorHolder = new List<Vector2>();
        totalSpawnLocations = new List<Vector2>();
        placedTrapsPos = new List<Vector2>();
        debugTrapTotalSpawnLocations = new List<GameObject>();
        placedTraps = new List<GameObject>();

        trapDaddy = gameManager.GetTrapDaddy();
        if (trapDaddy != null)
        {
            foreach (Transform kid in trapDaddy.transform)
            {
                placedTraps.Add(kid.gameObject);
            }
        }

        frontZ = Camera.main.transform.position.z + .1f;

        foreach (GameObject floor in towerBuilder.GetPlacedFloors())
        {
            FindNewSpawnLocations(floor.transform);   
        }
    }

    public void GenWallSpawns(object sender, EventArgs e)
    {
        List<GameObject> walls = genViableFloors.GetWalls();
        List<Vector2> newWallSpawns = new List<Vector2>();
        List<bool> newIsLeftWall = new List<bool>();
        foreach(GameObject wall in walls)
        {
            newWallSpawns.Add(wall.transform.position);
            if(Mathf.Sign(wall.transform.localPosition.x) == 1)
            {
                newIsLeftWall.Add(false);
            }
            else { newIsLeftWall.Add(true); }
        }
        isLeftWallSpawn = newIsLeftWall;
        totalWallSpawns = newWallSpawns;
        foreach(Vector2 wall in totalWallSpawns)
        {
            Debug.Log("wall i could love" + wall);
        }

        UpdateOldWallTraps();
    }

    private void UpdateOldWallTraps()
    {
        List<GameObject> newWallTraps = new List<GameObject>();
        List<Vector2> new0PlacedWallSpawns = new List<Vector2>(); //kills null walls
        int count = 0;
        foreach(GameObject wall in wallTraps)
        {
            if (wall != null)
            {
                newWallTraps.Add(wall);
                new0PlacedWallSpawns.Add(placedWallSpawns[count]);
            }
            count++;
        }
        placedWallSpawns = new0PlacedWallSpawns;
        wallTraps = newWallTraps; //kills null walls

        List<Vector2> newPlacedWallSpawns = new List<Vector2>();
        foreach(Vector2 spawnedWall in placedWallSpawns)
        {
            if(!totalWallSpawns.Contains(spawnedWall))
            {
                int index = placedWallSpawns.IndexOf(spawnedWall);
                bool checkRight = false;
                Vector2 closestLeftWall = new Vector2(-1000, 0);
                Vector2 closestRightWall = new Vector2(1000, 0);

                if (wallTraps[index].GetComponent<Trap>().GetRotation().y == 180) //is right wall
                {
                    checkRight = true;
                    
                }
                Debug.Log("FUCKFUCKFUCK" + checkRight + "   " + wallTraps[index].GetComponent<Trap>().GetRotation().y);
                foreach (Vector2 wall in totalWallSpawns)
                {
                    if (wall.y == spawnedWall.y && wall.x > spawnedWall.x && wall.x < closestRightWall.x)
                    {
                        closestRightWall = wall;
                    }
                    if (wall.y == spawnedWall.y && wall.x < spawnedWall.x && wall.x > closestLeftWall.x)
                    {
                        closestLeftWall = wall;
                    }
                }
                Vector2 closestWall = Vector2.zero;
                if(checkRight || closestLeftWall.x == -1000)
                {
                    closestWall = closestRightWall;
                }
                if(closestRightWall.x == 1000 || !checkRight)
                {
                    closestWall = closestLeftWall;
                }

                if(Mathf.Abs(closestWall.x) != 1000)
                {
                    newPlacedWallSpawns.Add(closestWall);

                    Vector3 oldTrapPos = new Vector3(wallTraps[index].transform.position.x, wallTraps[index].transform.position.y + 1.5f, wallTraps[index].transform.position.z - 1);
                    GameObject effect1 = Instantiate(moveTrapEffect, oldTrapPos, Quaternion.identity);
                    effect1.transform.localScale = new Vector3(.6f, .6f, 1);
                    wallTraps[index].transform.position = closestWall;
                    Vector3 newTrapPos = new Vector3(closestWall.x, closestWall.y + 1.5f, oldTrapPos.z - 1);
                    GameObject effect2 = Instantiate(moveTrapEffect, newTrapPos, Quaternion.identity);
                    effect2.transform.localScale = new Vector3(.6f, .6f, 1);
                }
                else
                {
                    Destroy(wallTraps[index]);
                }
                
            }
            else
            {
                newPlacedWallSpawns.Add(spawnedWall);
            }
        }
        placedWallSpawns = newPlacedWallSpawns;


        UpdateValidWallSpawns();
    }


    private void UpdateValidWallSpawns()
    {
        List<Vector2> newValidWallSpawns = new List<Vector2>();
        foreach (Vector2 wallSpawn in totalWallSpawns) //assigns valid walls
        {
            if (!placedWallSpawns.Contains(wallSpawn))
            {
                newValidWallSpawns.Add(wallSpawn);
            }
        }
        validWallSpawns = newValidWallSpawns;
    }

    private void FindNewSpawnLocations(Transform placedFloor)
    {
        Debug.Log("firefly" + placedFloor.gameObject.name);
        if (!towerBuilder.GetPlacedFloors().Contains(placedFloor.gameObject)) return;
        Vector2 spawnLeft = new Vector2(placedFloor.transform.position.x - 2.5f, placedFloor.transform.position.y);
        Vector2 spawnRight = new Vector2(placedFloor.transform.position.x + 2.5f, placedFloor.transform.position.y);
        if (!totalSpawnLocations.Contains(spawnLeft))
        {
            UpdateValidTrapSpawns(spawnLeft, placedFloor.transform.position, false);
        }
        if (!totalSpawnLocations.Contains(spawnRight))
        {
            UpdateValidTrapSpawns(spawnRight, placedFloor.transform.position, false);
        }
    }

    private void NewRoom(object floorFather, EventArgs e)
    {

        GameObject buildDad = (GameObject)floorFather;
        foreach (Transform placedFloor in buildDad.transform)
        {
            FindNewSpawnLocations(placedFloor);
        }
    }

    public void UpdateValidTrapSpawns(Vector2 pos, Vector3 floorPos, bool remove)
    {
        if(remove)
        {
            if(floorHolder.Contains(pos))
            {
                int index = floorHolder.IndexOf(pos);
                totalSpawnLocations.RemoveAt(index);
                floorHolder.RemoveAt(index);
            }
            else if(totalSpawnLocations.Contains(pos))
            {
                int index = totalSpawnLocations.IndexOf(pos);
                totalSpawnLocations.RemoveAt(index);
                floorHolder.RemoveAt(index);
            }
        }
        else
        {
            Debug.Log("life test" + pos);
            totalSpawnLocations.Add(pos);
            floorHolder.Add(floorPos);
        }
    }

    public void RemoveTrapSpawns(object soldFloor, EventArgs empty)
    {
        GameObject room = (GameObject)soldFloor;
        Debug.Log("ding dong build dad" + room.transform.parent);
        foreach (Transform floor in room.transform.parent)
        {
            Debug.Log("ding dong floor" + floor.gameObject.name + " fart" + floorHolder[2] + "poop" + floor.transform.position);
            List<Vector2> result = new List<Vector2>(floorHolder.FindAll(v => v == (Vector2)floor.transform.position));
            foreach(Vector2 pos in result)
            {
                int index = floorHolder.IndexOf(pos);
                Debug.Log("ding dong removing trap spawnLocations");
                totalSpawnLocations.RemoveAt(index);
                floorHolder.RemoveAt(index);

                if(placedTrapsPos.Contains(pos))
                {
                    Debug.Log("ding dong removing trap spawnLocations");
                    int trapIndex = placedTrapsPos.IndexOf(pos);
                    placedTraps.RemoveAt(trapIndex);
                    placedTrapsPos.RemoveAt(trapIndex);
                }

            }
        }
    }

   
    public bool GetPlacingTrap()
    {
        Debug.Log("get placing trap");
        return placingTrap;
    }

    public void Place(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (placingTrap)
            {
                PlaceBuild();
                onTrapPlace?.Invoke(currentBuild, EventArgs.Empty);
            }
            placingTrap = false;
        }
    }

    private void PlaceBuild()
    {
        Vector3 spawnSpot = closestRoom;
        if(currentTrap.GetIsFacingLeft())
        {
            offSet.x = offSet.x * -1;
        }
        Vector3 nextTrapPos = new Vector3(spawnSpot.x + offSet.x, spawnSpot.y + offSet.y, 8 + offSet.z);

        if (mousePos == (Vector2)currentBuild.transform.position)
        {
            Debug.Log("80s!");
            uiManager.UseCard(trapName, false);
            EndPlacement();
            return;
        }
        if (placingTrap)
        {
            Debug.Log("90s!");
            if (Vector2.Distance(nextTrapPos, new Vector2(mousePos.x, mousePos.y - offSet.y)) < reqMouseDistanceToPlace)
            {
                uiManager.UseCard(trapName, true);
                GameObject newBuild = Instantiate(currentBuild, nextTrapPos, Quaternion.identity, trapDaddy.transform);
                Trap trap = newBuild.GetComponent<Trap>();
                Vector3 floorPos = towerBuilder.ClosestTo(nextTrapPos, towerBuilder.GetPlacedFloorsPos(), false);
                if (isWallTrap)
                {
                    wallTraps.Add(newBuild);
                    Vector3 wallPos = towerBuilder.ClosestTo(nextTrapPos, totalWallSpawns, false);
                    placedWallSpawns.Add(wallPos);
                    UpdateValidWallSpawns();

                    int index = totalWallSpawns.IndexOf(spawnSpot);
                    Debug.Log("INDEXING" + index);
                    if (index != -1)
                    {
                        if (isLeftWallSpawn[index]) { trap.Rotate(false); }
                        else { trap.Rotate(true); }
                    }

                }
                else
                {
                    GameObject floor = towerBuilder.GetPlacedFloor(floorPos);
                    floor.transform.parent.GetComponent<Floor>().AddFollower(newBuild);
                }


                placedTraps.Add(newBuild);
                placedTrapsPos.Add(floorPos);

                UpdateValidTrapSpawns(spawnSpot, floorPos, true);

                Debug.Log("BEW BUILD!" + newBuild);

                Detector detector = newBuild.GetComponent<Detector>();
                if (trap != null)
                {
                    trap.EnableTrap();
                    if (rotateTrap) { trap.Rotate(true); }
                }
                if (detector != null)
                {
                    detector.EnableSensor();
                    if (rotateTrap) { detector.Rotate(true); }
                }

                towerBuilder.SetAlpha(newBuild, 1);
                Vector3 particleSpawnPos = newBuild.transform.position;
                particle.gameObject.transform.position = new Vector3(particleSpawnPos.x, particleSpawnPos.y, particleSpawnPos.z - 2);
                particle.GetComponent<ParticleSystem>().Clear();
                particle.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                uiManager.UseCard(trapName, false);
            }
            EndPlacement();
        }
    }
    public void CurrentTrap(string newTrapName, bool rotation)
    {
        validTrapSpawnLocations.Clear();
        foreach (Vector2 spawnPos in totalSpawnLocations)
        {
            if (!placedTrapsPos.Contains(spawnPos))
            {
                validTrapSpawnLocations.Add(spawnPos);
            }
        }
        Debug.Log("life " + validTrapSpawnLocations.Count + " life 2" + totalSpawnLocations.Count + "life 3" + placedTraps.Count + " " + placedTrapsPos.Count + "lifE EE" + floorHolder.Count ) ;

        Debug.Log(newTrapName + "UNO");
        placingTrap = true;
        rotateTrap = false;

        GameObject activeTrap = traps[0];
        foreach (GameObject trap in traps)
        {
            if (trap.name == newTrapName)
            {
                activeTrap = trap;
            }
        }


        Trap trapScript = activeTrap.GetComponent<Trap>();
        Detector sensorScript = activeTrap.GetComponent<Detector>();
        trapName = activeTrap.name;
        if (trapScript != null)
        { //it is a trap
            offSet = trapScript.GetOffset();
            GameObject newTrap = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            newTrap.GetComponent<Trap>().DisableTrap();
            towerBuilder.SetAlpha(newTrap, opacity);
            currentBuild = newTrap;
            currentTrap = newTrap.GetComponent<Trap>();
            isLarge = currentBuild.GetComponent<Trap>().GetTrapSize();
            isWallTrap = currentBuild.GetComponent<Trap>().GetIsWallTrap();

            if (rotation && !isWallTrap)
            {
                Debug.Log("LOLOL");
                currentTrap.Rotate(true);
                rotateTrap = true;
            }
        }
        else //it is a sensor
        {
            offSet = sensorScript.GetOffset();
            GameObject newSensor = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            newSensor.GetComponent<Detector>().DisableSensor();
            towerBuilder.SetAlpha(newSensor, opacity);
            currentBuild = newSensor;
            currentTrap = newSensor.GetComponent<Trap>();
            isLarge = false;

            if (rotation)
            {
                currentTrap.Rotate(true);
                rotateTrap = true;
            }
        }


    }


    public void DebugTrapSpawnLocations(bool availableCheck)
    {
        if (debugTrapTotalSpawnLocations.Count >= 1)
        {
            foreach (GameObject trap in debugTrapTotalSpawnLocations)
            {
                Destroy(trap);
            }
            debugTrapTotalSpawnLocations.Clear();
        }
        else
        {
            foreach (Vector3 pos in totalSpawnLocations)
            {
                if (placedTrapsPos.Contains(pos) && availableCheck) { continue; }
                GameObject trap = Instantiate(debugTrap, pos, Quaternion.identity);
                debugTrapTotalSpawnLocations.Add(trap);
            }
            foreach (Vector2 pos in totalWallSpawns)
            {
                if (placedTrapsPos.Contains(pos) && availableCheck) { continue; }
                GameObject trap = Instantiate(debugTrap, pos, Quaternion.identity);
                debugTrapTotalSpawnLocations.Add(trap);
            }
        }

    }

    public void EndPlacement()
    {
        Debug.Log("bew end");
        placingTrap = false;
        trapName = "";
        Destroy(currentBuild);
    }
}

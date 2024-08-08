using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapBuilder : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] GameObject[] traps;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject debugTrap;
    [SerializeField] float reqMouseDistanceToPlace;
    [SerializeField] float opacity;

    GameObject trapDaddy;

    public event EventHandler onTrapPlace;
    private GameObject currentBuild;
    TowerBuilder towerBuilder;
    TrapManager trapManager;
    UIManager uiManager;
    private List<Vector2> totalSpawnLocations;
    private List<GameObject> debugTrapTotalSpawnLocations;
    private List<GameObject> placedTraps;
    private List<Vector2> placedTrapsPos;
    private List<Vector2> validTrapSpawnLocations;
    private List<Vector2> floorHolder;
    private bool placingTrap;
    private string trapName;
    private bool isLarge;
    private bool rotateTrap;
    private Vector2 mousePos;
    private float frontZ;
    private float yOffSet;
    private void Awake()
    {
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        towerBuilder.onTowerSell += RemoveTrapSpawns;
        trapManager = gameObject.GetComponent<TrapManager>();
        uiManager = gameObject.GetComponent<UIManager>();

        gameManager = gameObject.GetComponent<GameManager>();
        towerBuilder.onTowerPlace += NewRoom;
        towerBuilder.onTowerStart += TowerLoaded;

    }
    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 closestRoom;

        if (placingTrap)
        {

            if (isLarge)
            {
                closestRoom = towerBuilder.ClosestTo(mousePos, towerBuilder.GetPlacedFloors().ToArray(), false);
            }
            else
            {
                closestRoom = towerBuilder.ClosestTo(mousePos, validTrapSpawnLocations, false);
            }

            if (Vector2.Distance(closestRoom, mousePos) < reqMouseDistanceToPlace)
            {
                currentBuild.transform.position = new Vector3(closestRoom.x, closestRoom.y + yOffSet, frontZ);
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
        totalSpawnLocations = new List<Vector2>();
        floorHolder = new List<Vector2>();
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
        Vector3 spawnSpot = towerBuilder.ClosestTo(mousePos, validTrapSpawnLocations, false);
        Vector3 nextTrapPos = new Vector3(spawnSpot.x, spawnSpot.y + yOffSet, 8);

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
            if (Vector2.Distance(nextTrapPos, new Vector2(mousePos.x, mousePos.y - yOffSet)) < reqMouseDistanceToPlace)
            {
                uiManager.UseCard(trapName, true);
                GameObject newBuild = Instantiate(currentBuild, nextTrapPos, Quaternion.identity, trapDaddy.transform);
                placedTraps.Add(newBuild);

                Vector3 floorPos = towerBuilder.ClosestTo(nextTrapPos, towerBuilder.GetPlacedFloorsPos(), false);
                GameObject floor = towerBuilder.GetPlacedFloor(floorPos);
                floor.transform.parent.GetComponent<Floor>().AddFollower(newBuild);
                placedTrapsPos.Add(floorPos);

                UpdateValidTrapSpawns(spawnSpot, floorPos, true);

                Debug.Log("BEW BUILD!" + newBuild);

                Trap trap = newBuild.GetComponent<Trap>();
                Detector detector = newBuild.GetComponent<Detector>();
                if (trap != null)
                {
                    trap.EnableTrap();
                    if (rotateTrap) { trap.Rotate(); }
                }
                if (detector != null)
                {
                    detector.EnableSensor();
                    if (rotateTrap) { detector.Rotate(); }
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
            yOffSet = trapScript.GetYOffset();
            GameObject newTrap = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            newTrap.GetComponent<Trap>().DisableTrap();
            towerBuilder.SetAlpha(newTrap, opacity);
            currentBuild = newTrap;
            isLarge = currentBuild.GetComponent<Trap>().GetTrapSize();

            if (rotation)
            {
                Debug.Log("LOLOL");
                currentBuild.GetComponent<Trap>().Rotate();
                rotateTrap = true;
            }
        }
        else //it is a sensor
        {
            yOffSet = sensorScript.GetYOffset();
            GameObject newSensor = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            newSensor.GetComponent<Detector>().DisableSensor();
            towerBuilder.SetAlpha(newSensor, opacity);
            currentBuild = newSensor;
            isLarge = false;

            if (rotation)
            {
                newSensor.GetComponent<Detector>().Rotate();
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

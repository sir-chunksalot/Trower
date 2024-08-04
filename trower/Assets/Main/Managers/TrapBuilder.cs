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
    private GameObject currentTrap;
    TowerBuilder towerBuilder;
    TrapManager trapManager;
    UIManager uiManager;
    private List<Vector2> totalSpawnLocations;
    private List<GameObject> debugTrapTotalSpawnLocations;
    private List<Vector2> placedTrapsPos;
    private List<Vector2> trapSpawnLocations;
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
        trapManager = gameObject.GetComponent<TrapManager>();
        uiManager = gameObject.GetComponent<UIManager>();

        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneChange += OnSceneChange;


    }
    private void Start()
    {


        towerBuilder.onTowerPlace += NewRoom;
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
                closestRoom = towerBuilder.ClosestTo(mousePos, trapSpawnLocations, false);
            }

            if (Vector2.Distance(closestRoom, mousePos) < reqMouseDistanceToPlace)
            {
                currentTrap.transform.position = new Vector3(closestRoom.x, closestRoom.y + yOffSet, frontZ);
            }
            else
            {

                currentTrap.transform.position = new Vector3(mousePos.x, mousePos.y, frontZ);
            }
            Debug.Log("PLACING" + closestRoom + " " + currentTrap.transform.position);

        }
    }

    private void OnSceneChange(object sender, EventArgs e)
    {
        Debug.Log("gameManager" + gameManager + gameManager);
        trapSpawnLocations = new List<Vector2>();
        totalSpawnLocations = new List<Vector2>();
        placedTrapsPos = new List<Vector2>();
        debugTrapTotalSpawnLocations = new List<GameObject>();

        trapDaddy = gameManager.GetTrapDaddy();
        if (trapDaddy != null)
        {
            foreach (Transform kid in trapDaddy.transform)
            {
                UpdateValidTrapSpawns(kid.position, false);
            }
        }

        List<GameObject> traps = trapManager.GetAllTraps();

        foreach (GameObject trap in traps)
        {
            Instantiate(trap, transform.position, Quaternion.identity, trapDaddy.transform);
        }
        frontZ = Camera.main.transform.position.z + .1f;

    }

    private void NewRoom(object sender, EventArgs e)
    {
        foreach (GameObject placedFloor in towerBuilder.GetPlacedFloors())
        {
            Vector2 spawnLeft = new Vector2(placedFloor.transform.position.x - 2.5f, placedFloor.transform.position.y);
            Vector2 spawnRight = new Vector2(placedFloor.transform.position.x + 2.5f, placedFloor.transform.position.y);
            if (!trapSpawnLocations.Exists(item => item == spawnLeft))
            {
                UpdateValidTrapSpawns(spawnLeft, true);
            }
            if (!trapSpawnLocations.Exists(item => item == spawnRight))
            {
                UpdateValidTrapSpawns(spawnRight, true);
            }
        }
    }

    public void UpdateValidTrapSpawns(Vector2 pos, bool newSpawn)
    {
        //if (Vector2.Distance(pos, towerBuilder.GetDoorPos()) <= 7)
        //{
        //    placedTrapsPos.Add(pos);
        //}
        Vector2 closestTrap = towerBuilder.ClosestTo(pos, placedTrapsPos, false);
        if (newSpawn)
        {
            totalSpawnLocations.Add(pos);
            Debug.Log(pos + " " + "closestTrap" + closestTrap + "NIGHTONFIREW");
            if (Mathf.Abs(Vector2.Distance(pos, closestTrap)) >= 4 && !trapSpawnLocations.Contains(pos)) //confirms the requested pos is nowhere near another trap
            {
                Debug.Log("NEW POS PASSIOn" + pos);
                trapSpawnLocations.Add(pos);
            }
        }
        else
        {
            placedTrapsPos.Add(pos);
            Debug.Log("removing valid trap spawn");
            closestTrap = towerBuilder.ClosestTo(pos, placedTrapsPos, false);
            trapSpawnLocations.Remove(closestTrap);
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
                onTrapPlace?.Invoke(currentTrap, EventArgs.Empty);
            }
            placingTrap = false;
        }
    }

    private void PlaceBuild()
    {
        Vector3 nextTrapPos = towerBuilder.ClosestTo(mousePos, trapSpawnLocations, false);
        nextTrapPos = new Vector3(nextTrapPos.x, nextTrapPos.y + yOffSet, 8);

        if (mousePos == (Vector2)currentTrap.transform.position)
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
                GameObject newBuild = Instantiate(currentTrap, nextTrapPos, Quaternion.identity, trapDaddy.transform);
                Debug.Log("BEW BUILD!" + newBuild);
                UpdateValidTrapSpawns(new Vector2(nextTrapPos.x, nextTrapPos.y - yOffSet), false);

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
            currentTrap = newTrap;
            isLarge = currentTrap.GetComponent<Trap>().GetTrapSize();

            if (rotation)
            {
                Debug.Log("LOLOL");
                currentTrap.GetComponent<Trap>().Rotate();
                rotateTrap = true;
            }
        }
        else //it is a sensor
        {
            yOffSet = sensorScript.GetYOffset();
            GameObject newSensor = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            newSensor.GetComponent<Detector>().DisableSensor();
            towerBuilder.SetAlpha(newSensor, opacity);
            currentTrap = newSensor;
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
        Destroy(currentTrap);
    }
}

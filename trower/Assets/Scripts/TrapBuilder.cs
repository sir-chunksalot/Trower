using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapBuilder : MonoBehaviour
{
    [SerializeField] GameObject[] traps;
    [SerializeField] GameObject particle;
    [SerializeField] float reqMouseDistanceToPlace;
    [SerializeField] float opacity;
    [SerializeField] GameObject trapDaddy;

    public event EventHandler onTrapPlace;
    private GameObject currentTrap;
    TowerBuilder towerBuilder;
    TrapSelect trapSelect;
    UIManager uiManager;
    private List<Vector2> totalSpawnLocations;
    private List<Vector2> placedTrapsPos;
    private List<Vector2> trapSpawnLocations;
    private bool placingTrap;
    private string trapName;
    private bool isLarge;
    private Vector2 mousePos;
    private Vector3 closestRoom;
    private float frontZ;
    private float yOffSet;
    private void Start()
    {
        trapSpawnLocations = new List<Vector2>();
        totalSpawnLocations = new List<Vector2>();
        placedTrapsPos = new List<Vector2>();

        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        trapSelect = gameObject.GetComponent<TrapSelect>();
        uiManager = gameObject.GetComponent<UIManager>();
        towerBuilder.onTowerPlace += NewRoom;

        frontZ = Camera.main.transform.position.z + .1f;
        OldTraps();
    }


    private void OldTraps()
    {
        GameObject trapDaddy = GameObject.FindGameObjectWithTag("TrapDaddy");
        foreach(Transform kid in trapDaddy.transform)
        {
            UpdateValidTrapSpawns(kid.position, false);
        }
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

            if (Vector2.Distance(closestRoom, mousePos) < reqMouseDistanceToPlace) {
                currentTrap.transform.position = new Vector3(closestRoom.x, closestRoom.y + yOffSet, frontZ);
            }
            else {

                currentTrap.transform.position = new Vector3(mousePos.x, mousePos.y, frontZ);
            }
            Debug.Log("PLACING" + closestRoom + " " + currentTrap.transform.position);

        }
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
        if (Vector2.Distance(pos, towerBuilder.GetDoorPos()) <= 7)
        {
            placedTrapsPos.Add(pos);
        }
        Vector2 closestTrap = towerBuilder.ClosestTo(pos, placedTrapsPos, false);
        if (newSpawn)
        {
            totalSpawnLocations.Add(pos);
            Debug.Log(pos + " " + "closestTrap" + closestTrap + "NIGHTONFIREW");
            if (Mathf.Abs(Vector2.Distance(pos, closestTrap)) >= 4 && !trapSpawnLocations.Contains(pos)) //confirms the requested pos is no where near another trap
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

        if(mousePos == (Vector2)currentTrap.transform.position) {
            Debug.Log("80s!");
            uiManager.UseTrap(trapName, false);
            EndPlacement();
            return;
        }
        if (placingTrap)
        {
            Debug.Log("90s!");
            if (Vector2.Distance(nextTrapPos, new Vector2(mousePos.x, mousePos.y - yOffSet)) < reqMouseDistanceToPlace)
            {
                float rotation = 0;
                if (currentTrap.GetComponent<Trap>().GetRotation() == 1)
                {
                    rotation = 180;
                }
                uiManager.UseTrap(trapName, true);
                GameObject newBuild = Instantiate(currentTrap, nextTrapPos, Quaternion.Euler(0, rotation, 0), trapDaddy.transform);
                Debug.Log("BEW BUILD!" + newBuild);
                UpdateValidTrapSpawns(new Vector2(nextTrapPos.x, nextTrapPos.y - yOffSet), false); 
                Trap trap = newBuild.GetComponent<Trap>();

                towerBuilder.SetAlpha(newBuild, 1);
                trap.EnableTrap();
                Vector3 particleSpawnPos = newBuild.transform.position;
                particle.gameObject.transform.position = new Vector3(particleSpawnPos.x, particleSpawnPos.y, particleSpawnPos.z - 2);
                particle.GetComponent<ParticleSystem>().Clear();
                particle.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                uiManager.UseTrap(trapName, false);
            }
            EndPlacement();
        }
    }
    public void CurrentTrap(string newTrapName, bool rotation)
    {
        Debug.Log(newTrapName + "UNO");
        placingTrap = true;

        GameObject activeTrap = traps[0];
        foreach (GameObject trap in traps)
        {
            if (trap.name == newTrapName)
            {
                activeTrap = trap;
            }
        }
        yOffSet = activeTrap.GetComponent<Trap>().GetYOffset();
        trapName = activeTrap.name;
        GameObject newTrap = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
        newTrap.GetComponent<Trap>().DisableTrap();
        towerBuilder.SetAlpha(newTrap, opacity);
        currentTrap = newTrap;
        isLarge = currentTrap.GetComponent<Trap>().GetTrapSize();

        if (rotation)
        {
            Debug.Log("LOLOL");
            currentTrap.GetComponent<Trap>().Rotate();
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

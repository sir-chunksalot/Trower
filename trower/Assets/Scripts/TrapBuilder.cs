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
    private void Start()
    {
        trapSpawnLocations = new List<Vector2>();
        totalSpawnLocations = new List<Vector2>();
        placedTrapsPos = new List<Vector2>();

        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        trapSelect = gameObject.GetComponent<TrapSelect>();
        uiManager = gameObject.GetComponent<UIManager>();
        towerBuilder.onTowerPlace += NewRoom;

    }

    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 closestRoom;

        if (placingTrap)
        {
            Debug.Log("PLACING");
            if (isLarge)
            {
                closestRoom = towerBuilder.ClosestTo(mousePos, towerBuilder.GetPlacedFloors().ToArray(), false);
            }
            else
            {
                closestRoom = towerBuilder.ClosestTo(mousePos, totalSpawnLocations, false);
            }

            if (Vector2.Distance(closestRoom, mousePos) < reqMouseDistanceToPlace) {
                currentTrap.transform.position = new Vector3(closestRoom.x, closestRoom.y, 5);
            }
            else {

                currentTrap.transform.position = mousePos;
            }
                

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

    private void UpdateValidTrapSpawns(Vector2 pos, bool newSpawn)
    {
        Vector2 closestTrap = towerBuilder.ClosestTo(mousePos, placedTrapsPos, false);
        if (Vector2.Distance(pos, towerBuilder.GetDoorPos()) <= 5)
        {
            placedTrapsPos.Add(pos);
            if (trapSpawnLocations.Contains(pos)) {
                //trapSpawnLocations.Remove(pos);
            }

        }
        if (newSpawn)
        {
            totalSpawnLocations.Add(pos);
            Debug.Log(pos + " " + "closestTrap" + closestTrap + "NIGHTONFIREW");
            if (Vector2.Distance(pos, closestTrap) >= 5)
            {

                trapSpawnLocations.Add(pos);
            }
        }
        else
        {
            placedTrapsPos.Add(pos);
            if (trapSpawnLocations.Contains(pos))
            {
                //trapSpawnLocations.Remove(pos);
            }
        }
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
        Vector3 roomPos = currentTrap.transform.position;
        nextTrapPos = new Vector3(nextTrapPos.x, nextTrapPos.y, 8);

        if((Vector2)nextTrapPos != (Vector2)currentTrap.transform.position) {
            Debug.Log("80s!");
            EndPlacement();
            return;
        }
        if (placingTrap)
        {
            Debug.Log("90s!");
            if (Vector2.Distance(nextTrapPos, mousePos) < reqMouseDistanceToPlace)
            {
                float rotation = 0;
                if (currentTrap.GetComponent<Trap>().GetRotation() == 1)
                {
                    rotation = 180;
                }
                uiManager.UseTrap(trapName);
                GameObject newBuild = Instantiate(currentTrap, nextTrapPos, Quaternion.Euler(0, rotation, 0));
                UpdateValidTrapSpawns(roomPos, false); 
                Trap trap = newBuild.GetComponent<Trap>();

                if (trap.GetCost() <= Coins.GetCoins())
                {
                    towerBuilder.SetAlpha(newBuild, 1);
                    trap.EnableTrap();
                    Coins.ChangeCoins(-1 * trap.GetCost());

                    Vector3 particleSpawnPos = newBuild.transform.position;
                    particle.gameObject.transform.position = new Vector3(particleSpawnPos.x, particleSpawnPos.y, particleSpawnPos.z - 2);
                    particle.GetComponent<ParticleSystem>().Clear();
                    particle.GetComponent<ParticleSystem>().Play();

                    trapSelect.DeselectOne(trap.name);
                }
                else
                {
                    Destroy(newBuild);
                }

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
        placingTrap = false;
        trapName = "";
        Destroy(currentTrap);
    }
}

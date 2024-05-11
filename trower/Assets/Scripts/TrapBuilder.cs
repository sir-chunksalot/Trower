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
    private List<Vector3> trapSpawnLocations;
    private bool placingTrap;
    private bool isLarge;
    private Vector2 mousePos;

    private void Start()
    {
        trapSpawnLocations = new List<Vector3>();

        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        trapSelect = gameObject.GetComponent<TrapSelect>();
        towerBuilder.onTowerPlace += GenerateTrapSpawns;

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

            currentTrap.transform.position = new Vector3(closestRoom.x, closestRoom.y, 5);

        }
    }

    private void GenerateTrapSpawns(object sender, EventArgs e)
    {
        foreach (GameObject placedFloor in towerBuilder.GetPlacedFloors())
        {
            Vector3 spawnLeft = new Vector3(placedFloor.transform.position.x - 5, placedFloor.transform.position.y, placedFloor.transform.position.z);
            Vector3 spawnRight = new Vector3(placedFloor.transform.position.x + 5, placedFloor.transform.position.y, placedFloor.transform.position.z);
            if (!trapSpawnLocations.Exists(item => item == spawnLeft))
            {
                trapSpawnLocations.Add(spawnLeft);
            }
            if (!trapSpawnLocations.Exists(item => item == spawnRight))
            {
                trapSpawnLocations.Add(spawnRight);
            }
            trapSpawnLocations.Add(placedFloor.transform.position);
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
        Vector3 nextTrapPos = currentTrap.transform.position;
        nextTrapPos = new Vector3(nextTrapPos.x, nextTrapPos.y, 8);

        if (placingTrap)
        {
            if (Vector2.Distance(nextTrapPos, mousePos) < reqMouseDistanceToPlace)
            {
                float rotation = 0;
                if (currentTrap.GetComponent<Trap>().GetRotation() == 1)
                {
                    rotation = 180;
                }
                GameObject newBuild = Instantiate(currentTrap, nextTrapPos, Quaternion.Euler(0, rotation, 0));
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
    public void CurrentTrap(string trapName, bool rotation)
    {
        Debug.Log(trapName + "UNO");
        placingTrap = true;

        GameObject activeTrap = traps[0];
        foreach (GameObject trap in traps)
        {
            if (trap.name == trapName)
            {
                activeTrap = trap;
            }
        }
        GameObject newTrap = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
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
        Destroy(currentTrap);
    }
}

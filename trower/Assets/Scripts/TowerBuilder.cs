using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] GameObject[] traps; //FOR TRAP TYPES
    [SerializeField] GameObject[] floors; //FOR FLOOR TYPES
    [SerializeField] GameObject potentialFloor;
    [SerializeField] float opacity;
    [SerializeField] GameObject particle;
    [SerializeField] float sideMapBounds;
    [SerializeField] float topMapBounds;
    [SerializeField] float reqMouseDistanceToPlace;
    [SerializeField] GameObject door;
    [SerializeField] GameObject sellBomb;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject UIparent;

    public event EventHandler onTowerPlace;
    public event EventHandler onTowerSell;

    GameObject currentFloor;
    GameObject sellBombObject;
    CameraController cameraController;
    UIManager uiManager;

    private List<GameObject> placedFloors = new List<GameObject>(); //LIST OF PLACED INDIVIDUAL FLOORS
    private List<GameObject> floorPapas = new List<GameObject>(); //LIST OF PLACED GROUPED FLOOR TYPES
    //private List<GameObject> trapTypes = new List<GameObject>();
    private List<GameObject> floorTypes = new List<GameObject>();//assigns the floor types at runtime
    private List<GameObject> potentialFloors = new List<GameObject>();//potential spots a floor could spawn (saved as instantiated gameobjects in valid spots, i did this for debugging)
    private List<GameObject> ladders = new List<GameObject>();
    List<Vector3> floorSpawnSpots = new List<Vector3>();
    Vector3 nextFloorPos;
    Vector3 highestPoint;
    Vector2 mousePos;

    private string floorName;
    private bool goingUp;
    private bool placingFloor;
    private bool placingBomb;
    private int ladderType;
    private int totalPlacedFloorsCount;

    private void Start()
    {
        uiManager = gameObject.GetComponent<UIManager>();
        sellBombObject = Instantiate(sellBomb, Vector2.zero, Quaternion.identity);
        SetAlpha(sellBombObject, 0);
        ladderType = 2;
        cameraController = Camera.main.GetComponent<CameraController>();
        StartCoroutine(GenerateNewWalls());

        GameObject oldFloorsDad = GameObject.FindGameObjectWithTag("BuildDaddy");
        if(oldFloorsDad != null)
        {
            Debug.Log("dad found");
            foreach (Floor floor in oldFloorsDad.GetComponentsInChildren<Floor>())
            {
                if (floor.gameObject.tag == "Build")
                {
                    AddPlacedFloor(floor.gameObject);
                    floorSpawnSpots.Add(floor.gameObject.transform.position);
                }
            }
        }
    }

    public void Place(InputAction.CallbackContext context) //called when mouse is released
    {
        if (context.performed)
        {
            if (placingFloor)
            {
                PlaceBuild(currentFloor);
                PlaceDoor();
                Cleanup();
            }
            placingFloor = false;
            if (placingBomb)
            {
                SellFloor();
                Cleanup();
            }
        }

    }

    private void DestroyOverlap(GameObject newFloor)
    {
        foreach (GameObject floor in placedFloors)
        {
            if (floor.transform.position == newFloor.transform.position)
            {
                Destroy(floor);
                placedFloors.Remove(floor);
                return;
            }
        }
    }

    public void PlaceBuild(GameObject build)
    {
        float cost = currentFloor.GetComponent<Floor>().GetCost();
        if (placingFloor)
        {
            nextFloorPos = ClosestTo(currentFloor.transform.position, potentialFloors.ToArray(), true);
        }


        if (Vector2.Distance(nextFloorPos, mousePos) < reqMouseDistanceToPlace)
        {
            Vector3 particleSpawnPos = Vector3.zero;

            if (placingFloor)
            {
                GameObject papa = new GameObject();
                papa.name = build.name + "#" + totalPlacedFloorsCount;
                floorPapas.Add(papa);

                totalPlacedFloorsCount++;

                Debug.Log("goose");
                int count = 0;
                foreach (Transform child in build.GetComponentInChildren<Transform>())
                {
                    count++;
                    Debug.Log("HELLOW BABY");
                    uiManager.UseTrap(floorName, true);
                    GameObject newBuild = Instantiate(child.gameObject, nextFloorPos, Quaternion.identity, papa.transform);
                    newBuild.transform.position += child.transform.position - build.transform.position;
                    newBuild.transform.position = new Vector3(newBuild.transform.position.x, newBuild.transform.position.y, 10);
                    SetAlpha(newBuild, 1);
                    DestroyOverlap(newBuild);
                    placedFloors.Add(newBuild);


                    particleSpawnPos = newBuild.transform.position;
                }
            }
            else
            {
                uiManager.UseTrap(floorName, false);
            }

            particle.gameObject.transform.position = new Vector3(particleSpawnPos.x, particleSpawnPos.y, particleSpawnPos.z - 2);
            particle.GetComponent<ParticleSystem>().Clear();
            particle.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            uiManager.UseTrap(floorName, false);
            Debug.Log("Mission Failed. We'll get em' next time (failed to build structure)");
        }
    }

    public void CurrentFloor(string floorName) //CALLED FIRST BY THE BUILD SELECT SCRIPT
    {
        NewPotentialFloors();
        placingFloor = true;


        if (floorTypes.Count == 0)
        {
            foreach (GameObject floor in floors)
            {
                Debug.Log(floor.name);
                GameObject newFloor = Instantiate(floor, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
                floorTypes.Add(newFloor);
                newFloor.SetActive(false);
            }
        }

        GameObject activeFloor = floorTypes[0];

        foreach (GameObject floor in floorTypes)
        {
            Debug.Log(floor.name + " " + floorName);
            if (floor.name == floorName)
            {
                activeFloor = floor;
            }
        }
        this.floorName = floorName.Substring(0, floorName.Length - 7);
        Debug.Log("DOOFEN activefloorname" + floorName);
        int index = floorTypes.IndexOf(activeFloor);
        Debug.Log("LUFFY: " + index + " " + activeFloor + " " + ladderType + " " + goingUp);

        if (currentFloor != null) { currentFloor.SetActive(false); }

        currentFloor = activeFloor;
        floorSpawnSpots.Clear();
        foreach (Transform child in currentFloor.GetComponentInChildren<Transform>())
        {
            Vector3 newFloorPos = new Vector3(child.localPosition.x, child.localPosition.y, placedFloors[0].transform.position.z);
            floorSpawnSpots.Add(newFloorPos);
        }

        currentFloor.transform.position = new Vector3(mousePos.x, mousePos.y, 10);
        currentFloor.SetActive(true);
        SetAlpha(activeFloor, opacity);

    }

    public void EndPlacement()
    {
        placingFloor = false;
        Cleanup();
    }

    public Vector3 ClosestTo(Vector3 target, GameObject[] objPos, bool validCheck)
    {
        List<Vector3> pos = new List<Vector3>();
        foreach (GameObject p in objPos)
        {
            pos.Add(p.transform.position);
        }
        return ClosestTo(target, pos, validCheck);
    }

    public Vector3 ClosestTo(Vector3 target, List<Vector3> pos, bool validCheck)
    {
        bool valid = true;
        Vector3 closest = new Vector3(100, 100, 100);
        foreach (Vector3 p in pos)
        {
            if ((target - p).magnitude < (target - closest).magnitude)
            {
                if (validCheck)
                {
                    valid = ValidFloorSpawn(p);
                }
                if (valid)
                {
                    closest = p;
                }
            }
        }
        return closest;
    }

    public Vector3 ClosestTo(Vector2 target, List<Vector2> pos, bool validCheck)
    {
        bool valid = true;
        Vector2 closest = new Vector3(100, 100);
        foreach (Vector2 p in pos)
        {
            if ((target - p).magnitude < (target - closest).magnitude)
            {
                if (validCheck)
                {
                    valid = ValidFloorSpawn(p);
                }
                if (valid)
                {
                    closest = p;
                }
            }
        }
        return closest;
    }

    private bool ValidFloorSpawn(Vector3 floorSpawn)
    {
        Vector3 spawnSpot = new Vector3(floorSpawn.x, floorSpawn.y, 0);
        //Debug.Log((ClosestTo(currentFloor.transform.position, placedFloors.ToArray(), false) - currentFloor.transform.position).magnitude + "helldivers");
        //if ((ClosestTo(currentFloor.transform.position, placedFloors.ToArray(), false) - currentFloor.transform.position).magnitude > 12)
        //{
        //    return false;
        //}

        bool hasConnection = false;
        foreach (Vector3 floor in floorSpawnSpots)
        {
            if (GetPlacedFloorsPos().Contains(floor + spawnSpot)) //makes sure that rooms dont try and spawn on top of eachother
            {
                Debug.Log("STINKER");
                return false;
            }
            Debug.Log(floor.x + spawnSpot.x + "owo" + GetRoomBounds().x * sideMapBounds);
            if (floor.x + spawnSpot.x > (GetRoomBounds().x * sideMapBounds) / 2 || floor.x + spawnSpot.x < (GetRoomBounds().x * sideMapBounds * -1) / 2) //checks to see if any room is trying to spawn outside of map bounds
            {
                Debug.Log("STINKER");
                return false;
            }
            foreach (Vector3 placedFloor in GetPlacedFloorsPos())
            {
                if ((placedFloor.x == (floor.x + spawnSpot.x) - GetRoomBounds().x || placedFloor.x == (floor.x + spawnSpot.x) + GetRoomBounds().x) && floor.y + spawnSpot.y == placedFloor.y)
                {
                    Debug.Log(placedFloor + " X FLOOR POS CONNECTION" + floor + spawnSpot);
                    hasConnection = true;
                }
                if ((placedFloor.y == (floor.y + spawnSpot.y) - GetRoomBounds().y || placedFloor.y == (floor.y + spawnSpot.y) + GetRoomBounds().y) && floor.x + spawnSpot.x == placedFloor.x)
                {
                    Debug.Log(placedFloor + " Y FLOOR POS CONNECTION" + floor + spawnSpot);
                    hasConnection = true;
                }
            }
        }

        if (!hasConnection)
        {
            return false;
        }

        return true;
    }

    private void DestroyPotentialFloors()
    {
        foreach (GameObject e in potentialFloors)
        {
            Destroy(e);
        }
        potentialFloors.Clear();
    }
    private void NewPotentialFloors()
    {
        DestroyPotentialFloors();
        Debug.Log("SHITCUM");
        Vector3 right = new Vector3(GetRoomBounds().x, 0, 0);
        Vector3 up = new Vector3(0, GetRoomBounds().y, 0);
        Vector3 spawnPos = new Vector3(-((sideMapBounds * right.x) / 2), -up.y, 1);
        for (int i = 0; i < topMapBounds; i++)
        {
            spawnPos = spawnPos + up;
            for (int j = 0; j < sideMapBounds + 1; j++)
            {
                potentialFloors.Add(Instantiate(potentialFloor, spawnPos, Quaternion.identity));
                spawnPos = spawnPos + right;
            }
            spawnPos = spawnPos - (right * sideMapBounds) - right;
        }
        float highestPoint = -5;
        foreach (Vector3 floor in GetPlacedFloorsPos())
        {
            if (highestPoint < floor.y)
            {
                highestPoint = floor.y;
            }
        }
        bool invalid;
        GameObject[] potFloorList = potentialFloors.ToArray();
        foreach (GameObject potentialFloor in potFloorList)
        {
            invalid = false;

            foreach (GameObject floor in placedFloors)
            {
                Debug.Log(potentialFloor.transform.position + " dqagplayz" + floor.transform.position);
                if (potentialFloor.transform.position.x == floor.transform.position.x && potentialFloor.transform.position.y == floor.transform.position.y) //makes sure there arent any pot floors in the same spot as placed floors
                {
                    invalid = true;
                }

            }
            if (potentialFloor.transform.position.y > highestPoint) //makes sure there arent any pot floors above the tiower
            {
                invalid = true;
            }
            //if (potentialFloor.transform.position.x > (sideMapBounds * GetRoomBounds().x) / 2 || potentialFloor.transform.position.x < (sideMapBounds * GetRoomBounds().x * -1) / 2)
            //{
            //    invalid = true;
            //}
            if (invalid)
            {
                potentialFloors.Remove(potentialFloor);
                Destroy(potentialFloor);
            }
        }
    }

    private void PlaceDoor()
    {
        foreach (GameObject floor in placedFloors)
        {
            if (floor.transform.position.y > highestPoint.y)
            {
                highestPoint = floor.transform.position;
                door.transform.position = new Vector3(highestPoint.x, highestPoint.y, door.transform.position.z);
            }
        }
    }
    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 closestFloor = ClosestTo(mousePos, potentialFloors.ToArray(), true);
        if (Vector2.Distance(closestFloor, mousePos) > reqMouseDistanceToPlace)
        {
            closestFloor.x = mousePos.x;
            closestFloor.y = mousePos.y;
        }


        if (!placingFloor)
        {
            if (currentFloor != null)
            {
                currentFloor.SetActive(false);
            }
        }
        else
        {
            if (currentFloor != null)
            {
                currentFloor.transform.position = new Vector3(closestFloor.x, closestFloor.y, 9);
            }
        }

        if (placingBomb)
        {
            sellBombObject.transform.position = new Vector3(mousePos.x, mousePos.y, 9);
            Vector3 floorBombIsNearest = ClosestTo(sellBombObject.transform.position, placedFloors.ToArray(), false);

            Transform closestPapa = FindClosestFloorToMe(sellBombObject).transform.parent;
            Debug.Log(Vector2.Distance(sellBombObject.transform.position, floorBombIsNearest));

            if (Vector2.Distance(sellBombObject.transform.position, floorBombIsNearest) > reqMouseDistanceToPlace)
            {
                closestPapa = gameObject.transform; //if the papa is too far away it defaults to the game manager game object, which means there are no valid floor, which means they all turn white
            }

            Debug.Log(closestPapa);
            foreach (GameObject floor in placedFloors)
            {
                Transform parent = floor.transform;
                if (floor.transform.parent != null)
                {
                    parent = floor.transform.parent;
                }

                Debug.Log("floor" + floor + "paprent" + floor.transform.parent + "link");
                if (parent == closestPapa)
                {
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        parent.GetChild(i).GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
                else
                {
                    for (int i = 0; i < parent.transform.childCount; i++)
                    {
                        parent.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }

        }

        Debug.Log(placedFloors.Count + " COUNT");

    }

    private GameObject FindClosestFloorToMe(GameObject me)
    {
        Vector3 closestFloor = ClosestTo(sellBombObject.transform.position, placedFloors.ToArray(), false);
        foreach (GameObject floor in placedFloors)
        {
            if (floor.transform.position == closestFloor)
            {
                return floor;
            }
        }

        return null;
    }

    private void CheckForConnections()
    {
        GenerateViableFloors genViableFloors = this.gameObject.GetComponent<GenerateViableFloors>();
        genViableFloors.Test();
        //List<GameObject> invalidFloors = new List<GameObject>();
        //bool validLadderFound = false;

        //foreach (GameObject floor in placedFloors)
        //{
        //    foreach (Transform kid in floor.GetComponentsInChildren<Transform>())
        //    {
        //        if(genViableFloors.GetViableLadders().Contains(kid.gameObject)) {
        //            Debug.Log("FoundViableLadder");
        //            validLadderFound = true;
        //        }
        //    }
        //    if(!validLadderFound) {
        //        invalidFloors.Add(floor.transform.parent.gameObject);
        //    }
        //}

        //invalidFloors.Add(gameObject);
        //Debug.Log("Brealk;");
        //foreach(GameObject f in invalidFloors)
        //{
        //    Debug.Log("Invalid Floor" + f);
        //}

    }

    public void EquipBomb()
    {
        SetAlpha(sellBombObject, 100);
        placingBomb = true;
        Debug.Log("tried to equip bomb");
    }

    private void SellFloor()
    {
        Debug.Log("tried to sell");

        GameObject targetFloor = FindClosestFloorToMe(sellBombObject);
        Debug.Log("targer floor" + targetFloor);
        Vector3 explosionSpawnSpot = targetFloor.transform.position - new Vector3(0, 10);
        Vector3 floorBombIsNearest = ClosestTo(sellBombObject.transform.position, placedFloors.ToArray(), false);

        if (targetFloor != null && Vector2.Distance(sellBombObject.transform.position, floorBombIsNearest) < reqMouseDistanceToPlace)
        {
            //cameraController.AddShake(1.8f);
            if (targetFloor.transform.parent == null)
            {
                return;
            }
            foreach (Transform child in targetFloor.transform.parent)
            {
                Destroy(child.gameObject);
                placedFloors.Remove(child.gameObject);
            }
            floorPapas.Remove(targetFloor.transform.parent.gameObject);
            Destroy(targetFloor.transform.parent.gameObject);


            GameObject explody = Instantiate(explosionEffect, Vector3.zero, Quaternion.identity, UIparent.transform);
            explody.transform.position = explosionSpawnSpot * 25;
            onTowerSell?.Invoke(gameObject, EventArgs.Empty);

            StartCoroutine(DestroyAfterTime(explody, 1.5f));
        }
        placingBomb = false;
        SetAlpha(sellBombObject, 0);
    }

    public void SetAlpha(GameObject floor, float alpha)
    {
        foreach (SpriteRenderer s in floor.transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (s.gameObject.tag == "Vines") continue;
            Color opacity = s.color;
            opacity.a = alpha;
            s.color = opacity;
        }

    }

    //public List<GameObject> GetLaders()
    //{
    //    return ladders;
    //}

    //public void AddLadders(GameObject ladder) //used by ladder enable script (the script that makes it so enemies cant hop on ladders prematurely)
    //{
    //    ladders.Add(ladder);
    //}

    //public void RemoveLadders(GameObject ladder)
    //{
    //    if (ladders.Contains(ladder))
    //    {
    //        ladders.Remove(ladder);
    //    }
    //}
    public List<GameObject> GetPlacedFloors()
    {
        return placedFloors;
    }

    public GameObject GetFloor(Vector3 pos)
    {
        foreach (GameObject floor in placedFloors)
        {
            if (floor.transform.position == pos)
            {
                return floor;
            }
        }

        return null;
    }

    public List<Vector3> GetPlacedFloorsPos()
    {
        List<Vector3> placedFloorsPos = new List<Vector3>();
        List<GameObject> floorLoop = placedFloors;
        foreach (GameObject floor in floorLoop)
        {
            if (floor != null)
            {
                placedFloorsPos.Add(floor.transform.position);
            }
        }
        return placedFloorsPos;
    }

    public List<Vector3> GetPotentialFloorsPos()
    {
        List<Vector3> potentialFloorsPos = new List<Vector3>();
        foreach (GameObject floor in potentialFloors)
        {
            potentialFloorsPos.Add(floor.transform.position);
        }
        return potentialFloorsPos;
    }

    public Vector3 GetDoorPos()
    {
        Debug.Log("door pos" + door.transform.position + gameObject);
        return door.transform.position;
    }

    public Vector3 GetInitialFloorPos()
    {
        return placedFloors[0].transform.position;
    }

    public Vector2 GetRoomBounds()
    {
        return new Vector2(10, 5);
    }

    public bool GetIsPlacingFloor()
    {
        return placingFloor;
    }

    public void AddPlacedFloor(GameObject floor) //used by other scripts whenever a floor is spawned in the gameworld without this scripts permission. its added to the list so other scripts can see it 
    {
        placedFloors.Add(floor);
    }

    private IEnumerator GenerateNewWalls() //for some reason you have to wait a little bit or it breaks. dont ask me
    {
        yield return new WaitForSeconds(.01f);
        onTowerPlace?.Invoke(gameObject, EventArgs.Empty);
    }

    private IEnumerator DestroyAfterTime(GameObject target, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(target);
    }


    private void Cleanup()
    {
        DestroyPotentialFloors();

        foreach (GameObject floor in floorTypes)
        {
            Destroy(floor);
        }
        floorTypes.Clear();
        StartCoroutine(GenerateNewWalls());
    }


}

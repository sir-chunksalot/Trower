using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerBuilder : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] GameObject[] traps; //FOR TRAP TYPES
    [SerializeField] GameObject[] floors; //FOR FLOOR TYPES
    [SerializeField] GameObject potentialFloor;
    [SerializeField] float opacity;
    [SerializeField] GameObject particle;
    [SerializeField] float sideMapBounds;
    [SerializeField] float topMapBounds;
    [SerializeField] float reqMouseDistanceToPlace;
    [SerializeField] GameObject sellBomb;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] GameObject debugThing;

    public event EventHandler onTowerPlace;
    public event EventHandler onTowerPlaceLate;
    public event EventHandler onTowerPlaceLast;
    public event EventHandler onTowerSell;
    public event EventHandler onTowerStart;

    GameObject currentFloor;
    GameObject sellBombObject;
    GameObject buildDaddy;
    CameraController cameraController;
    UIManager uiManager;

    private List<GameObject> placedFloors = new List<GameObject>(); //LIST OF PLACED INDIVIDUAL FLOORS
    private List<Vector3> placedFloorsPos = new List<Vector3>(); //LIST OF PLACED INDIVIDUAL FLOOR POSISTIONS
    private List<GameObject> floorPapas = new List<GameObject>(); //LIST OF PLACED GROUPED FLOOR TYPES
    private List<GameObject> debugFloors = new List<GameObject>();
    private List<GameObject> debugFloorPos = new List<GameObject>();
    //private List<GameObject> trapTypes = new List<GameObject>();
    private List<GameObject> potentialFloors = new List<GameObject>();//potential spots a floor could spawn (saved as instantiated gameobjects in valid spots, i did this for debugging) ||CLEARED EVERY TOWER PLACE
    private List<Vector3> mapPotentialFloors = new List<Vector3>();//potential spots a floor could spawn (saved as instantiated gameobjects in valid spots, i did this for debugging) ||CLEARED ON LEVEL COMPLETION
    private List<GameObject> ladders = new List<GameObject>();
    List<Vector3> floorSpawnSpots = new List<Vector3>();
    Vector3 nextFloorPos;
    float highestPoint;
    float floorSpawnZ;
    Vector2 mousePos;

    private string floorName;
    private bool goingUp;
    private bool canUndo;
    private bool canPlace;
    private bool placingFloor;
    private bool placingBomb;
    private int ladderType;
    private bool cleanupLate;
    private bool cleanupLast;
    private int cleanupCount;
    private int totalPlacedFloorsCount;
    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;
        gameManager.OnSceneUnloaded += OnSceneUnloaded;
        floorSpawnZ = 10;
    }
    private void Start()
    {
        canUndo = true;
        canPlace = true;
        uiManager = gameObject.GetComponent<UIManager>();
        sellBombObject = Instantiate(sellBomb, Vector2.zero, Quaternion.identity);
        SetAlpha(sellBombObject, 0);
        ladderType = 2;
        cameraController = Camera.main.GetComponent<CameraController>();
    }
    public void OnSceneUnloaded(object sender, EventArgs e)
    {

    }
    public void OnSceneLoad(object sender, EventArgs e)
    {
        placedFloorsPos.Clear();
        floorPapas.Clear();
        potentialFloors.Clear();
        mapPotentialFloors.Clear();
        buildDaddy = gameManager.GetCurrentLevelDetails().GetBuildDaddy();
        foreach (GameObject floor in placedFloors)
        {
            Destroy(floor);
        }
        placedFloors.Clear();
        if (buildDaddy != null)
        {
            foreach (Transform kid in buildDaddy.transform)
            {
                floorPapas.Add(kid.gameObject);


                foreach (Transform floor in kid)
                {
                    AddPlacedFloor(floor.gameObject);
                }
                onTowerPlace.Invoke(kid.gameObject, EventArgs.Empty);
            }
        }
        GenerateMapSpawns();
        onTowerStart?.Invoke(gameObject, EventArgs.Empty);
    }

    public void Place(InputAction.CallbackContext context) //called when mouse is released
    {
        if (context.performed)
        {
            if (placingFloor)
            {
                PlaceBuild(currentFloor);
            }
            placingFloor = false;
        }

    }
    public void PlaceBuild(GameObject build)
    {
        GameObject newBuild = build;
        if (placingFloor)
        {
            nextFloorPos = ClosestTo(currentFloor.transform.position, (GameObject[])potentialFloors.ToArray().Clone(), true);
        }


        if (Vector2.Distance(nextFloorPos, mousePos) < reqMouseDistanceToPlace)
        {
            Vector3 particleSpawnPos = Vector3.zero;

            if (placingFloor)
            {
                floorPapas.Add(build);
                Debug.Log("HELLOW BABY");
                uiManager.UseCard(floorName, true);
                nextFloorPos = new Vector3(nextFloorPos.x, nextFloorPos.y, 10);
                newBuild = Instantiate(build, nextFloorPos, Quaternion.identity, buildDaddy.transform);
                newBuild.GetComponent<Floor>().PlaceFloor();
                SetAlpha(newBuild, 1);
                foreach(Transform floor in newBuild.transform)
                {
                    AddPlacedFloor(floor.gameObject);
                    Debug.Log("owen klanke is sleepy" + floor);
                    floor.gameObject.name = floor.gameObject.name + "#" + UnityEngine.Random.Range(0, 10000);

                    floor.transform.position = new Vector3(Mathf.Round(floor.transform.position.x), Mathf.Round(floor.transform.position.y), floor.transform.position.z);
                }
                particleSpawnPos = newBuild.transform.position;
            }
            else
            {
                uiManager.UseCard(floorName, false);
            }

            particle.gameObject.transform.position = new Vector3(particleSpawnPos.x, particleSpawnPos.y, particleSpawnPos.z - 2);
            particle.GetComponent<ParticleSystem>().Clear();
            particle.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            uiManager.UseCard(floorName, false);
            Destroy(currentFloor);
            Debug.Log("Mission Failed. We'll get em' next time (failed to build structure)");
        }

        Cleanup(newBuild);
        StartCoroutine(PlaceDelay());
    }

    public void CurrentFloor(string floorName) //CALLED FIRST BY THE BUILD SELECT SCRIPT
    {
        if(currentFloor != null) { Destroy(currentFloor); }
        if (!canPlace) return;
        NewPotentialFloors();
        placingFloor = true;

        GameObject activeFloor = floors[0];

        foreach (GameObject floor in floors)
        {
            if (floor.name + "(Clone)" == floorName)
            {
                activeFloor = Instantiate(floor, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            }
        }
        this.floorName = floorName.Substring(0, floorName.Length - 7);
        if (currentFloor != null) { currentFloor.SetActive(false); }

        currentFloor = activeFloor;
        floorSpawnSpots.Clear();
        foreach (Transform child in currentFloor.GetComponentInChildren<Transform>())
        {
            Vector3 newFloorPos = new Vector3(child.localPosition.x, child.localPosition.y, placedFloors[0].transform.position.z);
            floorSpawnSpots.Add(newFloorPos);
        }

        currentFloor.transform.position = new Vector3(mousePos.x, mousePos.y, floorSpawnZ);
        currentFloor.SetActive(true);
        SetAlpha(activeFloor, opacity);
    }

    private IEnumerator PlaceDelay()
    {
        canPlace = false;
        yield return new WaitForSeconds(.3f);
        canPlace = true;
    }

    public void EndPlacement()
    {
        if(placingFloor)
        {
            placingFloor = false;
            Cleanup(null);
        }
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
                return false;
            }
            if (floor.x + spawnSpot.x > (GetRoomBounds().x * sideMapBounds) / 2 || floor.x + spawnSpot.x < (GetRoomBounds().x * sideMapBounds * -1) / 2) //checks to see if any room is trying to spawn outside of map bounds
            {
                return false;
            }
            foreach (Vector3 placedFloor in GetPlacedFloorsPos())
            {
                if ((placedFloor.x == (floor.x + spawnSpot.x) - GetRoomBounds().x || placedFloor.x == (floor.x + spawnSpot.x) + GetRoomBounds().x) && floor.y + spawnSpot.y == placedFloor.y)
                {
                    //Debug.Log(placedFloor + " X FLOOR POS CONNECTION" + floor + spawnSpot);
                    hasConnection = true;
                }
                if ((placedFloor.y == (floor.y + spawnSpot.y) - GetRoomBounds().y || placedFloor.y == (floor.y + spawnSpot.y) + GetRoomBounds().y) && floor.x + spawnSpot.x == placedFloor.x)
                {
                    //Debug.Log(placedFloor + " Y FLOOR POS CONNECTION" + floor + spawnSpot);
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

    public void Undo()
    {
        if(canUndo)
        {
            GameObject floor = placedFloors[placedFloors.Count - 1];
            DestroyFloor(floor);
            StartCoroutine(UndoDelay());
        }

    }

    private IEnumerator UndoDelay() {

        canUndo = false;
        yield return new WaitForSeconds(.5f);
        canUndo = true;
    }
    private void DestroyFloor(GameObject floor)
    {
        if (placedFloors.Contains(floor))
        {
            GameObject floorDad = floor.transform.parent.gameObject;
            foreach (Transform childFloor in floorDad.transform)
            {
                int index = placedFloors.IndexOf(childFloor.gameObject);
                placedFloors.RemoveAt(index);
                placedFloorsPos.RemoveAt(index);
                Vector3 explosionSpawnPos = new Vector3(childFloor.transform.position.x, childFloor.transform.position.y, Camera.main.transform.position.z + 1);
                GameObject explosion = Instantiate(explosionEffect, explosionSpawnPos, Quaternion.identity);
                StartCoroutine(DestroyExplosionEffect(explosion));
            }
            Debug.Log("floor dad!!" + floorDad);
            floorPapas.Remove(floorDad);

            floorDad.GetComponent<Floor>().DestroyFloor();

            float highestFloor = 0;
            foreach(Vector3 floorPos in placedFloorsPos)
            {
                if(floorPos.y > highestFloor)
                {
                    highestFloor = floorPos.y;
                }
            }
            highestPoint = highestFloor;
        }
        else
        {
            Debug.Log("ERROR! TowerBuilder tried to destroy " + floor + " but it does not exist.");
        }

        onTowerSell?.Invoke(floor, EventArgs.Empty);
        Cleanup(floor);
        DestroyPotentialFloors();
    }

    private IEnumerator DestroyExplosionEffect(GameObject explosion)
    {
        yield return new WaitForSeconds(2);
        Destroy(explosion);
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
        if(mapPotentialFloors.Count <= 0) { return; }
        DestroyPotentialFloors();
       
        foreach(Vector3 mapSpawn in mapPotentialFloors)
        {
            if(!placedFloorsPos.Contains(mapSpawn) && mapSpawn.y <= highestPoint)
            {
                potentialFloors.Add(Instantiate(potentialFloor, new Vector3(mapSpawn.x, mapSpawn.y, 1), Quaternion.identity));
            }
        }
    }

    public float GetHighestPoint()
    {
        return highestPoint;
    }

    private void GenerateMapSpawns()
    {
        Vector3 right = new Vector3(GetRoomBounds().x, 0, 0);
        Vector3 up = new Vector3(0, GetRoomBounds().y, 0);
        Vector3 spawnPos = new Vector3(-((sideMapBounds * right.x) / 2), -up.y, floorSpawnZ);
        for (int i = 0; i < topMapBounds; i++)
        {
            spawnPos = spawnPos + up;
            for (int j = 0; j < sideMapBounds + 1; j++)
            {
                mapPotentialFloors.Add(spawnPos);
                spawnPos = spawnPos + right;
            }
            spawnPos = spawnPos - (right * sideMapBounds) - right;
        }
    }
    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 closestFloor = ClosestTo(mousePos, (GameObject[])potentialFloors.ToArray().Clone(), true);
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
            Vector3 floorBombIsNearest = ClosestTo(sellBombObject.transform.position, (GameObject[])placedFloors.ToArray().Clone(), false);

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

    }

    private void LateUpdate()
    {
        if(cleanupLast)
        {
            if(cleanupCount > 16 && cleanupLate) //WE LOVE 16!!!!
            {
                Debug.Log("we gotta late update in here fellas");
                onTowerPlaceLate?.Invoke(gameObject, EventArgs.Empty);
                cleanupLate = false;
            }
            if(cleanupCount > 32)
            {
                onTowerPlaceLast.Invoke(gameObject, EventArgs.Empty);
                cleanupLast = false;
                cleanupCount = 0;
            }
            cleanupCount++;

        }
    }



    private GameObject FindClosestFloorToMe(GameObject me)
    {
        Vector3 closestFloor = ClosestTo(sellBombObject.transform.position, (GameObject[])placedFloors.ToArray().Clone(), false);
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

    //public void SellFloor(GameObject targetFloor)
    //{
    //    Debug.Log("targer floor" + targetFloor);
    //    Vector3 explosionSpawnSpot = targetFloor.transform.position - new Vector3(0, 10);

    //    if (targetFloor != null)
    //    {
    //        //cameraController.AddShake(1.8f);
    //        if (targetFloor.transform.parent == null) { return; }
    //        //foreach (Transform child in targetFloor.transform.parent)
    //        //{
    //        //    Debug.Log("KIlled kid" + child);
    //        //    Destroy(child.gameObject);
    //        //    
    //        //    placedFloors.Remove(child.gameObject);
    //        //}
    //        floorPapas.Remove(targetFloor.transform.parent.gameObject);
    //        placedFloors.Remove(targetFloor);
    //        targetFloor.GetComponent<Floor>().DestroyFloor();
    //        //StartCoroutine(DestroyAfterTime(targetFloor.transform.parent.gameObject));
    //        onTowerSell?.Invoke(targetFloor, EventArgs.Empty);
    //        //Destroy(targetFloor.transform.parent.gameObject);


    //        //GameObject explody = Instantiate(explosionEffect, Vector3.zero, Quaternion.identity, UIparent.transform);
    //        //explody.transform.position = explosionSpawnSpot * 25;


    //        //StartCoroutine(DestroyAfterTime(explody, 1.5f));
    //    }
    //    placingBomb = false;
    //    if (sellBombObject != null)
    //    {
    //        SetAlpha(sellBombObject, 0);
    //    }
    //}

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
    public List<GameObject> GetPlacedFloors()
    {
        return placedFloors;
    }

    public GameObject GetPlacedFloor(Vector3 floorPos)
    {
        if(placedFloorsPos.Contains(floorPos))
        {
            int index = placedFloorsPos.IndexOf(floorPos);
            return placedFloors[index];
        }
        return null;
    }

    public List<Vector3> GetPlacedFloorsPos()
    {
        return placedFloorsPos;
    }

    public void DebugPotentialFloorLocations()
    {
        foreach(Vector3 placedFloor in placedFloorsPos)
        {
            Debug.Log("-DEV- placedFloor pos" + placedFloor);
        }
        foreach (Vector3 mapSpawn in mapPotentialFloors)
        {
            Debug.Log("-DEV- mapSpawn pos" + mapSpawn);
        }

        Debug.Log("-DEV- potential floor locations. floor count: " + GetPotentialFloorsPos().Count);
        if (potentialFloors.Count >= 1)
        {
            DestroyPotentialFloors();
        }
        else
        {
            NewPotentialFloors();
        }
    }

    public void DebugPlacedFloorLocations()
    {
        Debug.Log("-DEV- floor locations. floor count: " + debugFloors.Count);
        if (debugFloors.Count >= 1)
        {
            foreach (GameObject floor in debugFloors)
            {
                Destroy(floor);
            }
            debugFloors.Clear();
        }
        else
        {
            foreach(GameObject floor in placedFloors)
            {
                GameObject debugFloor = Instantiate(debugThing, new Vector3(floor.transform.position.x, floor.transform.position.y, Camera.main.transform.position.z + 1), Quaternion.identity);
                debugFloors.Add(debugFloor);
            }
        }
    }

    public void DebugPlacedFloorPosLocations()
    {
        Debug.Log("-DEV- floor pos. floor count: " + debugFloors.Count);
        if (debugFloorPos.Count >= 1)
        {
            foreach (GameObject floor in debugFloorPos)
            {
                Destroy(floor);
            }
            debugFloorPos.Clear();
        }
        else
        {
            foreach (Vector3 floor in placedFloorsPos)
            {
                GameObject debugFloor = Instantiate(debugThing, new Vector3(floor.x, floor.y, Camera.main.transform.position.z + 1), Quaternion.identity);
                debugFloorPos.Add(debugFloor);
            }
        }
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
        placedFloorsPos.Add(new Vector3(Mathf.Round(floor.transform.position.x), Mathf.Round(floor.transform.position.y), Mathf.Round(floor.transform.position.z)));
        if (highestPoint < floor.transform.position.y)
        {
            highestPoint = floor.transform.position.y;
        }
    }

    private void Cleanup(GameObject floor)
    {
        DestroyPotentialFloors();

        onTowerPlace?.Invoke(floor, EventArgs.Empty);
        cleanupLast = true;
        cleanupLate = true;
    }


}

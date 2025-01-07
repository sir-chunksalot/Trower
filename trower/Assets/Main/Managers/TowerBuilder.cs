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
    GridManager gridManager;
    LevelDetails levelDetails;
    UIManager uiManager;

    private List<GameObject> placedFloors = new List<GameObject>(); //LIST OF PLACED INDIVIDUAL FLOORS
    private List<Vector3> placedFloorsPos = new List<Vector3>(); //LIST OF PLACED INDIVIDUAL FLOOR POSISTIONS
    private List<GameObject> floorPapas = new List<GameObject>(); //LIST OF PLACED GROUPED FLOOR TYPES
    private List<GameObject> debugFloors = new List<GameObject>();
    private List<GameObject> debugFloorPos = new List<GameObject>();
    //private List<GameObject> trapTypes = new List<GameObject>();
    private List<GameObject> ladders = new List<GameObject>();
    List<Vector3> floorSpawnSpots = new List<Vector3>();
    Vector3 nextFloorPos;
    float highestPoint;
    float floorSpawnZ;

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
        gridManager = gameObject.GetComponent<GridManager>();
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
        buildDaddy = gameManager.GetCurrentLevelDetails().GetBuildDaddy();
        levelDetails = gameManager.GetCurrentLevelDetails();
        foreach (GameObject floor in placedFloors)
        {
            Destroy(floor);
        }
        placedFloors.Clear();
        SpawnExistingFloors();
        onTowerStart?.Invoke(gameObject, EventArgs.Empty);
    }

    private void SpawnExistingFloors()
    {
        if (buildDaddy == null) return;
        for (int y = 0; y < levelDetails.gridSize.y; y++)
        {
            for (int x = 0; x < levelDetails.gridSize.x; x++)
            {
                GridSpace cell = gridManager.GetGridSpace(x, y);
                if (levelDetails.existingFloors.rows[y].row[x]) { MakeFloor(cell.GetPos()); Debug.Log("Spawning Starting Floor"); }
            }
        }
    }

    private void MakeFloor(Vector2Int pos)
    {
        MakeFloor(floors[0], pos);
    }
    private void MakeFloor(GameObject build, Vector2Int spawnPos)
    {
        floorPapas.Add(build);
        uiManager.UseCard(build.name, true);
        Vector3 finalSpawnPos = new Vector3(spawnPos.x, spawnPos.y, 10);
        build = Instantiate(build, finalSpawnPos, Quaternion.identity, buildDaddy.transform);
        build.GetComponent<Floor>().PlaceFloor();

        SetAlpha(build, 1);
        foreach (Transform floor in build.transform)
        {
            AddPlacedFloor(floor.gameObject);
            floor.gameObject.name = floor.gameObject.name + "#" + UnityEngine.Random.Range(0, 10000);

            floor.transform.position = new Vector3(Mathf.Round(floor.transform.position.x), Mathf.Round(floor.transform.position.y), floor.transform.position.z);
            GridSpace gridSpace = gridManager.GetGridSpace(floor.transform.position);
            gridSpace.SetCurrentFloor(floor.gameObject);
            foreach (WallCollision wall in floor.GetComponentsInChildren<WallCollision>())
            {
                gridSpace.SetWall(wall);
            }
            foreach (Transform kid in floor)
            {
                Debug.Log("kid.tag" + kid.tag);
                if (kid.tag == "Ladder")
                {
                    gridSpace.SetCurrentLadder(kid.gameObject);
                    Debug.Log("LADDER SET!" + floor.gameObject.name);
                    break;
                }
            }



            onTowerPlace?.Invoke(floor, EventArgs.Empty);
        }
        Vector3 particleSpawnPos = build.transform.position;

        particle.gameObject.transform.position = new Vector3(particleSpawnPos.x, particleSpawnPos.y, particleSpawnPos.z - 2);
        particle.GetComponent<ParticleSystem>().Clear();
        particle.GetComponent<ParticleSystem>().Play();
    }

    public void Place(InputAction.CallbackContext context) //called when mouse is released
    {
        if (context.performed)
        {
            if (placingFloor)
            {
                GameObject newBuild = currentFloor;
                List<GridSpace> cells = new List<GridSpace>();
                List<Vector2> posList = new List<Vector2>();
                foreach (Transform floor in newBuild.transform)
                {
                    GridSpace gridSpace = gridManager.GetClosestGridSpace(floor.transform.position, true);
                    cells.Add(gridSpace);
                    posList.Add(floor.transform.position);
                }

                GridSpace currentGridSpace = gridManager.GetClosestGridSpace(currentFloor.transform.position, true);
                if (gridManager.DoCellsHaveNeighbors(cells) && gridManager.AreCellsValid(posList, true))
                {
                    MakeFloor(newBuild, currentGridSpace.GetPos());
                }
                else
                {
                    uiManager.UseCard(newBuild.name, false);
                    Debug.Log("Mission Failed. We'll get em' next time (failed to build structure)");
                }
                Destroy(currentFloor);
                Cleanup();
                StartCoroutine(PlaceDelay());
            }
            placingFloor = false;
        }

    }


    public void CurrentFloor(string floorName) //CALLED FIRST BY THE BUILD SELECT SCRIPT
    {
        if (currentFloor != null) { Destroy(currentFloor); }
        if (!canPlace) return;
        placingFloor = true;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject activeFloor = floors[0];

        foreach (GameObject floor in floors)
        {
            if (floor.name + "(Clone)" == floorName)
            {
                activeFloor = Instantiate(floor, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            }
        }
        string newName = floorName.Substring(0, floorName.Length - 7);

        currentFloor = activeFloor;
        currentFloor.name = newName;
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
        if (placingFloor)
        {
            placingFloor = false;
            Cleanup();
        }
    }

    public void Undo()
    {
        if (canUndo)
        {
            GameObject floor = placedFloors[placedFloors.Count - 1];
            DestroyFloor(floor);
            StartCoroutine(UndoDelay());
        }

    }

    private IEnumerator UndoDelay()
    {

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
            foreach (Vector3 floorPos in placedFloorsPos)
            {
                if (floorPos.y > highestFloor)
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
        Cleanup();
    }

    private IEnumerator DestroyExplosionEffect(GameObject explosion)
    {
        yield return new WaitForSeconds(2);
        Destroy(explosion);
    }

    public float GetHighestPoint()
    {
        return highestPoint;
    }

    private void FixedUpdate()
    {
        GridSpace closestGridSpace = gameManager.GetClosestGridSpace();
        if (closestGridSpace == null) { return; }
        Vector2 closestGridSpacePos = closestGridSpace.GetPos();
        if (placingFloor)
        {
            currentFloor.transform.position = new Vector3(closestGridSpacePos.x, closestGridSpacePos.y, 9);
        }
    }

    private void LateUpdate()
    {
        if (cleanupLast)
        {
            if (cleanupCount > 16 && cleanupLate) //WE LOVE 16!!!!
            {
                Debug.Log("we gotta late update in here fellas");
                onTowerPlaceLate?.Invoke(gameObject, EventArgs.Empty);
                cleanupLate = false;
            }
            cleanupCount++;

        }
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
        if (placedFloorsPos.Contains(floorPos))
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
            foreach (GameObject floor in placedFloors)
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

    public Vector3 GetInitialFloorPos()
    {
        return placedFloors[0].transform.position;
    }

    public Vector2Int GetRoomBounds()
    {
        return new Vector2Int(10, 5); //MUST BE INTS FOREVER AND ALWAYS
    }

    public bool GetIsPlacingFloor()
    {
        return placingFloor;
    }

    public void AddPlacedFloor(GameObject floor)
    {
        placedFloors.Add(floor);
        placedFloorsPos.Add(new Vector3(Mathf.Round(floor.transform.position.x), Mathf.Round(floor.transform.position.y), Mathf.Round(floor.transform.position.z)));
        if (highestPoint < floor.transform.position.y)
        {
            highestPoint = floor.transform.position.y;
        }
    }

    private void Cleanup()
    {
        cleanupLast = true;
        cleanupLate = true;
    }


}

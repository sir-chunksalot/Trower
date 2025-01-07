using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapBuilder : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] GameObject[] traps;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject moveTrapEffect;
    [SerializeField] GameObject debugTrap;
    [SerializeField] GameObject redX;
    [SerializeField] float reqMouseDistanceToPlace;
    [SerializeField] float opacity;

    GameObject trapDaddy;

    public event EventHandler onTrapPlace;
    private Trap currentTrap;
    TowerBuilder towerBuilder;
    TrapManager trapManager;
    GridManager gridManager;
    UIManager uiManager;
    //reg spawns

    private bool placingTrap;
    private string trapName;
    private bool isWallTrap;
    private bool rotateTrap;
    private Vector2 mousePos;
    private float frontZ;
    private Vector3 offSet;

    Tuple<int, int>[] trapSize;
    GameObject[] redExes = new GameObject[9];

    GridSpace gridSpace;
    private void Awake()
    {
        uiManager = gameObject.GetComponent<UIManager>();
        gridManager = gameObject.GetComponent<GridManager>();
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        trapManager = gameObject.GetComponent<TrapManager>();

        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;

        frontZ = Camera.main.transform.position.z + .1f;

    }

    public void OnSceneLoad(object sender, EventArgs e)
    {
        trapDaddy = gameManager.GetCurrentLevelDetails().GetTrapDaddy();
        for (int i = 0; i < 9; i++)
        {
            redExes[i] = Instantiate(redX, new Vector3(9999, 9999, frontZ), Quaternion.identity, trapDaddy.transform);
        }
    }
    private void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x - offSet.x, mousePos.y - offSet.y, frontZ);

        if (placingTrap)
        {
            gridSpace = gridManager.GetClosestGridSpace(mousePos, false);
            Vector2 spacePos = gridSpace.GetPos();

            currentTrap.gameObject.transform.position = new Vector3(spacePos.x + offSet.x, spacePos.y + offSet.y, frontZ);
            trapSize = currentTrap.GetTrapSize();

            if(redExes[0] == null) { return; }
            int count = 0;
            foreach (Tuple<int, int> pos in trapSize)
            {
                int xIndex = gridSpace.GetIndex().x + pos.Item1;
                int yIndex = gridSpace.GetIndex().y + pos.Item2;

                GridSpace cell = gridManager.GetGridSpace(xIndex, yIndex);
                if (cell == null || cell.GetCurrentFloor() == null)
                {
                    Vector2Int spawnPos = gridManager.GetApproximatePos(xIndex, yIndex);
                    redExes[count].transform.position = new Vector3(spawnPos.x, spawnPos.y, frontZ);
                }
                else
                {
                    redExes[count].transform.position = new Vector3(9999, 9999);
                }

                count++;
            }
        }


        frontZ = Camera.main.transform.position.z + .1f;
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
                onTrapPlace?.Invoke(currentTrap.gameObject, EventArgs.Empty);
            }
            placingTrap = false;
        }
    }

    private void PlaceBuild()
    {
        GridSpace targetGridSpace = gridSpace;
        Vector2Int spawnSpot = gridSpace.GetPos();

        if (targetGridSpace.GetCurrentFloor() == null || targetGridSpace.GetCurrentTrap() != null)
        { //invalid trap spawn if there is no floor or if there is already a trap
            uiManager.UseCard(trapName, false);
            EndPlacement();
            return;
        }
        if (isWallTrap && ((currentTrap.GetIsFacingLeft() && !gridSpace.GetIsWallActive('R')) || (!currentTrap.GetIsFacingLeft() && !gridSpace.GetIsWallActive('L'))))  //invalid trap spawn if its a wall trap and there are no walls on the requested side
        {
            uiManager.UseCard(trapName, false);
            EndPlacement();
            return;
        }
        for (int i = 0; i < 9; i++)
        {
            if (redExes[i].transform.position.x > 9000)//invalid trap spawn if trap size conflicts with the placement (the red x thing)
            {
                uiManager.UseCard(trapName, false);
                EndPlacement();
                return;
            }
        }
       
        Vector3 nextTrapPos = new Vector3(spawnSpot.x + offSet.x, spawnSpot.y + offSet.y, 8 + offSet.z);




        if (Vector2.Distance(nextTrapPos, new Vector2(mousePos.x, mousePos.y - offSet.y)) < reqMouseDistanceToPlace)
        {
            //makes new trap
            uiManager.UseCard(trapName, true);
            GameObject targetObj = GetTrap(currentTrap.gameObject.name);
            Debug.Log("targetObj" + targetObj + "current trap" + currentTrap + "dogdog1234");
            GameObject newBuild = Instantiate(targetObj, nextTrapPos, Quaternion.identity, trapDaddy.transform);

            //setting trap variables
            Trap trap = newBuild.GetComponent<Trap>();
            trapManager.AddTrapToList(trap);
            trap.EnableTrap();
            targetGridSpace.SetCurrentTrap(trap);
            if (rotateTrap) { trap.Rotate(true); }

            //finishing with flare
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
    public void CurrentTrap(string newTrapName, bool rotation)
    {
        Debug.Log(newTrapName + "UNO");
        placingTrap = true;
        rotateTrap = false;

        GameObject activeTrap = GetTrap(newTrapName);
        Trap trapScript = activeTrap.GetComponent<Trap>();
        trapName = activeTrap.name;
        if (trapScript != null)
        { //it is a trap
            offSet = trapScript.GetOffset();
            GameObject newTrap = Instantiate(activeTrap, new Vector3(mousePos.x, mousePos.y, 20), Quaternion.identity);
            newTrap.GetComponent<Trap>().DisableTrap();
            towerBuilder.SetAlpha(newTrap, opacity);
            currentTrap = newTrap.GetComponent<Trap>();
            isWallTrap = currentTrap.GetIsWallTrap();
            trapSize = currentTrap.GetTrapSize();

            if (rotation)
            {
                Debug.Log("LOLOL");
                currentTrap.Rotate(true);
                rotateTrap = true;

                if (currentTrap.GetIsFacingLeft()) //reverses offset if the trap is flipped
                {
                    offSet.x = offSet.x * -1;
                }
            }
        }
    }

    private GameObject GetTrap(string name)
    {
        foreach (GameObject trap in traps)
        {
            if (trap.name == name || trap.name + "(Clone)" == name)
            {
                return trap;
            }
        }
        return null;
    }

    public void EndPlacement()
    {
        if (currentTrap == null) { return; }
        for(int i = 0; i < 9; i++)
        {
            redExes[i].transform.position = new Vector3(9999, 9999, 9999);
        }
        Debug.Log("bew end");
        placingTrap = false;
        trapName = "";
        Destroy(currentTrap.gameObject);
    }
}

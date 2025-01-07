using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAdditions : MonoBehaviour
{
    [SerializeField] GameObject woodenTriangle;
    [SerializeField] GameObject roofExtension;
    [SerializeField] GameObject door;
    [SerializeField] GameObject bridge;
    [SerializeField] GameObject smokeEffect;
    TowerBuilder towerBuilder;
    GameManager gameManager;
    GridManager gridManager;
    List<GameObject> addedFrills;
    GridSpace[,] gameGrid;
    float zValue;
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        towerBuilder.onTowerPlace += CreateAdditions;
        gridManager = gameObject.GetComponent<GridManager>();

        addedFrills = new List<GameObject>();
        zValue = 15;
    }

    private void OnSceneLoad(object sender, EventArgs e)
    {
        addedFrills.Clear();
        gameGrid = gridManager.GetGrid();
    }

    public void CreateAdditions(object floor, EventArgs e)
    {
        if (gameGrid == null) { return; }

        foreach (GridSpace cell in gameGrid)
        {
            //gets existing frills on this cell (if they exist)
            GameObject OG_bridge = cell.GetFrill("bridge");
            GameObject OG_leftTriangle = cell.GetFrill("leftTriangle");
            GameObject OG_rightTriangle = cell.GetFrill("rightTriangle");
            GameObject OG_roofExtension = cell.GetFrill("roofExtension");
            GameObject OG_roomDoor = cell.GetFrill("roomDoor");

            //do checks
            bool makeBridge = Bridge(cell);
            bool makeLeftTriangle = LeftTriangle(cell);
            bool makeRightTriangle = RightTriangle(cell);
            bool makeRoofExtension = RoofExtensions(cell);
            bool makeDoor = RoomDoor(cell);

            //if a frill is on a cell and it shouldnt be, destroy it
            if (!makeBridge) { DestroyFrill(OG_bridge, cell); }
            if (!makeLeftTriangle) { DestroyFrill(OG_leftTriangle, cell); }
            if (!makeRightTriangle) { DestroyFrill(OG_rightTriangle, cell); }
            if (!makeRoofExtension) { DestroyFrill(OG_roofExtension, cell); }
            if (!makeDoor) { DestroyFrill(OG_roomDoor, cell); }

            //if there isnt a frill on a cell and its allowed to be there, make one
            if (OG_bridge == null && makeBridge) { MakeFrill(bridge, "bridge", cell, new Vector2(0, 0), false, true); }
            if (OG_leftTriangle == null && makeLeftTriangle) { MakeFrill(woodenTriangle, "leftTriangle", cell, new Vector2(1f, 0), false); }
            if (OG_rightTriangle == null && makeRightTriangle) { MakeFrill(woodenTriangle, "rightTriangle", cell, new Vector2(-1f, 0), true); }
            if (OG_roofExtension == null && makeRoofExtension) { MakeFrill(roofExtension, "roofExtension", cell, new Vector3(5, 3.07f, -15f), false); }
            if (OG_roomDoor == null && makeDoor) { MakeFrill(door, "roomDoor", cell, new Vector3(5f, 0, -15f), false); }

        }
    }

    private bool RightTriangle(GridSpace cell)
    {
        if (cell.GetCurrentFloor() != null) { return false; }
        if (cell.GetFrill("bridge") != null) { return false; }

        GridSpace up = gridManager.GetCellNeighbor(cell, 0);
        GridSpace left = gridManager.GetCellNeighbor(cell, 1);
        if (up != null && left != null)
        {
            if (up.GetCurrentFloor() != null && left.GetCurrentFloor() != null)
            {
                return true;
            }
        }
        return false;
    }
    private bool LeftTriangle(GridSpace cell)
    {
        if (cell.GetCurrentFloor() != null) { return false; }
        if (cell.GetFrill("bridge") != null) { return false; }

        GridSpace up = gridManager.GetCellNeighbor(cell, 0);
        GridSpace right = gridManager.GetCellNeighbor(cell, 2);
        if (up != null && right != null)
        {
            if (up.GetCurrentFloor() != null && right.GetCurrentFloor() != null)
            {
                return true;
            }
        }
        return false;
    }
    private bool RoomDoor(GridSpace cell)
    {
        GridSpace right = gridManager.GetCellNeighbor(cell, 2);
        if (cell.GetCurrentFloor() == null || right == null) { return false; }
        if (right != null)
        {
            if (right.GetCurrentFloor() == null) { return false; }
            if (right.GetCurrentFloor().transform.parent == cell.GetCurrentFloor().transform.parent) { return false; }
        }
        return true;
    }
    private bool RoofExtensions(GridSpace cell)
    {
        GridSpace right = gridManager.GetCellNeighbor(cell, 2);
        GridSpace up = gridManager.GetCellNeighbor(cell, 0);
        GridSpace topRight = gridManager.GetCornerCells(cell, 1);

        if (cell.GetCurrentFloor() == null || right == null) { return false; }
        if (right != null)
        {
            if (right.GetCurrentFloor() == null) { return false; }
        }
        if (topRight != null)
        {
            if (topRight.GetCurrentFloor() != null) { return false; }
        }
        if (up != null)
        {
            if (up.GetCurrentFloor() != null) { return false; }
        }

        return true;
    }

    private bool Bridge(GridSpace cell)
    {
        Vector2Int index = cell.GetIndex();
        GridSpace left = gridManager.GetCellNeighbor(cell, 1);
        GridSpace right = gridManager.GetCellNeighbor(cell, 2);
        if (cell.GetCurrentFloor() != null) { return false; }//cant place a bridge on a preexisting floor
        if (right == null) { return false; } //space to the right must exist
        if (left == null) { return false; } //space to the left must exist

        bool doesBridgeHaveStart = false;
        bool doesBridgeHaveEnd = false;
        for (int i = index.x + 1; i < gridManager.GetGridSize().x; i++)
        {
            if (gridManager.GetGridSpace(i, index.y).GetCurrentFloor() != null)
            {
                doesBridgeHaveEnd = true;
            }
        }
        for (int i = index.x - 1; i >= 0; i--)
        {
            if (gridManager.GetGridSpace(i, index.y).GetCurrentFloor() != null)
            {
                doesBridgeHaveStart = true;
            }
        }

        if (doesBridgeHaveStart && doesBridgeHaveEnd) { return true; }
        return false;
    }


    private void MakeFrill(GameObject type, string name, GridSpace targetCell, Vector3 offset, bool flip, bool smokeEffect = false)
    {
        Vector3 spawnPos = new Vector3(targetCell.GetPos().x + offset.x, targetCell.GetPos().y + offset.y, zValue + offset.z);
        GameObject newFrill = Instantiate(type, spawnPos, Quaternion.identity);
        newFrill.name = name;
        if (flip) { newFrill.GetComponent<SpriteRenderer>().flipX = true; }
        targetCell.AddNewFrill(newFrill);
        addedFrills.Add(newFrill);

        if (smokeEffect)
        {
            Vector3 smokeSpawnPos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z - 20);
            GameObject smoke = Instantiate(this.smokeEffect, smokeSpawnPos, Quaternion.identity);
            StartCoroutine(CleanupSmoke(smoke));
        }
    }
    private void DestroyFrill(GameObject frill, GridSpace cell)
    {
        addedFrills.Remove(frill);
        cell.RemoveFrill(frill);
        Destroy(frill);
    }

    private IEnumerator CleanupSmoke(GameObject smoke)
    {
        yield return new WaitForSeconds(5f);
        Destroy(smoke);
    }
}

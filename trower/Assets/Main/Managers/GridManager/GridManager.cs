using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    Vector2[,] worldGrid;
    GridSpace[,] gameGrid;
    GameManager gameManager;
    TowerBuilder towerBuilder;
    Vector2Int gridSize;
    Vector2 roomBounds;
    int worldSizeX = 20;
    int worldSizeY = 20;

    public bool isGridUpdated;
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoadedAwake += OnSceneLoadAwake;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        roomBounds = towerBuilder.GetRoomBounds();
    }

    private void OnSceneLoadAwake(object sender, EventArgs e)
    {
        Setup();
    }

    private void Setup()
    {
        //gamegrid
        gridSize = gameManager.GetCurrentLevelDetails().gridSize;
        gameGrid = new GridSpace[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                gameGrid[x, y] = new GridSpace(new Vector2Int(x * (int)roomBounds.x, y * (int)roomBounds.y), new Vector2Int(x, y));
            }
        }


        //debugging
        Vector2 topRightCorner = gameGrid[gridSize.x - 1, gridSize.y - 1].GetPos();
        topRightCorner = new Vector2(topRightCorner.x + (roomBounds.x / 2), topRightCorner.y + (roomBounds.y / 2));

        for (float i = -(roomBounds.x / 2); i <= topRightCorner.x; i += roomBounds.x)
        {
            Debug.DrawLine(new Vector2(i, -(roomBounds.y / 2)), new Vector2(i, topRightCorner.y), Color.green, 100f);
            Debug.Log("Drew line at: " + new Vector2(i, topRightCorner.y));
        }
        for (float i = -(roomBounds.y / 2); i <= topRightCorner.y; i += roomBounds.y)
        {
            Debug.DrawLine(new Vector2(-(roomBounds.x / 2), i), new Vector2(topRightCorner.x, i), Color.green, 100f);
            Debug.Log("Drew line at: " + new Vector2(topRightCorner.x, i));
        }

        isGridUpdated = true;
    }

    public GridSpace[,] GetGrid()
    {
        return gameGrid;
    }

    public Vector2Int GetGridSize()
    {
        return gridSize;
    }

    public Vector2 GetCellSize()
    {
        return new Vector2(roomBounds.x, roomBounds.y);
    }

    public Vector2Int GetApproximatePos(int indedxX, int indexY)
    {
        return (new Vector2Int(indedxX * (int)roomBounds.x, indexY * (int)roomBounds.y));
    }

    public GridSpace GetGridSpace(Vector2 pos)
    {
        int x = (int)(pos.x / roomBounds.x);
        int y = (int)(pos.y / roomBounds.y);
        if (gameGrid == null) { return null; }
        if (x >= gridSize.x || y >= gridSize.y || x < 0 || y < 0) { return null; }
        return gameGrid[x, y];
    }
    public GridSpace GetGridSpace(int x, int y)
    {
        if (x >= gridSize.x || y >= gridSize.y || x < 0 || y < 0) { return null; }
        return gameGrid[x, y];
    }


    public GridSpace GetClosestGridSpace(Vector2 pos, bool checkForFloor)
    {
        if (gameGrid == null) { return null; }
        Vector2 closestPos = new Vector2(-999, -999);
        GridSpace closestSpace = null;
        foreach (GridSpace space in gameGrid)
        {
            if (checkForFloor && space.GetCurrentFloor() != null) { continue; }
            if ((space.GetPos() - pos).magnitude < (closestPos - pos).magnitude)
            {
                closestPos = space.GetPos();
                closestSpace = space;
            }
        }

        return closestSpace;

    }

    public GridSpace[] GetAdjacentCells(GridSpace cell)
    {
        if (gameGrid == null || cell == null) { return null; }

        int xIndex = cell.GetIndex().x;
        int yIndex = cell.GetIndex().y;

        GridSpace up = GetGridSpace(xIndex, yIndex + 1);
        GridSpace left = GetGridSpace(xIndex - 1, yIndex);
        GridSpace right = GetGridSpace(xIndex + 1, yIndex);
        GridSpace down = GetGridSpace(xIndex, yIndex - 1);

        return new GridSpace[4] { up, left, right, down };
    }

    public GridSpace[] GetCornerCells(GridSpace cell)
    {
        if (gameGrid == null || cell == null) { return null; }

        int xIndex = cell.GetIndex().x;
        int yIndex = cell.GetIndex().y;

        GridSpace topLeft = GetGridSpace(xIndex - 1, yIndex + 1);
        GridSpace topRight = GetGridSpace(xIndex + 1, yIndex + 1);
        GridSpace bottomLeft = GetGridSpace(xIndex - 1, yIndex - 1);
        GridSpace bottomRight = GetGridSpace(xIndex + 1, yIndex - 1);

        return new GridSpace[4] { topLeft, topRight, bottomLeft, bottomRight };
    }

    public bool DoesCellHaveNeighbor(GridSpace cell)
    {
        GridSpace[] adjacentCells = GetAdjacentCells(cell);
        if (gameGrid == null || adjacentCells == null || cell == null) { return false; }

        for (int i = 0; i < adjacentCells.Length; i++)
        {
            if (adjacentCells[i] != null && adjacentCells[i].GetCurrentFloor() != null) { return true; }
        }
        Debug.Log(cell + " CELL DOES NOT HAVE A NEIGHBOR");
        return false;
    }

    public bool DoCellsHaveNeighbors(List<GridSpace> cellList)
    {
        foreach (GridSpace cell in cellList)
        {
            if (DoesCellHaveNeighbor(cell)) { return true; }
        }
        return false;
    }

    public GridSpace GetCellNeighbor(GridSpace cell, int index)//0 is up, 1 is left, 2 is right, and 3 is down
    {
        GridSpace[] neighbors = GetAdjacentCells(cell);
        return neighbors[index];
    }

    public GridSpace GetCornerCells(GridSpace cell, int index)
    {
        GridSpace[] corners = GetCornerCells(cell);
        return corners[index];
    }

    public bool AreCellsValid(List<Vector2> posList, bool checkForFloors)
    {
        foreach (Vector2 gridPos in posList)
        {
            if (GetGridSpace(gridPos) == null) { return false; }
            if (checkForFloors && GetGridSpace(gridPos).GetCurrentFloor() != null) { return false; }
        }
        return true;
    }


    public Vector2 GetCenterOfGrid()
    {
        float x = gridSize.x / 2f;
        float y = gridSize.y / 2f;

        Vector2 center = new Vector2((x * roomBounds.x) - (roomBounds.x / 2), (y * roomBounds.y));
        return center;
    }

    public bool GetIsGridUpdated()
    {
        return isGridUpdated;
    }

    public void SetIsGridUpdated(bool isGridUpdated)
    {
        this.isGridUpdated = isGridUpdated;
    }

    public void DebugGridContents()
    {
        string output = "";
        for (int y = gridSize.y - 1; 0 <= y; y--)
        {
            string line = "";
            for (int x = 0; x < gridSize.x; x++)
            {
                string cell = ("[  " + gameGrid[x, y].GetContents() + "  ]");
                int padding = 50 - cell.Length;
                string paddedContent = cell + new string('-', padding);
                line += paddedContent;

            }
            output += line + "\n";
        }
        Debug.Log(output + "\n GAMEGRID CONTENTS");
    }

}

using System.Collections.Generic;
using UnityEngine;

public class GridSpace
{
    Vector2Int pos;
    Vector2Int index;
    Trap currentTrap = null;
    GameObject currentFloor = null;
    GameObject currentLadder = null;
    GameObject bridge = null;
    bool hasDoor;
    WallCollision[] walls = new WallCollision[4];
    List<GameObject> frills = new List<GameObject>();
    int wallCount = 0;
    public GridSpace(Vector2Int pos, Vector2Int index)
    {
        this.pos = pos;
        this.index = index;
    }

    public Vector2Int GetPos()
    {
        return pos;
    }

    public void SetWall(WallCollision wall)
    {
        if (wallCount >= 4)
        {
            Debug.Log("ERROR. Tried to set more than 4 walls in: " + GetCurrentFloor() + "");
            return;
        }

        walls[wallCount] = wall;
        wallCount++;
    }

    public bool GetIsWallActive(char dir)
    {
        int index = 0;
        if (dir == 'l' || dir == 'L')
        {
            index = 0;
        }
        if (dir == 'r' || dir == 'R')
        {
            index = 1;
        }
        if (dir == 'd' || dir == 'D')
        {
            index = 2;
        }
        if (dir == 'u' || dir == 'U')
        {
            index = 3;
        }

        if (walls[index] != null)
        {
            return walls[index].GetIsWallVisible();
        }
        Debug.Log("No valid wall found with the given char.");
        return false;
    }

    public bool GetHasActiveWall()
    {
        if (walls[0].GetIsWallVisible() || walls[1].GetIsWallVisible()) { return true; }
        return false;
    }

    public Vector2Int GetIndex()
    {
        return index;
    }

    public Trap GetCurrentTrap()
    {
        return currentTrap;
    }
    public void SetCurrentTrap(Trap newTrap)
    {
        currentTrap = newTrap;
    }

    public GameObject GetCurrentFloor()
    {
        return currentFloor;
    }
    public void SetCurrentFloor(GameObject newFloor)
    {
        currentFloor = newFloor;
    }

    public GameObject GetCurrentLadder()
    {
        return currentLadder;
    }
    public void SetCurrentLadder(GameObject ladder)
    {
        currentLadder = ladder;
    }

    public bool GetHasDoor()
    {
        return hasDoor;
    }

    public void SetHasDoor(bool hasDoor)
    {
        this.hasDoor = hasDoor;
    }

    public GameObject GetFrill(string frillName)
    {
        foreach (GameObject frill in frills)
        {
            Debug.Log(frill.name + " FRILL NAME MEMORY" + GetIndex());
            if (frill.name == frillName) { return frill; }
        }
        return null;
    }

    public void AddNewFrill(GameObject frill)
    {
        frills.Add(frill);
    }

    public void RemoveFrill(GameObject frill)
    {
        frills.Remove(frill);
    }

    public string GetContents()
    {
        string floorName = "None";
        string trapName = "None";
        string ladderName = "None";
        if (currentFloor != null) { floorName = currentFloor.name; }
        if (currentTrap != null) { trapName = currentTrap.name; }
        if (currentLadder != null) { ladderName = currentLadder.name; }

        //return "Floor: " + floorName + ", Trap: " + trapName + ", Ladder: " + ladderName;
        return "Ladder: " + ladderName;
    }
}

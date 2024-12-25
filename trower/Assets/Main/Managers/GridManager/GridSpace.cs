using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace 
{
    Vector2 pos;
    Vector2Int index;
    GameObject currentTrap = null;
    GameObject currentFloor = null;
    public GridSpace(Vector2 pos, Vector2Int index)
    {
        this.pos = pos;
        this.index = index;
    }

    public Vector2 GetPos()
    {
        return pos;
    }

    public Vector2Int GetIndex()
    {
        return index;
    }
    
    public GameObject GetCurrentTrap()
    {
        return currentTrap;
    }
    public void SetCurrentTrap(GameObject newTrap)
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

    public string GetContents()
    {
        string floorName = "None";
        string trapName = "None";
        if (currentFloor != null) { floorName = currentFloor.name; }
        if (currentTrap != null) { trapName = currentTrap.name; }

        return "Floor: " + floorName + ", Trap: " + trapName;
    }
}

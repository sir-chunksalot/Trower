using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace 
{
    Vector2 pos;
    public GridSpace(Vector2 pos)
    {
        this.pos = pos;
    }

    public Vector2 GetPos()
    {
        return pos;
    }
}

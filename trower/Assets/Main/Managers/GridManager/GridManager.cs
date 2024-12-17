using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    GridSpace[,] gameGrid;
    GameManager gameManager;
    TowerBuilder towerBuilder;
    Vector2Int gridSize;
    Vector2 roomBounds;

    public bool isGridUpdated;
    void Start()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        roomBounds = towerBuilder.GetRoomBounds();
    }

    private void OnSceneLoad(object sender, EventArgs e)
    {
        Setup();
    }

    private void Setup()
    {
        gridSize = gameManager.GetCurrentLevelDetails().gridSize;
        gameGrid = new GridSpace[gridSize.x, gridSize.y];

        for(int x = 0; x < gridSize.x; x++) 
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                gameGrid[x,y] = new GridSpace(new Vector2(x * roomBounds.x, y * roomBounds.y));
            }
        }


        //debugging
        Vector2 topRightCorner = gameGrid[gridSize.x - 1, gridSize.y - 1].GetPos();
        topRightCorner = new Vector2(topRightCorner.x + (roomBounds.x / 2), topRightCorner.y + (roomBounds.y / 2));

        for (float i = -(roomBounds.x/2); i <= topRightCorner.x; i+=roomBounds.x)
        {
            Debug.DrawLine(new Vector2(i, -(roomBounds.y / 2)), new Vector2(i, topRightCorner.y), Color.green, 100f);
            Debug.Log("Drew line at: " + new Vector2(i, topRightCorner.y));
        }
        for(float i = -(roomBounds.y/2); i <= topRightCorner.y; i+=roomBounds.y)
        {
            Debug.DrawLine(new Vector2(-(roomBounds.x / 2), i), new Vector2(topRightCorner.x, i), Color.green, 100f);
            Debug.Log("Drew line at: " + new Vector2(topRightCorner.x, i));
        }

        isGridUpdated = true;
    }
    
    public Vector2 GetCenterOfGrid()
    {
        float x = gridSize.x / 2f;
        float y = gridSize.y / 2f;

        Vector2 center = new Vector2((x * roomBounds.x), (y * roomBounds.y));
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

}

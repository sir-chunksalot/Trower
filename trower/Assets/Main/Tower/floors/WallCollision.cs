using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    [SerializeField] GameObject enemyWall;
    [SerializeField] GameObject raycastWall;
    [SerializeField] Sprite bottomFloorSprite;
    [SerializeField] float strength;
    [SerializeField] List<SpriteRenderer> sprites;
    [SerializeField] bool isBridge;
    public int colCount;
    BoxCollider2D enemyCollider;
    BoxCollider2D col;
    EnemyWallCollision enemyWallCollision;
    Floor floor;
    bool hasBeenEnabled;
    private void Awake()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
        if (enemyWall != null)
        {
            enemyCollider = enemyWall.GetComponent<BoxCollider2D>();
            enemyWallCollision = enemyWall.GetComponent<EnemyWallCollision>();
        }
        hasBeenEnabled = false;

        if (sprites.Count == 0)
        {
            sprites.Add(gameObject.GetComponent<SpriteRenderer>());
        }

        floor = gameObject.GetComponentInParent<Floor>();

        if (floor.isPlaced)
        {
            StartUp();
        }
        else
        {
            floor.onFloorPlace += DestroyWalls;
        }



    }

    public BoxCollider2D GetEnemyWall()
    {
        if (enemyWall != null)
        {
            return enemyWall.GetComponent<BoxCollider2D>();
        }
        return null;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit that collider buddy!!!");
        if ((collision.transform.tag == "Wall" && gameObject.tag == "Wall") || (collision.transform.tag == "Floor" && gameObject.tag == "Floor") || (collision.transform.tag == "Extension" && gameObject.tag == "Extension") || (collision.transform.tag == "FloorMurderer" && gameObject.tag == "Floor"))
        {
            if (gameObject != null)
            {
                WallCollision otherCol = collision.gameObject.GetComponent<WallCollision>();

                if (strength <= otherCol.strength)
                {
                    if(!otherCol.isBridge && raycastWall != null)
                    {
                        raycastWall.SetActive(false);
                    }
                    if (enemyWall != null && gameObject.name != "Floor1")
                    {
                        enemyCollider.enabled = false;
                        enemyWall.SetActive(false);
                    }
                    foreach (SpriteRenderer sprite in sprites)
                    {
                        sprite.enabled = false;
                    }
                    colCount++;
                }

            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Wall" && gameObject.tag == "Wall") || (collision.transform.tag == "Floor" && gameObject.tag == "Floor") || (collision.transform.tag == "Extension" && gameObject.tag == "Extension") || (collision.transform.tag == "FloorMurderer" && gameObject.tag == "Floor"))
        {
            Debug.Log("Collision exit: " + collision.transform.tag + "real");
            if (gameObject != null) 
            {
                WallCollision otherCol = collision.gameObject.GetComponent<WallCollision>();
                if (strength <= otherCol.strength)
                {
                    colCount--;
                    if (colCount <= 0)
                    {
                        RegenWalls();
                    }
                }

            }
        }
    }
    private void OnDestroy()
    {
        floor.onFloorPlace -= DestroyWalls;
    }

    private void RegenWalls()
    {
        Debug.Log("Renabling!");
        if (sprites != null)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.enabled = true;
            }
        }
        if (raycastWall != null)
        {
            raycastWall.SetActive(true);
        }
        if (enemyWall != null)
        {
            enemyCollider.enabled = true;
            enemyWall.SetActive(true);
            enemyWallCollision.TurnOn();
        }


        col.enabled = false;
        col.enabled = true;

        colCount = 0;
    }

    private void DestroyWalls(object sender, EventArgs e) //this triggers the first time the wall is placed in the world, this is so that the wall collision script doesnt get fucked as your dragging the wall around
    {
        StartUp();
    }

    private void StartUp()
    {
        if (gameObject != null && !hasBeenEnabled)
        {
            hasBeenEnabled = true;
            col.enabled = true;
            //this.GetComponent<SpriteRenderer>().enabled = true;
            if (raycastWall != null)
            {
                raycastWall.SetActive(true);
            }
            if (enemyWall != null)
            {
                enemyWallCollision.TurnOn();
            }

            if (transform.position.y <= 0) //runs whenever you try and place walls on the bottom layer
            {
                if (gameObject.tag != "Floor")
                {
                    if (GetEnemyWall() != null)
                    {
                        GetEnemyWall().gameObject.SetActive(false);
                        GetEnemyWall().enabled = false;
                    }
                    if (raycastWall != null)
                    {
                        raycastWall.SetActive(false);
                    }
                }
                if (bottomFloorSprite != null)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = bottomFloorSprite;
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    [SerializeField] GameObject enemyWall;
    [SerializeField] Sprite bottomFloorSprite;
    [SerializeField] float strength;
    [SerializeField] List<SpriteRenderer> sprites;
    GameObject gameManager;
    BoxCollider2D towerCollider;
    BoxCollider2D enemyCollider;
    EnemyWallCollision enemyWallCollision;
    TowerBuilder towerBuilder;
    bool hasBeenEnabled;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        towerBuilder = gameManager.GetComponent<TowerBuilder>();
        towerBuilder.onTowerPlace += DestroyWalls;
        towerBuilder.onTowerSell += RegenWalls;
        towerBuilder.onTowerPlace += RegenWalls;
        towerCollider = gameObject.GetComponent<BoxCollider2D>();
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
    }

    public BoxCollider2D GetEnemyWall()
    {
        if (enemyWall != null)
        {
            return enemyWall.GetComponent<BoxCollider2D>();
        }
        return null;

    }
    private void Update()
    {
        if (transform.position.y <= 0 && towerCollider.enabled) //runs whenever you try and place walls on the bottom layer
        {
            if (gameObject.tag != "Floor")
            {
                if (GetEnemyWall() != null)
                {
                    GetEnemyWall().gameObject.SetActive(false);
                    GetEnemyWall().enabled = false;
                }
            }
            if (bottomFloorSprite != null)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = bottomFloorSprite;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Wall" && gameObject.tag == "Wall") || (collision.transform.tag == "Floor" && gameObject.tag == "Floor") || (collision.transform.tag == "Extension" && gameObject.tag == "Extension"))
        {
            if (gameObject != null)
            {
                if (strength <= collision.gameObject.GetComponent<WallCollision>().strength)
                {
                    if (enemyWall != null && gameObject.name != "Floor1")
                    {
                        enemyWall.GetComponent<BoxCollider2D>().enabled = false;
                        enemyWall.SetActive(false);
                    }
                    foreach (SpriteRenderer sprite in sprites)
                    {
                        sprite.enabled = false;
                    }
                }

            }
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null) //i dont remember what this is for but dont remove it
        {
            towerBuilder.onTowerPlace -= DestroyWalls;
            towerBuilder.onTowerSell -= RegenWalls;
        }
    }

    private void RegenWalls(object sender, EventArgs e) //this triggers whenever a build is sold, so all the calculations can be ran again 
    {
        if(enemyWall != null)
        {
            enemyCollider.enabled = true;
            enemyWall.SetActive(true);

            gameObject.GetComponent<SpriteRenderer>().enabled = true;

            hasBeenEnabled = true;
            enemyWallCollision.TurnOn();

            gameObject.SetActive(false); // i have to do this so that the colission detection updates
            gameObject.SetActive(true);
        }
    }

    private void DestroyWalls(object sender, EventArgs e) //this triggers the first time the wall is placed in the world, this is so that the wall collision script doesnt get fucked as your dragging the wall around
    {
        if (gameObject != null && !hasBeenEnabled)
        {
            hasBeenEnabled = true;
            this.GetComponent<BoxCollider2D>().enabled = true;
            //this.GetComponent<SpriteRenderer>().enabled = true;

            if (enemyWall != null)
            {
                enemyWallCollision.TurnOn();
            }
        }
    }
}

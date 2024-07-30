using System;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    [SerializeField] GameObject enemyWall;
    [SerializeField] Sprite bottomFloorSprite;
    [SerializeField] float armor;
    [SerializeField] List<SpriteRenderer> sprites;
    GameObject gameManager;
    bool hasBeenEnabled;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager.GetComponent<TowerBuilder>().onTowerPlace += DestroyWalls;
        gameManager.GetComponent<TowerBuilder>().onTowerSell += RegenWalls;
        Debug.Log(transform.position.y);
        hasBeenEnabled = false;

        if(sprites.Count == 0)
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
        if (transform.position.y <= 0 && this.GetComponent<BoxCollider2D>().enabled) //runs whenever you try and place walls on the bottom layer
        {
            if (gameObject.tag != "Floor")
            {
                if (GetEnemyWall() != null)
                {
                    GetEnemyWall().gameObject.SetActive(false);
                    GetEnemyWall().enabled = false;
                }
            }
            if(bottomFloorSprite != null)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = bottomFloorSprite;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("test");
        if ((collision.transform.tag == "Wall" && gameObject.tag == "Wall") || (collision.transform.tag == "Floor" && gameObject.tag == "Floor") || (collision.transform.tag == "Extension" && gameObject.tag == "Extension"))
        {
            Debug.Log("fart collision" + collision.transform.gameObject);
            if (gameObject != null)
            {
                Debug.Log("deletion");
                if (enemyWall != null && gameObject.name != "Floor1")
                {
                    enemyWall.GetComponent<BoxCollider2D>().enabled = false;
                    enemyWall.SetActive(false);
                }
                if(armor <= 0)
                {
                    foreach(SpriteRenderer sprite in sprites)
                    {
                        sprite.enabled = false;
                    }
                }
                else
                {
                    armor--;
                }



                if (collision.gameObject.name == "Floor1") return;
                BoxCollider2D otherEnemyWall = collision.gameObject.GetComponent<WallCollision>().GetEnemyWall();
                if (otherEnemyWall != null)
                {
                    otherEnemyWall.enabled = false;
                    // collision.gameObject.GetComponent<WallCollision>().GetEnemyWall().gameObject.SetActive(false);
                }
            }
        }
    }

    public float GetArmor()
    {
        return armor;
    }

    private void OnDestroy()
    {
        if (gameManager != null) //i dont remember what this is for but dont remove it
        {
            gameManager.GetComponent<TowerBuilder>().onTowerPlace -= DestroyWalls;
            gameManager.GetComponent<TowerBuilder>().onTowerSell -= RegenWalls;
        }
    }

    private void RegenWalls(object sender, EventArgs e) //this triggers whenever a build is sold, so all the calculations can be ran again 
    {
        enemyWall.GetComponent<BoxCollider2D>().enabled = true;
        enemyWall.SetActive(true);

        gameObject.GetComponent<SpriteRenderer>().enabled = true;

        hasBeenEnabled = true;
        enemyWall.GetComponent<EnemyWallCollision>().TurnOn();

        gameObject.SetActive(false); // i have to do this so that the colission detection updates
        gameObject.SetActive(true);
    }

    private void DestroyWalls(object sender, EventArgs e) //this triggers the first time the wall is placed in the world, this is so that the wall collision script doesnt get fucked as your dragging the wall around
    {
        if (gameObject != null && !hasBeenEnabled)
        {
            hasBeenEnabled = true;
            Debug.Log("box collider enabled");
            this.GetComponent<BoxCollider2D>().enabled = true;
            //this.GetComponent<SpriteRenderer>().enabled = true;
            Debug.Log(this.GetComponent<BoxCollider2D>().enabled);

            if(enemyWall != null)
            {
                enemyWall.GetComponent<EnemyWallCollision>().TurnOn();
            }
        }
    }
}

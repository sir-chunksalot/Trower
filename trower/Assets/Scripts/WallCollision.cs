using System;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    [SerializeField] GameObject enemyWall;
    [SerializeField] Sprite bottomFloorSprite;
    GameObject gameManager;
    bool hasBeenEnabled;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager.GetComponent<TowerBuilder>().onTowerPlace += DestroyWalls;
        gameManager.GetComponent<TowerBuilder>().onTowerSell += RegenWalls;
        Debug.Log(transform.position.y);
        hasBeenEnabled = false;
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
            gameObject.GetComponent<SpriteRenderer>().sprite = bottomFloorSprite;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("test");
        if ((collision.transform.tag == "Wall" && gameObject.tag == "Wall") || (collision.transform.tag == "Floor" && gameObject.tag == "Floor"))
        {
            Debug.Log("fart collision" + collision.transform.gameObject);
            if (gameObject != null)
            {
                Debug.Log("deletion");
                if (enemyWall != null)
                {
                    enemyWall.GetComponent<BoxCollider2D>().enabled = false;
                    enemyWall.SetActive(false);
                }
                gameObject.GetComponent<SpriteRenderer>().enabled = false;

                BoxCollider2D otherEnemyWall = collision.gameObject.GetComponent<WallCollision>().GetEnemyWall();
                if (otherEnemyWall != null)
                {
                    otherEnemyWall.enabled = false;
                    // collision.gameObject.GetComponent<WallCollision>().GetEnemyWall().gameObject.SetActive(false);
                }
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
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
            enemyWall.GetComponent<EnemyWallCollision>().TurnOn();
        }
    }
}

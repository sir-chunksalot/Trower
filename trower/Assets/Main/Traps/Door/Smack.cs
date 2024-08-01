using System.Collections.Generic;
using UnityEngine;

public class Smack : MonoBehaviour
{
    [SerializeField] Sprite openDoor;
    [SerializeField] Sprite closedDoor;
    [SerializeField] float doorOpenTime;
    [SerializeField] SpriteRenderer doorSpriteRenderer;
    bool usingDoor;
    private List<GameObject> enemies = new List<GameObject>();

    private void Start()
    {
        doorSpriteRenderer.sprite = openDoor;
        usingDoor = false;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        usingDoor = false;
        doorSpriteRenderer.sprite = closedDoor;
        Debug.Log("holding");
    }

    private void OnDisable()
    {
        doorSpriteRenderer.sprite = openDoor;

        Debug.Log(enemies.Count);
        foreach (GameObject e in enemies)
        {
            Debug.Log("CUM");
            if (e != null)
            {
                //e.gameObject.GetComponent<Hero>().Fall();
            }
        }

        Debug.Log("letgo");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("hitting something" + collision.tag + usingDoor);
        if (collision.tag == "Enemy")
        {
            Debug.Log("hit enemy");
            enemies.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            enemies.Remove(collision.gameObject);
        }
    }


}

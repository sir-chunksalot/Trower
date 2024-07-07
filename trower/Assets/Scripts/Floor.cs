using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private float cost;
    TowerBuilder towerBuilder;
    List<GameObject> heroes;
    private void Awake()
    {
        towerBuilder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TowerBuilder>();
        towerBuilder.AddPlacedFloor(gameObject);
        heroes = new List<GameObject>();
    }
    public float GetCost()
    {
        return cost;
    }

    public TowerBuilder GetTowerBuilder()
    {
        return towerBuilder;
    }

    public void DestroyFloor()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        foreach(GameObject hero in heroes)
        {
            Debug.Log("floor made hero super fall wacum");
            hero.GetComponent<Hero>().SuperFall();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            Debug.Log("wacum + hero was added to list");
            heroes.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 6)
        {
            Debug.Log("hero was removed from list");
            if (!heroes.Contains(collision.gameObject)) { return;}
            heroes.Remove(collision.gameObject);
        }
    }

}

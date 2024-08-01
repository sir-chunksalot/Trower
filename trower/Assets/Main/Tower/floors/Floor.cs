using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private float cost;
    [SerializeField] private Transform enemyCol;
    TowerBuilder towerBuilder;
    List<Hero> heroes;
    List<GameObject> heroObj;
    private void Awake()
    {
        towerBuilder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TowerBuilder>();
        heroes = new List<Hero>();
        heroObj = new List<GameObject>();
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

        foreach (Hero hero in heroes)
        {
            Debug.Log("floor made hero super fall wacum");
            hero.GetComponent<Hero>().StartSuperFall(-1, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("wacum + hero was added to list");
            Hero hero = collision.gameObject.GetComponent<Hero>();
            if (hero != null)
            {
                heroObj.Add(collision.gameObject);
                heroes.Add(hero);
            }

        }
    }

    public GameObject GetEnemyCol(string name)
    {
        foreach (Transform child in enemyCol)
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("hero was removed from list");
            if (!heroObj.Contains(collision.gameObject)) { return; }
            int index = heroObj.IndexOf(collision.gameObject);
            heroes.RemoveAt(index);
            heroObj.RemoveAt(index);
        }
    }

}

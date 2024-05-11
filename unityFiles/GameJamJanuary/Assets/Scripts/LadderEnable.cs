using System;
using UnityEngine;

public class LadderEnable : MonoBehaviour
{
    private TowerBuilder towerBuilder;
    void Start()
    {
        towerBuilder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TowerBuilder>();
        towerBuilder.onTowerPlace += EnableLadders;
    }

    public void EnableLadders(object sender, EventArgs e)
    {
        if (!gameObject.GetComponent<BoxCollider2D>().enabled)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            //towerBuilder.AddLadders(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (towerBuilder != null)
        {
            towerBuilder.onTowerPlace -= EnableLadders;
            //towerBuilder.RemoveLadders(gameObject);
        }
    }
}

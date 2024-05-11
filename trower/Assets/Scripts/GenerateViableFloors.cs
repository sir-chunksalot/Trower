using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateViableFloors : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    List<GameObject> viableLadders;
    TowerBuilder towerBuilder;
    void Start()
    {
        viableLadders = new List<GameObject>();
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        //towerBuilder.onTowerPlace += NewFloor;
    }

    //private void NewFloor(object sender, EventArgs e)
    //{
    //    StartCoroutine(RaycastDelay());
    //}

    private List<RaycastHit2D> ShootRaycasts(Vector3 startPos)
    {
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(startPos, transform.TransformDirection(Vector3.right), 100, layerMask);
        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(startPos, transform.TransformDirection(Vector3.left), 100, layerMask);

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        bool foundWall = false;
        Vector2 rightWallPos = new Vector2(100 + startPos.x, 0);
        Vector2 lefttWallPos = new Vector2(-100 + startPos.x, 0);
        foreach (RaycastHit2D hit in hitsRight)
        {
            if (hit.collider.gameObject.tag == "EnemyWall" && !foundWall)
            {
                Debug.Log("hit collision" + hit.collider.gameObject + " " + hit.collider.gameObject.tag + " " + hit.collider.gameObject.transform.parent.gameObject.transform.parent.transform.parent.name);
                hits.Add(hit);
                foundWall = true;
                rightWallPos = hit.transform.position;
            }
            else if (hit.collider.gameObject.tag == "BottomOfLadder")
            {
                Debug.Log("hit a ladder");
                if (hit.transform.position.x < rightWallPos.x) //confirms that there isnt a wall seperating this ladder and the start pos
                {
                    if (!hits.Contains(hit))
                    {
                        Debug.Log("hit added (ladder)");
                        hits.Add(hit);
                    }
                    if (!viableLadders.Contains(hit.collider.gameObject))
                    {
                        viableLadders.Add(hit.collider.gameObject);
                    }
                }

            }
        }
        foundWall = false;
        foreach (RaycastHit2D hit in hitsLeft) //confirms that there isnt a wall seperating this ladder and the start pos
        {
            if (hit.collider.gameObject.tag == "EnemyWall" && !foundWall)
            {
                Debug.Log("hit collision" + hit.collider.gameObject + " " + hit.collider.gameObject.tag + " " + hit.collider.gameObject.transform.parent.gameObject.transform.parent.transform.parent.name);
                hits.Add(hit);
                foundWall = true;
                lefttWallPos = hit.transform.position;
            }
            else if (hit.collider.gameObject.tag == "BottomOfLadder")
            {
                if (hit.transform.position.x > lefttWallPos.x)
                {
                    if (!hits.Contains(hit))
                    {
                        hits.Add(hit);
                    }
                    if (!viableLadders.Contains(hit.collider.gameObject))
                    {
                        viableLadders.Add(hit.collider.gameObject);
                    }
                }

            }
        }

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.position.x + "hit here");
                Debug.DrawRay(startPos, transform.TransformDirection(Vector3.right) * Mathf.Sign(hit.transform.position.x - startPos.x) * hit.distance, Color.green, 1);
                Debug.Log("Did Hit");
            }
        }
        if (hits.Count == 0)
        {
            Debug.DrawRay(startPos, transform.TransformDirection(Vector3.right) * 1000, Color.red, 100);
            Debug.DrawRay(startPos, transform.TransformDirection(Vector3.left) * 1000, Color.red, 100);
            Debug.Log("Did not Hit");
        }

        return hits;
    }

    private void ViableFloorss()
    {
        int ladderCount = viableLadders.Count;
        if (ladderCount == 0)
        {
            Vector3 raycastStartPos = Vector3.zero;
            ShootRaycasts(raycastStartPos);
        }
        GameObject[] ladders = viableLadders.ToArray();
        foreach (GameObject ladder in ladders)
        {
            Vector3 raycastStartPos = new Vector3(ladder.transform.position.x, ladder.transform.position.y - (towerBuilder.GetRoomBounds().y / 2), ladder.transform.position.z);
            ShootRaycasts(raycastStartPos);
        }
        if (ladderCount < viableLadders.Count)
        {
            ViableFloorss();
        }
        Debug.Log(viableLadders.Count + "MEN");

    }

    private List<GameObject> GenValidFloors(Vector3 pos)
    {
        List<GameObject> totalFloorConnections = new List<GameObject>();
        List<GameObject> newFloorConnections = new List<GameObject>();
        foreach(RaycastHit2D hit in ShootRaycasts(pos))
        {
            totalFloorConnections.Add(hit.collider.transform.parent.transform.parent.transform.gameObject);
            newFloorConnections.Add(hit.collider.transform.parent.transform.parent.transform.gameObject);
        }

        foreach(GameObject floors in totalFloorConnections)
        {
            Debug.Log("VALID FLOORS: " + floors.name);
        }
        
        return totalFloorConnections;
    }

    public bool GetHasConnection(Vector3 pos)
    {
        if(GenValidFloors(pos).Count >= 1) { //if it has even a single connections, its valid
            return true;
        }
        return false;
    }

    public List<GameObject> GetConnections(Vector3 pos)
    {
        return GenValidFloors(pos);
    }

    public void Test()
    {
        GetConnections(towerBuilder.GetInitialFloorPos());
    }

    //private IEnumerator RaycastDelay()
    //{
    //    viableLadders.Clear();

    //    yield return new WaitForSeconds(.2f);

    //    ViableFloors();
    //}
}

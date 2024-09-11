using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateViableFloors : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    List<GameObject> upRooms;
    List<GameObject> walls;
    List<GameObject> inacessibleRooms;
    List<GameObject> totalRooms;
    List<Vector3> inacessibleRoomsPos;
    TowerBuilder towerBuilder;
    int safetyCount;

    public event EventHandler onFinishedScan;
    void Start()
    {
        upRooms = new List<GameObject>();
        walls = new List<GameObject>();
        inacessibleRooms = new List<GameObject>();
        totalRooms = new List<GameObject>();
        inacessibleRoomsPos = new List<Vector3>();
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
        towerBuilder.onTowerPlaceLate += GenViableFloors;
    }
    public void GenViableFloors(object sender, EventArgs e)
    {
        inacessibleRooms.Clear();
        inacessibleRoomsPos.Clear();
        totalRooms.Clear();
        walls.Clear();
        safetyCount = 0;
        upRooms.Clear();
        ViableFloors();
    }
    private List<RaycastHit2D> ShootRaycasts(Vector3 startPos)
    {
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(startPos, transform.TransformDirection(Vector3.right), 100, layerMask);
        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(startPos, transform.TransformDirection(Vector3.left), 100, layerMask);

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        bool foundWall = false;
        bool foundLadder = false;
        Vector2 rightWallPos = new Vector2(100 + startPos.x, 0);
        Vector2 lefttWallPos = new Vector2(-100 + startPos.x, 0);
        foreach (RaycastHit2D hit in hitsRight)
        {
            if (hit.collider.gameObject.tag == "RaycastWall")
            {
                if(!foundWall)
                {
                    //Debug.Log("right hit collision" + hit.collider.gameObject + " " + hit.collider.gameObject.tag + " " + hit.collider.gameObject.transform.parent.gameObject.transform.parent.transform.parent.name);
                    hits.Add(hit);
                    rightWallPos = hit.transform.position;
                    foundWall = true;
                }
            }
            else if(hit.collider.gameObject.tag == "Wall" && !foundWall)
            {
                hits.Add(hit);
            }
            else if (hit.collider.gameObject.tag == "EnemyWall")
            {
                walls.Add(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.tag == "BottomOfLadder")
            {
                Debug.Log("right hit a ladder" + hit.collider.gameObject + " " + hit.collider.gameObject.transform.position);
                if (hit.transform.position.x < rightWallPos.x) //confirms that there isnt a wall seperating this ladder and the start pos
                {
                    if (!hits.Contains(hit))
                    {
                        Debug.Log("right hit added (ladder)");
                        hits.Add(hit);
                        foundLadder = true;
                    }
                    if (!upRooms.Contains(hit.collider.gameObject.transform.parent.gameObject))
                    {
                        upRooms.Add(hit.collider.gameObject.transform.parent.gameObject);
                    }
                }

            }
        }
        foundWall = false;
        foreach (RaycastHit2D hit in hitsLeft) //confirms that there isnt a wall seperating this ladder and the start pos
        {
            if (hit.collider.gameObject.tag == "RaycastWall")
            {
                if (!foundWall)
                {
                    //Debug.Log("left hit collision" + hit.collider.gameObject + " " + hit.collider.gameObject.tag + " " + hit.collider.gameObject.transform.parent.gameObject.transform.parent.transform.parent.name);
                    hits.Add(hit);
                    lefttWallPos = hit.transform.position;
                    foundWall = true;
                }
            }
            else if (hit.collider.gameObject.tag == "Wall" && !foundWall)
            {
                hits.Add(hit);
            }
            else if (hit.collider.gameObject.tag == "EnemyWall")
            {
                walls.Add(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.tag == "BottomOfLadder")
            {
                Debug.Log("left hit a ladder" + hit.collider.gameObject + " " + hit.collider.gameObject.transform.position);
                if (hit.transform.position.x > lefttWallPos.x)
                {
                    if (!hits.Contains(hit))
                    {
                        Debug.Log("left hit added (ladder)");
                        hits.Add(hit);
                        foundLadder = true;
                    }
                    if (!upRooms.Contains(hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject))
                    {
                        upRooms.Add(hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject);
                    }
                }

            }
        }
        Color color = Color.green;

        if (foundWall == false)
        {
            color = Color.yellow;
        }
        else if (foundLadder == false)
        {
            color = Color.blue;
        }

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                GameObject room = hit.collider.gameObject.transform.parent.transform.parent.gameObject;
                if (foundLadder == false)
                {
                    if (!inacessibleRoomsPos.Contains(room.transform.position))
                    {
                        inacessibleRooms.Add(room);
                        inacessibleRoomsPos.Add(room.transform.position);
                    }
                }
                if (!totalRooms.Contains(room))
                {
                    totalRooms.Add(room);
                }
                

                Debug.Log(hit.transform.position.x + "hit here" + transform.TransformDirection(Vector3.right) * Mathf.Sign(hit.transform.position.x - startPos.x) * hit.distance);
                Debug.DrawRay(startPos, transform.TransformDirection(Vector3.right) * Mathf.Sign(hit.transform.position.x - startPos.x) * hit.distance, color, 2);
                Debug.Log("Did Hit");
            }
        }
        if (hits.Count == 0)
        {
            //Debug.DrawRay(startPos, transform.TransformDirection(Vector3.right) * 1000, Color.red, 100);
            //Debug.DrawRay(startPos, transform.TransformDirection(Vector3.left) * 1000, Color.red, 100);
            Debug.Log("Did not Hit");
        }

        return hits;
    }

    private void ViableFloors()
    {
        GameObject[] towerBuilderFloors = (GameObject[])towerBuilder.GetPlacedFloors().ToArray().Clone();
        safetyCount++;
        foreach (GameObject floors in towerBuilderFloors)
        {
            if (!totalRooms.Contains(floors))
            {
                Vector3 raycastStartPos = new Vector3(floors.transform.position.x, floors.transform.position.y - 1, floors.transform.position.z);
                ShootRaycasts(raycastStartPos);
            }
        }
        if (totalRooms.Count < towerBuilderFloors.Length && safetyCount < 100)
        {
            ViableFloors();
        }
        else
        {
            Debug.Log("FINISHED SCAN!!!" + safetyCount);
            onFinishedScan?.Invoke(gameObject, EventArgs.Empty);
        }
    }

    public void Test()
    {
        inacessibleRooms.Clear();
        inacessibleRoomsPos.Clear();
        totalRooms.Clear();
        walls.Clear();
        safetyCount = 0;
        upRooms.Clear();
        ViableFloors();
        Debug.Log("inacessibleRooms count" + inacessibleRooms.Count);
        foreach (GameObject room in inacessibleRooms)
        {
            Debug.Log("inacessibleRooms Room: " + room + "pos" + room.transform.position);
        }
    }

    public bool IsFloorAccessible(Vector2 pos)
    {
        Debug.Log("japan lol real" + pos);
        foreach(GameObject floor in GetInaccessibleFloors())
        {
            Debug.Log("japan lol real check" + floor.transform.position);
            if (new Vector2(floor.transform.position.x, floor.transform.position.y) == pos)
            {
                return false;
            }
        }
        return true;
    }
    public List<GameObject> GetInaccessibleFloors()
    {
        return inacessibleRooms;
    }

    public List<GameObject> GetWalls()
    {
        return walls;
    }

    //private IEnumerator RaycastDelay()
    //{
    //    viableLadders.Clear();

    //    yield return new WaitForSeconds(.2f);

    //    ViableFloors();
    //}
}

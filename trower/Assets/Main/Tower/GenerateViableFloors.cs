using System.Collections.Generic;
using UnityEngine;

public class GenerateViableFloors : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    List<GameObject> upRooms;
    List<GameObject> inacessibleRooms;
    List<Vector3> inacessibleRoomsPos;
    TowerBuilder towerBuilder;
    void Start()
    {
        upRooms = new List<GameObject>();
        Debug.Log("naht initialized");
        inacessibleRooms = new List<GameObject>();
        inacessibleRoomsPos = new List<Vector3>();
        towerBuilder = gameObject.GetComponent<TowerBuilder>();
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
            if (hit.collider.gameObject.tag == "EnemyWall" && !foundWall)
            {
                Debug.Log("right hit collision" + hit.collider.gameObject + " " + hit.collider.gameObject.tag + " " + hit.collider.gameObject.transform.parent.gameObject.transform.parent.transform.parent.name);
                hits.Add(hit);
                foundWall = true;
                rightWallPos = hit.transform.position;
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
            if (hit.collider.gameObject.tag == "EnemyWall" && !foundWall)
            {
                Debug.Log("left hit collision" + hit.collider.gameObject + " " + hit.collider.gameObject.tag + " " + hit.collider.gameObject.transform.parent.gameObject.transform.parent.transform.parent.name);
                hits.Add(hit);
                foundWall = true;
                lefttWallPos = hit.transform.position;
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
                    if (!upRooms.Contains(hit.collider.gameObject.transform.parent.gameObject))
                    {
                        upRooms.Add(hit.collider.gameObject.transform.parent.gameObject);
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
                if (foundLadder == false)
                {
                    GameObject room = hit.collider.gameObject.transform.parent.transform.parent.gameObject;
                    if (!inacessibleRoomsPos.Contains(room.transform.position))
                    {
                        inacessibleRooms.Add(room);
                        inacessibleRoomsPos.Add(room.transform.position);
                    }

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
        int ladderCount = upRooms.Count;
        if (ladderCount == 0)
        {
            Vector3 raycastStartPos = Vector3.zero;
            ShootRaycasts(raycastStartPos);
        }
        GameObject[] ladders = upRooms.ToArray();
        foreach (GameObject ladder in ladders)
        {
            Vector3 raycastStartPos = new Vector3(ladder.transform.position.x, ladder.transform.position.y + (towerBuilder.GetRoomBounds().y) / 2, ladder.transform.position.z);

            Debug.Log("START POS!! " + raycastStartPos + "candy" + ladder.transform.position.y + (towerBuilder.GetRoomBounds().y) / 2 + "shit" + towerBuilder.GetRoomBounds().y);
            ShootRaycasts(raycastStartPos);
        }
        if (ladderCount < upRooms.Count)
        {
            ViableFloors();
        }
        Debug.Log(upRooms.Count + "MEN");

    }

    public void Test()
    {
        upRooms.Clear();
        ViableFloors();
        Debug.Log("inacessibleRooms count" + inacessibleRooms.Count);
        foreach (GameObject room in inacessibleRooms)
        {
            Debug.Log("inacessibleRooms Room: " + room + "pos" + room.transform.position);
        }
    }

    public bool IsFloorAccessible(Vector3 pos)
    {
        foreach(GameObject floor in GetInaccessibleFloors())
        {
            if(floor.transform.position == pos)
            {
                return false;
            }
        }
        return true;
    }

    public List<GameObject> GetInaccessibleFloors()
    {
        if (towerBuilder == null)
        {
            Debug.Log("naht tower IS NULL");
            return null;
        }
        upRooms.Clear();
        inacessibleRooms.Clear();
        inacessibleRoomsPos.Clear();
        ViableFloors();
        foreach (GameObject room in inacessibleRooms)
        {
            Debug.Log("inacessibleRooms Room: " + room + "pos" + room.transform.position);
        }
        return inacessibleRooms;
    }

    //private IEnumerator RaycastDelay()
    //{
    //    viableLadders.Clear();

    //    yield return new WaitForSeconds(.2f);

    //    ViableFloors();
    //}
}

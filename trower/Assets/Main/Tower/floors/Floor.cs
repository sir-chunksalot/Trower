using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    List<FloorDestroy> floorDestroyables;
    List<GameObject> followers;
    private TowerBuilder towerBuilder;
    public bool floorDestroyed;
    private void Awake()
    {
        towerBuilder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TowerBuilder>();
        floorDestroyables = new List<FloorDestroy>();
        followers = new List<GameObject>();
        foreach (FloorDestroy floorDestroyable in gameObject.GetComponentsInChildren<FloorDestroy>())
        {
            floorDestroyables.Add(floorDestroyable);
        }
    }
    public void DestroyFloor()
    {
        floorDestroyed = true;
        foreach (FloorDestroy floorDestroyable in floorDestroyables)
        {
            floorDestroyable.DestroyFloor();
        }

        foreach(GameObject follower in followers)
        {
            Debug.Log("follower list!" + follower);
            Destroy(follower);
        }

        StartCoroutine(WaitToDelete());
    }

    private IEnumerator WaitToDelete()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    public TowerBuilder GetTowerBuilder()
    {
        return towerBuilder;
    }

    public void AddFollower(GameObject follower)
    {
        followers.Add(follower);
    }

}

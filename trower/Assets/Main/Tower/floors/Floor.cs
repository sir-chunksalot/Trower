using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] BoxCollider2D[] activateMe;
    public bool isPlaced;
    List<FloorDestroy> floorDestroyables;
    List<GameObject> followers;
    public event EventHandler onFloorPlace;
    public bool floorDestroyed;
    private void Awake()
    {
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

    public void PlaceFloor()
    {
        isPlaced = true;
        foreach(BoxCollider2D thing in activateMe)
        {
            thing.enabled = true;
        }
        onFloorPlace?.Invoke(gameObject, EventArgs.Empty);
    }

    private IEnumerator WaitToDelete()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
    public void AddFollower(GameObject follower)
    {
        followers.Add(follower);
    }

}

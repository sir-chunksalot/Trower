using UnityEngine;

public class LevelDetails : MonoBehaviour
{
    [SerializeField] public Vector2Int gridSize;
    public ArrayLayout existingFloors;
    [SerializeField] public bool fixedCamera;
    [SerializeField] public GameObject sceneCamera;
    [SerializeField] public bool startAttackPhase;
    [SerializeField] public bool startDefensePhase;
    [SerializeField] public bool hideProgressbar;
    [SerializeField] public bool hideBuildMenu;
    [SerializeField] public bool hideCoinCount;
    [SerializeField] public bool useDoor;
    [SerializeField] public bool canMoveDoor;
    [SerializeField] public bool enemiesCanWin;
    [SerializeField] public bool isEndScene;
    [SerializeField] public GameObject trapDaddy;
    [SerializeField] public GameObject roundDaddy;
    [SerializeField] public Transform[] spawnSpots;
    [SerializeField] public GameObject heroDaddy;
    [SerializeField] public GameObject buildDaddy;


    public GameObject GetRoundDaddy()
    {
        if (roundDaddy == null) return null;
        return roundDaddy;
    }
    public GameObject GetTrapDaddy()
    {
        if (trapDaddy == null) return null;
        return trapDaddy;
    }

    public Transform[] GetSpawnSpots()
    {
        if (spawnSpots == null) return null;
        Debug.Log("Current spawn spots: " + spawnSpots);
        return spawnSpots;
    }

    public GameObject GetHeroDaddy()
    {
        if (heroDaddy == null) return null;
        return heroDaddy;
    }

    public GameObject GetBuildDaddy()
    {
        if (buildDaddy == null) return null;
        return buildDaddy;
    }

#if (UNITY_EDITOR)
    [ContextMenu("UpdateGrid")]
    void Run()
    {
        existingFloors.xBounds = gridSize.x;
        existingFloors.yBounds = gridSize.y;

        Test();
    }
    void Test()
    {
        
    }

#endif


}

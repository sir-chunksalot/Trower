using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    LevelDetails level;
    [Header("SCRIPT DEPENDENCIES: WAVEMANAGER")]
    [SerializeField] List<GameObject> heroTypes;
    [SerializeField] GameObject bloodAnim;
    [SerializeField] GameObject bloodEffect;
    [SerializeField] Sprite[] bloodEffects;
    //[SerializeField] GameObject[] spawnSpots;
    //[SerializeField] GameObject heroDaddy;
    [SerializeField] GameObject bloodDaddy;

    GameManager gameManagerScript;
    TurnManager turnManager;

    Transform[] spawnSpots;
    GameObject heroDaddy;

    DoorManager doorManager;
    GridManager gridManager;
    public event EventHandler OnHeroDeath;

    List<AudioSource> bloodSounds;
    List<Hero> currentHeroes;
    List<GameObject> spawnableHeroes;
    List<GameObject> blood;
    List<Vector2> spawnrate;
    List<Vector2> cooldown;
    List<Vector2Int> groupsize;
    List<Vector2> delay;
    List<bool> cooldownTracker;
    bool finalWave;
    int heroCount;
    int killCount;
    bool moveHeroes;
    int movedHeroCount;

    private void Awake()
    {
        gameManagerScript = gameObject.GetComponent<GameManager>();
        gameManagerScript.OnSceneLoaded += OnSceneLoad;
        turnManager = gameObject.GetComponent<TurnManager>();
        turnManager.OnEnemyTurn += OnNewEnemyTurn;
        doorManager = gameObject.GetComponent<DoorManager>();
        gridManager = gameObject.GetComponent<GridManager>();
        //waveManager.OnDefensePhaseStart += StopSpawning;
        //waveManager.OnNewWave += StartSpawning;
        bloodSounds = new List<AudioSource>();
        blood = new List<GameObject>();
        currentHeroes = new List<Hero>();
        spawnableHeroes = new List<GameObject>();
        spawnrate = new List<Vector2>();
        cooldown = new List<Vector2>();
        groupsize = new List<Vector2Int>();
        delay = new List<Vector2>();
        cooldownTracker = new List<bool>();
        finalWave = false;

        foreach (AudioSource audioSource in bloodDaddy.GetComponentsInChildren<AudioSource>())
        {
            bloodSounds.Add(audioSource);
        }

    }

    private void OnSceneLoad(object sender, EventArgs e)
    {
        Setup();
    }

    private void Setup()
    {
        level = gameManagerScript.GetCurrentLevelDetails();
        spawnSpots = level.GetSpawnSpots();
        heroDaddy = level.GetHeroDaddy();

        foreach (GameObject obj in blood)
        {
            Destroy(obj);
        }
        blood.Clear();
    }
    public void KillHero(GameObject hero, float bloodSize = 1, float bloodFadeTime = 0)
    {

        Debug.Log("hero is being killed :sob:");
        Vector3 spawnPos = hero.transform.position;
        spawnPos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z + .1f); //puts it behind heroes 
        hero.GetComponent<BoxCollider2D>().enabled = false;
        hero.GetComponent<SpriteRenderer>().enabled = false;
        hero.transform.position = new Vector3(999, 999, 999);

        int max = bloodSounds.Count;
        int index = UnityEngine.Random.Range(0, max);
        Debug.Log("nick12" + bloodSounds[index] + " " + bloodSounds.Count);
        bloodSounds[index].Play();

        GameObject blood = Instantiate(bloodAnim, spawnPos, Quaternion.identity, bloodDaddy.transform);
        blood.transform.localScale = blood.transform.localScale * bloodSize;
        StartCoroutine(BloodAnim(blood, spawnPos, bloodFadeTime));
        StartCoroutine(DestroyObjectAfterTime(hero));
        currentHeroes.Remove(hero.GetComponent<Hero>());
        killCount++;
        OnHeroDeath?.Invoke(gameObject, EventArgs.Empty);

    }

    private IEnumerator BloodFade(GameObject blood, SpriteRenderer sprite, float time, int count = 0)
    {
        Color opacity = sprite.color;
        opacity.a = 100 - count;
        sprite.color = opacity;
        yield return new WaitForSeconds(time);
        if (count >= 99)
        {
            Destroy(blood);
        }
        else
        {
            StartCoroutine(BloodFade(blood, sprite, time, count++));
        }

    }

    private IEnumerator DestroyObjectAfterTime(GameObject hero)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Destroyed hero");
        Destroy(hero);
    }

    private IEnumerator BloodAnim(GameObject bloodAnim, Vector3 spawnPos, float bloodFadeTime)
    {
        yield return new WaitForSeconds(.4f);
        Destroy(bloodAnim);
        GameObject newBlood = Instantiate(bloodEffect, spawnPos, Quaternion.identity, bloodDaddy.transform);
        blood.Add(newBlood);
        SpriteRenderer bloodSprite = newBlood.GetComponent<SpriteRenderer>();
        bloodSprite.sprite = PickBloodEffect();
        BloodFade(newBlood, bloodSprite, bloodFadeTime / 100);

    }

    private Sprite PickBloodEffect()
    {
        int count = bloodEffects.Length - 1;
        int randomNum = UnityEngine.Random.Range(0, count);
        return bloodEffects[randomNum];
    }

    public void AddHeroToList(Hero hero)
    {
        currentHeroes.Add(hero);
    }

    public int GetKillCount()
    {
        return killCount;
    }

    public float TopOfMap()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y;
    }

    //spawn heroes

    public void ClearWaveData()
    {
        spawnrate.Clear();
        cooldown.Clear();
        groupsize.Clear();
        spawnableHeroes.Clear();
        cooldownTracker.Clear();
        delay.Clear();
    }

    //public void StopSpawning(object sender, EventArgs e) //called when build phase starts
    //{
    //    Debug.Log("STOP IT ALL ADAM 12!!");
    //    ClearWaveData();
    //    CancelInvoke("Spawner");
    //    StopAllCoroutines();
    //}

    public GameObject GetHero(string heroName)
    {
        foreach (GameObject hero in heroTypes)
        {
            Debug.Log(heroName + "   |   " + hero.name);
            if (hero.name == heroName)
            {
                return hero;
            }
        }
        return null;
    }

    public GameObject[] GetAllHeroes()
    {
        return heroTypes.ToArray();
    }

    public GridSpace GetClosestGridSpaceToHero(Hero hero)
    {
        GridSpace gridSpace = gridManager.GetClosestGridSpace(hero.transform.position, false);
        return gridSpace;
    }

    public void SpawnHero(GameObject hero, int distanceFromGrid)
    {
        Vector2 startOfMap = gridManager.GetGridSpace(0, 0).GetPos();
        if (distanceFromGrid > 0) { startOfMap = gridManager.GetGridSpace(gridManager.GetGridSize().x - 1, 0).GetPos(); }

        Vector2 spawnPos = new Vector2((distanceFromGrid * gridManager.GetCellSize().x) + startOfMap.x, 0);

        GameObject newHero = Instantiate(hero, spawnPos, Quaternion.identity, heroDaddy.transform);
        Hero heroScript = newHero.GetComponent<Hero>();
        heroScript.Birth(this);
        heroScript.SetDistFromGrid(distanceFromGrid);
        AddHeroToList(heroScript);
    }

    public void OnNewEnemyTurn(object sender, EventArgs e)
    {
        foreach (Hero hero in currentHeroes.ToArray())
        {
            hero.Restore();
        }

        movedHeroCount = 0;
        moveHeroes = true;
    }

    private void Update()
    {
        MoveHeroes();
    }

    public void MoveHeroes()
    {
        if (!moveHeroes) { return; } //dont move heroes if you're not supposed to


        if (movedHeroCount >= currentHeroes.Count)
        {
            if (currentHeroes.Count == 0) { moveHeroes = false; turnManager.EnemiesEvaluated(); return; }

            Hero lastHero = currentHeroes[movedHeroCount - 1];
            if (!lastHero.moving)
            {
                moveHeroes = false;
                turnManager.EnemiesEvaluated();
            }
        }
        else
        {
            Debug.Log("movedHeroCount" + movedHeroCount);
            Hero currentHero = currentHeroes[movedHeroCount];
            if (currentHero.GetCurrentStamina() > 0 && !currentHero.moving)
            { //if hero can move, move it.
                if (currentHero.GetDistanceFromGrid() != 0) { currentHero.GetCloserToGrid(); }
                Vector2 targetMove = GetTargetMove(currentHero);
                currentHero.Move(targetMove);
                Debug.Log("this was my target move!" + targetMove);
            }
            else
            { //otherwise, move on to the next hero.
                movedHeroCount++;
            }
        }
    }

    private Vector2 GetTargetMove(Hero hero)
    {
        //get to map
        if (hero.GetDistanceFromGrid() != 0)
        {
            Vector2 startOfMap = gridManager.GetGridSpace(0, 0).GetPos();
            if (hero.GetDistanceFromGrid() > 0) { startOfMap = gridManager.GetGridSpace(gridManager.GetGridSize().x - 1, 0).GetPos(); }
            Vector2 targetMove = new Vector2((hero.GetDistanceFromGrid() * gridManager.GetCellSize().x) + startOfMap.x, 0);
            return targetMove;
        }
        Debug.Log("I can see the grid!");
        GridSpace currentCell = gridManager.GetClosestGridSpace(hero.transform.position, false);
        if (currentCell != hero.GetCurrentCell()) { return currentCell.GetPos(); }
        Debug.Log("I made it to the grid!");


        //climb ladder
        if (currentCell.GetCurrentLadder() != null)
        {
            return gridManager.GetGridSpace(currentCell.GetIndex().x, currentCell.GetIndex().y + 1).GetPos();
        }

        //find target
        GridSpace targetCell = currentCell;
        for (int i = 0; i < gridManager.GetGridSize().x; i++)
        {
            GridSpace newCell = gridManager.GetGridSpace(i, currentCell.GetIndex().y);
            Debug.Log(newCell + " NEW CELL");
            if (newCell.GetCurrentLadder() != null)
            {
                Debug.Log("NEW CELL FOUND LADDER");
                targetCell = gridManager.GetGridSpace(i, currentCell.GetIndex().y);
            }
            if (newCell.GetHasDoor())
            {
                Debug.Log("NEW CELL FOUND DOOR");
                targetCell = newCell;
                break;
            }
        }

        //find paths to target
        GridSpace[] adjacentCells = gridManager.GetAdjacentCells(currentCell);
        Vector2 leftMove = currentCell.GetPos();
        Vector2 rightMove = currentCell.GetPos();
        bool onGround = false;
        if (currentCell.GetPos().y == 0) { onGround = true; }
        if (adjacentCells[1] != null && (adjacentCells[1].GetCurrentFloor() != null || onGround)) { leftMove = adjacentCells[1].GetPos(); Debug.Log("this was my left option" + leftMove); }
        if (adjacentCells[2] != null && (adjacentCells[2].GetCurrentFloor() != null || onGround)) { rightMove = adjacentCells[2].GetPos(); Debug.Log("this was my right option" + rightMove); }

        //determine best path to target
        float distLeft = Vector2.Distance(leftMove, targetCell.GetPos());
        Debug.Log("this was my left dist" + distLeft);
        float distRight = Vector2.Distance(rightMove, targetCell.GetPos());
        Debug.Log("this was my right dist" + distRight);


        if (distLeft < distRight)
        {
            return leftMove;
        }
        else
        {
            return rightMove;
        }

    }
}

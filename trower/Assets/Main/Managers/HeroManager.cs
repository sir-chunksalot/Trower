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

    Transform[] spawnSpots;
    GameObject heroDaddy;

    WaveManager waveManager;
    DoorManager doorManager;
    public event EventHandler OnHeroDeath;

    List<AudioSource> bloodSounds;
    List<GameObject> heroes;
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

    private void Awake()
    {
        gameManagerScript = gameObject.GetComponent<GameManager>();
        gameManagerScript.OnSceneLoaded += OnSceneLoad;
        waveManager = gameObject.GetComponent<WaveManager>();
        doorManager = gameObject.GetComponent<DoorManager>();
        waveManager.OnDefensePhaseStart += StopSpawning;
        waveManager.OnNewWave += StartSpawning;
        bloodSounds = new List<AudioSource>();
        blood = new List<GameObject>();
        heroes = new List<GameObject>();
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

    public Vector3 GetDoorPos()
    {
        return doorManager.GetDoorPos();
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
        heroes.Remove(hero);
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

    public void AddHeroToList(GameObject hero)
    {
        heroes.Add(hero);
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

    public void StopSpawning(object sender, EventArgs e) //called when build phase starts
    {
        Debug.Log("STOP IT ALL ADAM 12!!");
        ClearWaveData();
        CancelInvoke("Spawner");
        StopAllCoroutines();
    }

    public GameObject GetHero(string heroName)
    {
        foreach(GameObject hero in heroTypes)
        {
            Debug.Log(heroName + "   |   " + hero.name);
            if(hero.name == heroName)
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

    public void StartSpawning(object sender, EventArgs e) //called when a wave starts
    {
        if (waveManager.GetCurrentWave() == null) return;
        StopSpawning(gameObject, EventArgs.Empty);
        if (waveManager.GetIsDefensePhase())
        {
            return;
        }
        if (waveManager.GetCurrentWave().finalWave)
        {
            Debug.Log("hero maanger donezo");
            return;
        }
        foreach (GameObject hero in heroTypes)
        {
            if (waveManager.GetCurrentWave().GetHero(hero.tag) != null)
            {
                Wave.Hero heroData = waveManager.GetCurrentWave().GetHero(hero.tag);
                if (!heroData.canSpawn)
                {
                    continue;
                }
                spawnrate.Add(heroData.spawnRate);
                cooldown.Add(heroData.cooldown);
                cooldownTracker.Add(false);
                groupsize.Add(heroData.groupSize);
                delay.Add(heroData.delay);
                spawnableHeroes.Add(hero);
            }
        }
        InvokeRepeating("Spawner", 0, 1f);
    }

    private void Spawner()
    {
        Debug.Log("adam12 SPAWNER STARTED!");
        int count = 0;
        foreach (GameObject hero in spawnableHeroes)
        {

            if (cooldownTracker[count])
            {
                continue;
            }
            float spawnRate = UnityEngine.Random.Range(spawnrate[count].x, spawnrate[count].y);
            float value = UnityEngine.Random.Range(0.0f, 1.0f);
            if (spawnRate < value)
            {
                continue;
            }
            float delay = UnityEngine.Random.Range(this.delay[count].x, this.delay[count].y);
            int groupSize = UnityEngine.Random.Range(groupsize[count].x, groupsize[count].y + 1);
            StartCoroutine(SpawnHero(hero, this.delay[count].x, this.delay[count].y, groupSize));

            float cooldown = UnityEngine.Random.Range(this.cooldown[count].x, this.cooldown[count].y);

            Debug.Log("adam12 spawnrate" + spawnRate + "delay" + delay + "group size" + groupSize + "cooldown" + cooldown);

            StartCoroutine(Cooldown(cooldown, count));

            count++;
        }
    }

    private IEnumerator SpawnHero(GameObject hero, float minDelay, float maxDelay, int groupSize)
    {
        float delay = UnityEngine.Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);
        Debug.Log("adam12 SPAWNING HERO!");

        if (spawnSpots == null || spawnSpots.Length == 0)
        {
            Debug.Log("HeroManager tried to spawn but there was no valid spawn spots.");
            yield break;
        }

        int spawnDoor = UnityEngine.Random.Range(0, spawnSpots.Length);
        Vector3 spawnpos = spawnSpots[spawnDoor].transform.position;

        GameObject newHero = Instantiate(hero, spawnpos, Quaternion.identity, heroDaddy.transform);
        newHero.GetComponent<SpriteRenderer>().sortingOrder = heroCount;

        SpawnSpot spawnSpot = spawnSpots[spawnDoor].GetComponent<SpawnSpot>();
        Hero heroScript = newHero.GetComponent<Hero>();
        if (spawnSpot.faceRight && spawnSpot.faceLeft)
        {
            int randInt = UnityEngine.Random.Range(0, 2);
            if (randInt == 0)
            {
                heroScript.FlipRight();
            }
        }
        else if (spawnSpot.faceRight)
        {
            Debug.Log("tried to flip hero right");
            heroScript.FlipRight();
        }

        newHero.GetComponent<Hero>().AttackPhase();
        int newGroupSize = groupSize - 1;
        if (groupSize >= 1)
        {
            SpawnHero(hero, minDelay, maxDelay, newGroupSize);
        }
        heroCount++;

    }

    public void SpawnHero(GameObject hero, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            StartCoroutine(SpawnHero(hero, 0, 0, amount));
        }
    }

    private IEnumerator Cooldown(float cooldown, int index)
    {
        cooldownTracker[index] = true;
        yield return new WaitForSeconds(cooldown);
        cooldownTracker[index] = false;
    }
}

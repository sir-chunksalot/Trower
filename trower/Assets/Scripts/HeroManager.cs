using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    [Header("SCRIPT DEPENDENCIES: WAVEMANAGER")]
    [SerializeField] List<GameObject> heroTypes;
    [SerializeField] GameObject bloodAnim;
    [SerializeField] GameObject bloodEffect;
    [SerializeField] Sprite[] bloodEffects;
    [SerializeField] GameObject[] spawnSpots;
    [SerializeField] GameObject heroDaddy;
    [SerializeField] GameObject bloodDaddy;

    WaveManager waveManager;
    public event EventHandler OnHeroDeath;

    List<AudioSource> bloodSounds;
    List<GameObject> heroes;
    List<GameObject> spawnableHeroes;
    List<Vector2> spawnrate;
    List<Vector2> cooldown;
    List<Vector2Int> groupsize;
    List<Vector2> delay;
    List<bool> cooldownTracker;
    bool finalWave;
    int killCount;

    private void Awake()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        if(waveManager == null) {
            Debug.Log("Error. Class 'WaveManager' not found.");
            return;
        }
        waveManager.OnDefensePhaseStart += StopSpawning;
        waveManager.OnNewWave += StartSpawning;
        bloodSounds = new List<AudioSource>();
        heroes = new List<GameObject>();
        spawnableHeroes = new List<GameObject>();
        spawnrate = new List<Vector2>();
        cooldown = new List<Vector2>();
        groupsize = new List<Vector2Int>();
        delay = new List<Vector2>();
        cooldownTracker = new List<bool>();
        finalWave = false;

        foreach(AudioSource audioSource in bloodDaddy.transform.parent.GetComponentsInChildren<AudioSource>())
        {
            bloodSounds.Add(audioSource);
        }

    }
    public void KillHero(GameObject hero)
    {
        if (finalWave) //rn it does the same thing as if it were a regular wave, later add a cooler effect
        {
            Debug.Log("hero is being killed :sob:");
            Vector3 spawnPos = hero.transform.position;
            spawnPos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z + .1f); //puts it behind heroes 
            hero.GetComponent<BoxCollider2D>().enabled = false;
            hero.GetComponent<SpriteRenderer>().enabled = false;
            GameObject blood = Instantiate(bloodAnim, spawnPos, Quaternion.identity, bloodDaddy.transform);
            StartCoroutine(BloodAnim(blood, spawnPos));
            StartCoroutine(DestroyObjectAfterTime(hero));
            heroes.Remove(hero);
            killCount++;
            OnHeroDeath?.Invoke(gameObject, EventArgs.Empty);
        }
        else
        {
            Debug.Log("hero is being killed :sob:");
            Vector3 spawnPos = hero.transform.position;
            spawnPos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z + .1f); //puts it behind heroes 
            hero.GetComponent<BoxCollider2D>().enabled = false;
            hero.GetComponent<SpriteRenderer>().enabled = false;

            int max = bloodSounds.Count;
            int index = UnityEngine.Random.Range(0, max);
            Debug.Log("nick12" + bloodSounds[0] + " " + bloodSounds.Count);
            bloodSounds[index].Play();

            GameObject blood = Instantiate(bloodAnim, spawnPos, Quaternion.identity, bloodDaddy.transform);
            StartCoroutine(BloodAnim(blood, spawnPos));
            StartCoroutine(DestroyObjectAfterTime(hero));
            heroes.Remove(hero);
            killCount++;
            OnHeroDeath?.Invoke(gameObject, EventArgs.Empty);
        }

    }

    private IEnumerator DestroyObjectAfterTime(GameObject hero)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Destroyed hero");
        Destroy(hero);
    }

    private IEnumerator BloodAnim(GameObject bloodAnim, Vector3 spawnPos)
    {
        yield return new WaitForSeconds(.4f);
        Destroy(bloodAnim);
        GameObject newBlood = Instantiate(bloodEffect, spawnPos, Quaternion.identity, bloodDaddy.transform);
        newBlood.GetComponent<SpriteRenderer>().sprite = PickBloodEffect();
        
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

    public void StartSpawning(object sender, EventArgs e) //called when a wave starts
    {
        StopSpawning(gameObject, EventArgs.Empty);
        if (waveManager.GetIsDefensePhase()) {
            return;
        }
        if(waveManager.GetCurrentWave().finalWave) {
            Debug.Log("hero maanger donezo");
            return;
        }
        foreach (GameObject hero in heroTypes)
        {
            if (waveManager.GetCurrentWave().GetHero(hero.tag)  != null) {
                Wave.Hero heroData = waveManager.GetCurrentWave().GetHero(hero.tag);
                if(!heroData.canSpawn) {
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
            
            if(cooldownTracker[count]) {
                continue;
            }
            float spawnRate = UnityEngine.Random.Range(spawnrate[count].x, spawnrate[count].y);
            float value = UnityEngine.Random.Range(0.0f, 1.0f);
            if (spawnRate < value ) {
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

        int spawnDoor = UnityEngine.Random.Range(0, spawnSpots.Length);
        Vector3 spawnpos = spawnSpots[spawnDoor].transform.position;

        GameObject newHero = Instantiate(hero, spawnpos, Quaternion.identity, heroDaddy.transform);

        SpawnSpot spawnSpot = spawnSpots[spawnDoor].GetComponent<SpawnSpot>();
        Hero heroScript = newHero.GetComponent<Hero>();
        if (spawnSpot.faceRight && spawnSpot.faceLeft) {
            int randInt = UnityEngine.Random.Range(0, 2);
            if(randInt == 0)
            {
                heroScript.FlipRight(); 
            }
        } 
        else if(spawnSpot.faceRight)
        {
            Debug.Log("tried to flip hero right");
            heroScript.FlipRight();
        }

        newHero.GetComponent<Hero>().AttackPhase();
        int newGroupSize = groupSize - 1;
        if(groupSize >= 1)
        {
            SpawnHero(hero, minDelay, maxDelay, newGroupSize);
        }


    }

    private IEnumerator Cooldown(float cooldown, int index)
    {
        cooldownTracker[index] = true;
        yield return new WaitForSeconds(cooldown);
        cooldownTracker[index] = false;
    }


}

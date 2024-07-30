using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    //[SerializeField] GameObject[] roundList;
    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject progresssFillObj;
    [SerializeField] GameObject waveFlag;
    [SerializeField] GameObject attackPhaseEffect;
    [SerializeField] GameObject defensePhaseEffect;
    [SerializeField] GameObject bar;
    [SerializeField] GameObject endLevelParticles;

    private GameManager gameManagerScript;

    private LevelDetails level;
    private GameObject[] roundList;

    private List<Wave> waves;
    private Wave activeWave;

    private HeroManager heroManager;

    public event EventHandler OnAttackPhaseStart;
    public event EventHandler OnDefensePhaseStart;
    public event EventHandler OnNewWave;
    public event EventHandler OnFinalWave;

    private Image progressFill;

    private bool attackPhase;
    private bool defensePhase;

    private GameObject[] activeRound;

    private int roundCount;
    private int waveCount;
    private int enemiesKilled;
    private int timePassed;
    private float progressbarIncrement;
    private float waveFlagIncrement;
    private float fillAmount;
    private float waveFlagHeight;
    private int length;
    private bool isTime;
    private void Awake()
    {
        heroManager = gameObject.GetComponent<HeroManager>();
        heroManager.OnHeroDeath += EnemyDied;

        gameManagerScript = gameObject.GetComponent<GameManager>();
        gameManagerScript.OnSceneChange += OnSceneLoad;
    }
    private void OnSceneLoad(object sender, EventArgs e)
    {
        Setup();
    }

    private void Setup()
    {
        level = gameManagerScript.GetCurrentLevelDetails();
        Debug.Log("LOOKINATME" + level);
        GameObject roundDaddy = level.GetRoundDaddy();
        StopAllCoroutines();

        roundCount = 0;
        waveCount = 0;
        enemiesKilled = 0;
        timePassed = 0;
        isTime = false;
        progressbarIncrement = 0;
        if (roundDaddy != null)
        {
            List<GameObject> tempList = new List<GameObject>();
            foreach (Transform obj in roundDaddy.transform)
            {
                tempList.Add(obj.gameObject);
            }
            roundList = tempList.ToArray();
            activeRound = GetWaveList(0);
        }

        waves = new List<Wave>();
        if (roundDaddy != null)
        {
            if (progressBar != null)
            {
                MakeProgressBar();
                progressFill = progresssFillObj.GetComponent<Image>();
                fillAmount = 0;
                progressFill.fillAmount = fillAmount;
            }
            else
            {
                activeWave = activeRound[0].GetComponent<Wave>();
            }
            UpdateLength();
        }

        Debug.Log("garnet" + level + level.startAttackPhase);

        if (level.startAttackPhase)
        {
            Debug.Log("TRIED SWITCHING");
            SwitchAttackPhase(false);
        }
        else if (level.startDefensePhase)
        {
            SwitchDefensePhase(false);
        }

        OnNewWave?.Invoke(gameObject, EventArgs.Empty);
    }

    private GameObject[] GetWaveList(int index)
    {   
        Transform[] roundKids = roundList[index].GetComponentsInChildren<Transform>();
        Debug.Log(roundKids.Length + "PIRATECUM");
        GameObject[] waveList = new GameObject[roundKids.Length - 1];
        int count = 0;
        foreach (Transform kid in roundKids)
        {
            if (kid == roundKids[0]) { continue; }
            Debug.Log("PIRATECUM!" + count);
            waveList[count] = kid.gameObject;
            count++;
        }
        return waveList;
    }

    private void UpdateLength()
    {
        if (GetCurrentWave().waveLength.y > 0)
        {  //set up for if the progress bar goes off of time
            length = Random.Range(GetCurrentWave().waveLength.x, GetCurrentWave().waveLength.y);
            Debug.Log("JOHN 12 time wave");
            StartCoroutine(WaveTime(0, length));
            isTime = true;
        }
        else //set up for if the progress bar goes off of kills
        {
            length = Random.Range(GetCurrentWave().requiredKills.x, GetCurrentWave().requiredKills.y);
            isTime = false;
        }


        progressbarIncrement = (1.0f / length) / (activeRound.Length - 1);
    }

    public Wave GetCurrentWave()
    {
        return activeWave;
    }

    private void MakeProgressBar()
    {
        int count = 0;
        
        float screenHeight = Screen.height;
        float yOffSet = screenHeight/5.5f;
        float split = (screenHeight - (yOffSet*2))/(activeRound.Length - 1);
        foreach (GameObject wave in activeRound)
        {
            GameObject newWave = Instantiate(wave, progressBar.transform);
            waves.Add(newWave.GetComponent<Wave>());
            RectTransform barRect = newWave.GetComponent<RectTransform>();
            barRect.position = new Vector3(barRect.position.x, (split * count) + yOffSet, barRect.position.z);
            count++;
        }

        activeWave = waves[0];
    }

    private void StageSwitch(int stage)
    {
        if(waves.Count <= stage)
        {
            return;
        }
        Wave wave = waves[stage];
        activeWave = wave;

        Debug.Log("nick12 req kills " + length + "active wave" + activeWave +  " divide by 0?"+ (activeRound.Length - 1));
        if (wave.buildPhase) {
            SwitchDefensePhase(true);
            Debug.Log("adam12 defense phase switch");
        }
        else if(wave.tag == "Horde")  {
            //spawnhorde
        }
        else //regular wave
        {
            
        }
        if (wave.finalWave)
        {
            OnFinalWave?.Invoke(gameObject, EventArgs.Empty);
            roundCount++;
            if (roundList.Length - 1 >= roundCount)
            {
                activeRound = GetWaveList(roundCount);
                progressFill.fillAmount = 0;
                fillAmount = 0;
                waveCount = 0;
                StopAllCoroutines();
                waves = new List<Wave>();
                MakeProgressBar();
                StageSwitch(0);
            }
            else
            {
                Debug.Log("ENDING LEVEL");
                endLevelParticles.SetActive(true);
                StartCoroutine(EndLevelParticlesLifetime());
            }
        }
        else
        {
            UpdateLength();
            enemiesKilled = 0;
            timePassed = 0;
        }
        OnNewWave?.Invoke(gameObject, EventArgs.Empty);
    }

    private IEnumerator EndLevelParticlesLifetime()
    {
        yield return new WaitForSeconds(4);
        endLevelParticles.SetActive(false);
    }


    public void SwitchDefensePhase(bool flashy)
    {
        Debug.Log("WAVE MANAGER DEFENSE PHASE START");
        attackPhase = false;
        defensePhase = true;
        if (flashy)
        {
            TriggerDefensePhaseEffect();
        }
        
        OnDefensePhaseStart?.Invoke(gameObject, EventArgs.Empty);
        StartCoroutine(DefensePhase());
    }

    private IEnumerator DefensePhase()
    {
        yield return new WaitForSeconds(4.2f);
        defensePhaseEffect.SetActive(false);
    }


    private void TriggerDefensePhaseEffect()
    {
        Debug.Log("defnce phase effect");
        Vector3 camPos = Camera.main.transform.position;
        Vector3 spawnPos = new Vector3(camPos.x, camPos.y, camPos.z - -.1f);
        defensePhaseEffect.transform.position = spawnPos;
        defensePhaseEffect.SetActive(!defensePhaseEffect.activeSelf);
    }

    public void EnemyDied(object sender, EventArgs e)
    {
        if(isTime) { return; }
        if(defensePhase) { return; }
        enemiesKilled++;
        FillProgress();
    }

    private void FillProgress()
    {
        if(progressFill.fillAmount == 1) { return; }
        Debug.Log("current wave " + activeWave + "req kills" + activeWave.requiredKills + "req kills for wave manager " + length + "progress bar increment" + progressbarIncrement);

        fillAmount += progressbarIncrement;

        // waveFlagHeight += waveFlagIncrement;
        progressFill.fillAmount = fillAmount;
        // waveFlag.transform.localPosition = new Vector3(waveFlag.transform.localPosition.x, waveFlagHeight, waveFlag.transform.localPosition.z);
        if (enemiesKilled >= length || timePassed >= length)
        {
            waveCount++;
            StageSwitch(waveCount);
        }
    }



    public void SwitchAttackPhase(bool flashy)
    {
        attackPhase = true;
        defensePhase = false;
        Debug.Log("AttackPhase begun");
        if(flashy)
        {
            TriggerAttackPhaseEffect();

            StartCoroutine(AttackPhase());
        }
        else
        {
            OnAttackPhaseStart?.Invoke(gameObject, EventArgs.Empty);
            OnNewWave?.Invoke(gameObject, EventArgs.Empty);
        }

    }

    private IEnumerator AttackPhase()
    {
        yield return new WaitForSeconds(4.2f);
        OnAttackPhaseStart?.Invoke(gameObject, EventArgs.Empty);
        OnNewWave?.Invoke(gameObject, EventArgs.Empty);
        attackPhaseEffect.SetActive(false);
    }

    private void TriggerAttackPhaseEffect()
    {
        Debug.Log("attackc phase effect");
        Vector3 camPos = Camera.main.transform.position;
        Vector3 spawnPos = new Vector3(camPos.x - 10, camPos.y, camPos.z - -.1f);
        attackPhaseEffect.transform.position = spawnPos;
        attackPhaseEffect.SetActive(!attackPhaseEffect.activeSelf);
    }

    private IEnumerator WaveTime(int localTime, int localLength)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("JOHN 12 wave second passed");
        if(!defensePhase) {
            timePassed++;
            localTime++;
            FillProgress();
        }
        if (localTime < localLength) {
            StartCoroutine(WaveTime(localTime, localLength));
        }
        else
        {
            Debug.Log("JOHN 12 its my time, folks");
        }

    }
    public bool GetIsAttackPhase()
    {
        return attackPhase;
    }
    public bool GetIsDefensePhase()
    {
        return defensePhase;
    }

    private void NotActive()
    {
        Debug.Log("ERROR! Tried to call a 'WaveManager' method while the script was inactive");
    }


}

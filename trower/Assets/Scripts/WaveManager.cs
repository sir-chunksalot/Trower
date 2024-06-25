using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [SerializeField] GameObject[] waveList;
    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject progresssFillObj;
    [SerializeField] GameObject waveFlag;
    [SerializeField] GameObject attackPhaseEffect;
    [SerializeField] GameObject defensePhaseEffect;
    [SerializeField] GameObject bar;

    private List<Wave> waves;
    private Wave activeWave;

    private HeroManager heroManager;

    public event EventHandler OnAttackPhaseStart;
    public event EventHandler OnDefensePhaseStart;
    public event EventHandler OnNewWave;

    private Image progressFill;

    private bool attackPhase;
    private bool defensePhase;

    private int waveCount;
    private int enemiesKilled;
    private float progressbarIncrement;
    private float waveFlagIncrement;
    private float fillAmount;
    private float waveFlagHeight;
    private int reqKills;
    private bool finalWave;
    private void Awake()
    {
        waves = new List<Wave>();
        MakeProgressBar();
        heroManager = gameObject.GetComponent<HeroManager>();
        heroManager.OnHeroDeath += EnemyDied;


        reqKills = Random.Range(GetCurrentWave().requiredKills.x, GetCurrentWave().requiredKills.y);

        progressbarIncrement = (1.0f / reqKills) / (waveList.Length - 1);
        float topOfProgressBar = 700;
        //waveFlagHeight = -350;
        //waveFlagIncrement = ((topOfProgressBar / reqKills) / (waveList.Length - 1));
        progressFill = progresssFillObj.GetComponent<Image>();
        Debug.Log("progressbar" + waveFlagIncrement + "top" + topOfProgressBar + "req" + (topOfProgressBar / reqKills) + " help" + 350 + "bro" + waveList[waveList.Length - 1]);
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
        float split = (screenHeight - (yOffSet*2))/(waveList.Length - 1);
        foreach (GameObject wave in waveList)
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
        Wave wave = waves[stage];

        activeWave = wave;
        reqKills = Random.Range(GetCurrentWave().requiredKills.x, GetCurrentWave().requiredKills.y);

        Debug.Log("adam12 req kills " + reqKills + "active wave" + activeWave);
        if (wave.buildPhase) {
            SwitchDefensePhase(true);
            Debug.Log("adam12 defense phase switch");
        }
        else if(wave.tag == "Horde")  {
            //spawnhorde
        }
        else if(wave.tag == "End") {
            //end
        }
        else //regular wave
        {
            OnNewWave?.Invoke(gameObject, EventArgs.Empty);
        }

        if (stage + 2 >= waves.Count) {
            finalWave = true;
        }

        
    }

    public void SwitchDefensePhase(bool flashy)
    {
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
        Debug.Log("current wave " + activeWave + "req kills" + activeWave.requiredKills + "req kills for wave manager " + reqKills);
        enemiesKilled++;
        fillAmount += progressbarIncrement;
       // waveFlagHeight += waveFlagIncrement;
        progressFill.fillAmount = fillAmount;
       // waveFlag.transform.localPosition = new Vector3(waveFlag.transform.localPosition.x, waveFlagHeight, waveFlag.transform.localPosition.z);
        if (enemiesKilled >= reqKills)
        {
            if(!finalWave)
            {
                waveCount++;
                progressbarIncrement = (1 / reqKills) / (waveList.Length - 1);
               // waveFlagIncrement = (700 / reqKills) / (waveList.Length - 1);
                enemiesKilled = 0;
                StageSwitch(waveCount);
            }
            else
            {
                //finish level
            }

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

    public bool GetIsAttackPhase()
    {
        return attackPhase;
    }
    public bool GetIsDefensePhase()
    {
        return defensePhase;
    }


}

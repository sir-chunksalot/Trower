using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [SerializeField] GameObject[] waveList;
    [SerializeField] int[] requiredKills;
    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject progresssFillObj;
    [SerializeField] GameObject waveFlag;
    [SerializeField] GameObject attackPhaseEffect;
    [SerializeField] GameObject defensePhaseEffect;
    [SerializeField] GameObject bar;

    private HeroManager heroManager;

    public event EventHandler OnAttackPhaseStart;
    public event EventHandler OnDefensePhaseStart;

    private Image progressFill;

    private bool attackPhase;
    private bool defensePhase;

    private int waveCount;
    private int enemiesKilled;
    private float progressbarIncrement;
    private float waveFlagIncrement;
    private float fillAmount;
    private float waveFlagHeight;
    private void Awake()
    {
        MakeProgressBar();
        heroManager = gameObject.GetComponent<HeroManager>();
        heroManager.OnHeroDeath += EnemyDied;

        if (requiredKills.Length < waveList.Length - 1)
        {
            int[] reqKills = new int[waveList.Length - 1];
            int count = 0;
            foreach(int kills in requiredKills)
            {
                reqKills[count] = kills;
                count++;
            }
            int extra = (waveList.Length - 1) - requiredKills.Length;
            for(int i = 0; i < extra; i++)
            {
                reqKills[count] = 10;
                count++;
            }
            requiredKills = reqKills;
        }
        Debug.Log("req kills" + requiredKills.Length);

        progressbarIncrement = (1.0f / requiredKills[0]) / (waveList.Length - 1);
        float topOfProgressBar = 700;
        waveFlagHeight = -350;
        waveFlagIncrement = ((topOfProgressBar / requiredKills[0]) / (waveList.Length - 1));
        progressFill = progresssFillObj.GetComponent<Image>();
        Debug.Log("progressbar" + waveFlagIncrement + "top" + topOfProgressBar + "req" + (topOfProgressBar / requiredKills[0]) + " help" + 350 + "bro" + waveList[waveList.Length - 1]);
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
            RectTransform barRect = newWave.GetComponent<RectTransform>();
            barRect.position = new Vector3(barRect.position.x, (split * count) + yOffSet, barRect.position.z);
            count++;
        }
    }

    private void StageSwitch(int stage)
    {
        GameObject wave = waveList[stage];
        if (wave.tag == "BuildPhase")
        {
            SwitchDefensePhase(true);
        }
        else if(wave.tag == "Horde")
        {
            //spawnhorde
        }
        else if(wave.tag == "End")
        {
            //end
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
        enemiesKilled++;
        fillAmount += progressbarIncrement;
        waveFlagHeight += waveFlagIncrement;
        progressFill.fillAmount = fillAmount;
        waveFlag.transform.localPosition = new Vector3(waveFlag.transform.localPosition.x, waveFlagHeight, waveFlag.transform.localPosition.z);
        if (enemiesKilled >= requiredKills[waveCount])
        {
            if(waveCount < waveList.Length -2)
            {
                waveCount++;
                progressbarIncrement = (1 / requiredKills[waveCount]) / (waveList.Length - 1);
                waveFlagIncrement = (700 / requiredKills[waveCount]) / (waveList.Length - 1);
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
        }

    }

    private IEnumerator AttackPhase()
    {
        yield return new WaitForSeconds(4.2f);
        OnAttackPhaseStart?.Invoke(gameObject, EventArgs.Empty);
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

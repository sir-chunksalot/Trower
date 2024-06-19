using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] GameObject[] waveList;
    [SerializeField] GameObject progressBar;
    [SerializeField] GameObject AttackPhaseEffect;
    [SerializeField] GameObject bar;

    public event EventHandler OnAttackPhaseStart;
    public event EventHandler OnDefensePhaseStart;

    private bool attackPhase;
    private void Awake()
    {
        MakeProgressBar();
    }

    private void MakeProgressBar()
    {
        int count = 0;
        float xOffSet = 0;
        foreach (GameObject wave in waveList)
        {
            if (count >= 1)
            {

                GameObject newBar = Instantiate(bar, progressBar.transform);
                newBar.transform.SetAsFirstSibling();
                RectTransform barRect = newBar.GetComponent<RectTransform>();
                barRect.position = new Vector3(barRect.position.x + xOffSet + 32 - 60, barRect.position.y, barRect.position.z);

            }
            GameObject newWave = Instantiate(wave, progressBar.transform);
            RectTransform newTransform = newWave.GetComponent<RectTransform>();
            newTransform.position = new Vector3(newTransform.position.x + xOffSet, newTransform.position.y, newTransform.position.z);
            count++;
            xOffSet += 60;
        }
    }

    public void SwitchDefensePhase(bool flashy)
    {
        attackPhase = false;
        if (flashy)
        {
            TriggerDefensePhaseEffect();
        }
        
        OnDefensePhaseStart?.Invoke(gameObject, EventArgs.Empty);

    }


    private void TriggerDefensePhaseEffect()
    {
        //add defense phase effect
    }

    //
    //
    //
    //
    //
    //attack phase
    //
    //
    //
    //

    public void SwitchAttackPhase(bool flashy)
    {
        attackPhase = true;
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
    }

    private void TriggerAttackPhaseEffect()
    {
        Debug.Log("attackc phase effect");
        Vector3 camPos = Camera.main.transform.position;
        Vector3 spawnPos = new Vector3(camPos.x - 10, camPos.y, camPos.z - -.1f);
        AttackPhaseEffect.transform.position = spawnPos;
        AttackPhaseEffect.SetActive(!AttackPhaseEffect.activeSelf);
    }


}

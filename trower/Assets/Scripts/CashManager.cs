using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    WaveManager waveManager;
    private void Awake()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        Debug.Log("adam 12" + waveManager);
        waveManager.OnNewWave += DefensePhase;
        
    }

    public void DefensePhase(object sender, EventArgs e)
    {
        if (waveManager.GetCurrentWave() == null) { return; }
        if (waveManager.GetIsAttackPhase() && waveManager.GetCurrentWave().buildPhase) { //this means that this wave already gave out cash, no reason to do it again
            return;
        }
        Coins.ChangeCoins(waveManager.GetCurrentWave().earnedCoins);
    }
}

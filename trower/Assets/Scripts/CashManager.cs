using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    WaveManager waveManager;
    Wave currentWave;
    private void Awake()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        Debug.Log("adam 12" + waveManager);
        GameManager gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneChange += OnSceneLoad;
        waveManager.OnNewWave += GainCoins;
    }


    public void OnSceneLoad(object sender, EventArgs e)
    {
        Coins.SetCoin(0);
    }
    public void GainCoins(object sender, EventArgs e)
    {
        Wave wave = waveManager.GetCurrentWave();
        if (wave != null)
        {
            if(currentWave != wave)
            {
                Coins.ChangeCoins(wave.earnedCoins);
                currentWave = wave;
            }

        }
    }
}

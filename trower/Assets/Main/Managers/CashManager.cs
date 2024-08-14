using System;
using UnityEngine;

public class CashManager : MonoBehaviour
{
    WaveManager waveManager;
    Wave currentWave;

    int cashSpent;
    int cashEarned;

    private void Awake()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        Debug.Log("adam 12" + waveManager);
        GameManager gameManager = gameObject.GetComponent<GameManager>();
        gameManager.OnSceneLoaded += OnSceneLoad;
        waveManager.OnNewWave += GainCoins;
        Coins.onChangeCoin += CoinTracker;
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
            if (currentWave != wave)
            {
                Coins.ChangeCoins(wave.earnedCoins);
                currentWave = wave;
            }

        }
    }

    public void CoinTracker(object sender, EventArgs e)
    {
        string coinChange = (string)sender;
        if(coinChange.ToCharArray()[0] == '+')
        {
            int newCash = 0;
            Int32.TryParse(coinChange.Substring(1, coinChange.Length - 2), out newCash);

            cashEarned += newCash;
        }
        else if(coinChange.ToCharArray()[0] == '-')
        {
            int newCash = 0;
            Int32.TryParse(coinChange.Substring(1, coinChange.Length - 2), out newCash);

            cashSpent -= newCash;
        }

        Debug.Log("its what you do to me" + cashEarned + "casdfaf" + cashSpent);
    }


    public int GetCashSpent()
    {
        return cashSpent;
    }


    public int GetCashEarned()
    {
        return cashEarned;
    }
}

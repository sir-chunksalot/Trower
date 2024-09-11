using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : MonoBehaviour
{
    [SerializeField] Wave wave;
    WaveManager waveManager;
    private void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        waveManager = gameManager.GetComponent<WaveManager>();
    }

    public void ChangeActiveWave(string enemyType)
    {
        switch(enemyType)
        {
            case "PrisonGuard":
                wave.prisonGuard.SetCanSpawn(!wave.prisonGuard.GetCanSpawn());
                break;
        }

        waveManager.ManuallySetWave(wave);    
    }
}

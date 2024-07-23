using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnSceneChange;

    LevelLoader levelLoader;
    LevelDetails levelDetails;

    //--MANAGER SCRIPTS--//
    private TrapBuilder trapBuilder;
    private UIManager uiManager;
    private HeroManager heroManager;
    private WaveManager waveManager;
    private CashManager cashManager;

    private bool first;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManager").Length == 1)
        {
            first = true; //only called by the OG game manager
            UnlockItem("T_Spears");
            UnlockItem("F_BasicFloor"); //base unlocks
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        levelDetails = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelDetails>();
        trapBuilder = gameObject.GetComponent<TrapBuilder>();
        uiManager = gameObject.GetComponent<UIManager>();
        heroManager = gameObject.GetComponent<HeroManager>();
        waveManager = gameObject.GetComponent<WaveManager>();
        cashManager = gameObject.GetComponent<CashManager>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DontDestroyOnLoad(this.gameObject);

        levelDetails = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelDetails>();
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        Debug.Log("LevelDetails" +levelDetails);

        OnSceneChange?.Invoke(gameObject, EventArgs.Empty);

    }

    public void UnlockItem(string item)
    {
        Debug.Log("Tried to unlock!");
        UnlockManager.Unlock(item);
    }

    public LevelDetails GetCurrentLevelDetails()
    {
        return levelDetails;
    }

    public GameObject GetTrapDaddy()
    {
        return levelDetails.trapDaddy;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        levelLoader.SetCurrentLevel(0);
        levelLoader.SetCurrentWorld(0);
        levelLoader.ChangeLevel(0, 0);
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}


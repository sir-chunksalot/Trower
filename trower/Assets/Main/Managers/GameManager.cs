using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnSceneLoaded;
    public event EventHandler OnSceneUnloaded;

    LevelLoader levelLoader;
    LevelDetails levelDetails;

    //--MANAGER SCRIPTS--//
    private TrapBuilder trapBuilder;
    private UIManager uiManager;
    private HeroManager heroManager;
    private WaveManager waveManager;
    private CashManager cashManager;
    private GridManager gridManager;

    Statsheet statsheet;

    private bool first;
    private bool endScene;

    //STAT SHEET

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnNewScene;
        SceneManager.sceneUnloaded += OnLeaveScene;
    }

    private void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManager").Length == 1)
        {
            first = true; //only called by the OG game manager
            UnlockItem("T_Spears");
            UnlockItem("F_BasicFloor"); //base unlocks
            UnlockItem("F_LFloor"); //base unlocks
            UnlockItem("F_TFloor"); //base unlocks
            UnlockItem("T_Crossbow"); //base unlocks
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
        gridManager = gameObject.GetComponent<GridManager>();
    }

    void OnNewScene(Scene scene, LoadSceneMode mode)
    {
        DontDestroyOnLoad(this.gameObject);

        levelDetails = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelDetails>();
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        Debug.Log("LevelDetails" + levelDetails);


        if(levelDetails.isEndScene)
        {
            EndManager endManager = GameObject.FindGameObjectWithTag("EndManager").GetComponent<EndManager>();
            endManager.DisplayStats(statsheet);
        }

        OnSceneLoaded?.Invoke(gameObject, EventArgs.Empty);
    }
    void OnLeaveScene(Scene scene)
    {
        OnSceneUnloaded?.Invoke(gameObject, EventArgs.Empty);
    }

    public void UnlockItem(string item)
    {
        Debug.Log("Tried to unlock!");
        UnlockManager.Unlock(item);
    }

    public void EndLevel()
    {
        statsheet = new Statsheet();
        statsheet.kills = heroManager.GetKillCount();
        statsheet.cashSpent = cashManager.GetCashSpent();

        levelLoader.ChangeScene("EndLevel");

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
        levelLoader.ChangeLevel(0, 0);
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnNewScene;
    }

    private void Update()
    {
        if(gridManager.GetIsGridUpdated())
        {
            if (!levelDetails.fixedCamera)
            {
                Vector2 center = gridManager.GetCenterOfGrid();
                GetCurrentLevelDetails().sceneCamera.transform.position = new Vector3(center.x, center.y, -20);
            }
            gridManager.SetIsGridUpdated(false);
        }
    }

}


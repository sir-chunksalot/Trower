using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    LevelLoader levelLoader;

    private void Awake()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
    }
    public void UnlockItem(string item)
    {
        
        UnlockManager.Unlock(item);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;

        levelLoader.SetCurrentLevel(0);
        levelLoader.SetCurrentWorld(0);
        levelLoader.ChangeLevel(0, 0);
    }

}


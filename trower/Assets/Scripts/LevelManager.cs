using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int levelCount;
    void Start()
    {
        levelCount = 1;
    }

    public void ChangeLevel()
    {
        SceneManager.LoadScene(levelCount - 1);
    }
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{

    //[SerializeField] string homeLevel;

    //private void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}
    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("vision" + LevelManager.GetCurrentWorld() + "-" + LevelManager.GetCurrentLevel());
    //    if(homeLevel != LevelManager.GetCurrentWorld() + "-" + LevelManager.GetCurrentLevel())
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    DontDestroyOnLoad(this.gameObject);
    //}

    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

}

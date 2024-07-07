using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Animator transistion;
    private int currentLevel;
    private int currentWorld;

    private void Awake()
    {
        currentWorld = 1;
        currentLevel = 1;
        int index = SceneManager.GetActiveScene().buildIndex;
        for(int i = 0; i < index; i += 10)
        {
            if (i + 10 < index)
            {
                currentWorld++;
            }
            else
            {
                currentLevel = i - ((currentWorld - 1) * 10);
            }
        }
    }

    public int GetCurrentWorld()
    {
        return currentWorld;
    }

    public void SetCurrentWorld(int world)
    {
        currentWorld = world;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }

    public void ChangeLevel(int world, int level)
    {
        int sceneIndex = level * world;
        StartCoroutine(CrossFade(sceneIndex));
    }

    public void PlayMinigame(string game)
    {
        StartCoroutine(CrossFade(game));
    }

    private IEnumerator CrossFade(int sceneIndex)
    {
        transistion.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneIndex);
    }

    private IEnumerator CrossFade(string sceneName)
    {
        transistion.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneName);
    }
}

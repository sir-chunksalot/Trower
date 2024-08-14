using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Animator transistion;
    private int currentLevel;
    private int currentWorld;

    private void Awake()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        for (int i = 0; i < index; i += 10)
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

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void ChangeLevel(int world, int level)
    {
        int sceneIndex = level * world;
        currentLevel = level;
        currentWorld = world;
        StartCoroutine(CrossFade(sceneIndex));
    }

    public void ChangeScene(string sceneName)
    {
        currentLevel = 99;
        currentWorld = 99;
        StartCoroutine(CrossFade(sceneName));
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

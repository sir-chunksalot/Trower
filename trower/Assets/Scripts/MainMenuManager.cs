using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame(int level)
    {
        SceneManager.LoadScene(level) ;
    }

    public void LevelList()
    {

    }
}

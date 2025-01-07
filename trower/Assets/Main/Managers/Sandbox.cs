using UnityEngine;

public class Sandbox : MonoBehaviour
{
    [SerializeField] Wave wave;
    GameObject gameManager;
    WaveManager waveManager;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        waveManager = gameManager.GetComponent<WaveManager>();
    }

    public void ChangeActiveWave(string enemyType)
    {
        switch (enemyType)
        {
            case "PrisonGuard":
                wave.prisonGuard.SetCanSpawn(!wave.prisonGuard.GetCanSpawn());
                break;
        }

        waveManager.ManuallySetWave(wave);
    }

    public void SpawnEnemy(string enemyType)
    {
        HeroManager heroManager = gameManager.GetComponent<HeroManager>();
        GameObject hero = heroManager.GetHero(enemyType);
        heroManager.SpawnHero(hero, 2);
    }
}

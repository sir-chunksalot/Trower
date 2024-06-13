using UnityEngine;

public class HeroSpawner : MonoBehaviour
{
    //[SerializeField] private float startSpawnRate;
    //[SerializeField] private float spawnRateVariance;
    //[SerializeField] private float gracePeriod;
    //[SerializeField] private float increaseRate;
    //[SerializeField] GameObject[] heroes;
    //[SerializeField] GameObject[] heroSpawnList2;
    //[SerializeField] GameObject progressBarObject;
    //[SerializeField] GameObject particle;

    //private float spawnRate;

    //public event EventHandler OnHeroDeath;

    //ProgressBar progressBar;
    ////Spawner spawner;


    //void Start()
    //{
    //    spawnRate = startSpawnRate;
    //    //spawner = gameObject.GetComponent<Spawner>();

    //    progressBar = progressBarObject.GetComponent<ProgressBar>();
    //    //progressBar.OnWaveStart += SpawnWave;

    //    //StartCoroutine(GracePeriod());

    //}

    //public List<GameObject> WaveGenerate(int enemyCountLowerbound, int enemyCountUpperbound, float waveDifficulty)
    //{
    //    int enemyCount = UnityEngine.Random.Range(enemyCountLowerbound, enemyCountUpperbound);
    //    List<GameObject> validSpawns = new List<GameObject>();
    //    foreach (GameObject hero in heroes) //seperates all the valid heroes from the invalids
    //    {
    //        if (hero.GetComponent<Hero>().GetDifficulty() >= waveDifficulty)
    //        {
    //            validSpawns.Add(hero);
    //        }
    //    }
    //    List<GameObject> heroPool = new List<GameObject>();

    //    foreach (GameObject hero in validSpawns) //adds the valid heroes to a spawn pool, weighting it more favorably towards those with a higher spawn weight
    //    {
    //        float spawnWeight = hero.GetComponent<Hero>().GetSpawnWeight();
    //        for (int i = 0; i < spawnWeight; i++)
    //        {
    //            heroPool.Add(hero);
    //        }
    //    }

    //    int heroPoolSize = heroPool.Count;
    //    List<GameObject> spawnList = new List<GameObject>();
    //    for (int i = 0; i < enemyCount; i++) //the final hero spawn list is made with a random supply of the hero pool
    //    {
    //        int chosenHero = UnityEngine.Random.Range(0, heroPoolSize);
    //        spawnList.Add(heroPool[chosenHero]);
    //    }

    //    return spawnList;

    //}

    //public void SpawnWave(List<GameObject> spawnList)
    //{

    //}

    //public void HeroDied(GameObject hero)
    //{
    //    OnHeroDeath?.Invoke(gameObject, EventArgs.Empty);
    //}
    //private IEnumerator SpawnHero(GameObject[] heroes, int amount, float reduceSpawnRatePercentage = 1)
    //{
    //    if (amount > 0)
    //    {
    //        yield return new WaitForSeconds(UnityEngine.Random.Range(spawnRate - spawnRateVariance, spawnRate + spawnRateVariance) * reduceSpawnRatePercentage);
    //        spawner.SpawnCreature(heroes[UnityEngine.Random.Range(0, heroes.Length)], true);
    //        amount--;
    //        StartCoroutine(SpawnHero(heroes, amount, reduceSpawnRatePercentage));
    //    }
    //}
    //private IEnumerator GracePeriod()
    //{
    //    yield return new WaitForSeconds(gracePeriod);
    //    StartCoroutine(SpawnHero(heroSpawnList1, 1000));
    //    StartCoroutine(IncreaseSpawnRateOverTime());
    //}
    //public void SpawnWave(object obj, EventArgs e)
    //{
    //    StartCoroutine(SpawnHero(heroSpawnList1, 10, .3f));
    //    StartCoroutine(SpawnHero(heroSpawnList2, 10, .3f));

    //    StartCoroutine(SpawnHero(heroSpawnList2, 1000));
    //}
    //private IEnumerator IncreaseSpawnRateOverTime()
    //{
    //    yield return new WaitForSeconds(10);
    //    spawnRate = spawnRate * increaseRate;
    //    spawnRate = spawnRate * increaseRate;
    //    spawnRateVariance = spawnRateVariance * increaseRate;
    //    StartCoroutine(IncreaseSpawnRateOverTime());
    //}
}

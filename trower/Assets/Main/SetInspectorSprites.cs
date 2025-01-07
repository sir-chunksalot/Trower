using UnityEngine;

public class SetInspectorSprites : MonoBehaviour
{
    HeroManager heroManager;
#if(UNITY_EDITOR) 
    [ContextMenu("SetInspectorSprites")]
    void Run()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        heroManager = gameManager.GetComponent<HeroManager>();

        GameObject[] wavesObj = GameObject.FindGameObjectsWithTag("Wave");
        Wave[] waves = new Wave[wavesObj.Length];
        int count = 0;
        foreach (GameObject wave in wavesObj)
        {
            waves[count] = wave.GetComponent<Wave>();
            count++;
        }

        SetWaveHeroSprites(heroManager.GetAllHeroes(), waves);
    }
    private void SetWaveHeroSprites(GameObject[] heroes, Wave[] waves)
    {
        foreach (Wave wave in waves)
        {
            foreach (GameObject hero in heroes)
            {
                wave.SetHeroSprite(hero.tag, hero.GetComponent<SpriteRenderer>().sprite);
            }
        }

    }
#endif
}

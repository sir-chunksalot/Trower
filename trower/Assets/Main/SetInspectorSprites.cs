using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInspectorSprites : MonoBehaviour
{
    HeroManager heroManager;

    private void Run()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        heroManager = gameManager.GetComponent<HeroManager>();

        Wave[] waves = GameObject.FindGameObjectsWithTag.
    }
    private void SetWaveHeroSprites(GameObject[] heroes, Wave[] waves)
    {
        foreach(Wave wave in waves)
        {
            foreach (GameObject hero in heroes)
            {
                wave.SetHeroSprite(hero.tag, hero.GetComponent<SpriteRenderer>().sprite);
            }
        }
        
    }
}

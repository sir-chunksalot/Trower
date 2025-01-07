using System;
using System.Collections;
using UnityEngine;

public class DoorEntry : MonoBehaviour
{
    [SerializeField] GameObject bigSpacebar;
    [SerializeField] GameObject doorLeft;
    [SerializeField] GameObject doorRight;
    [SerializeField] Animator doorLeftAnim;
    [SerializeField] Animator doorRightAnim;
    [SerializeField] SpriteRenderer spacebarLeft;
    [SerializeField] SpriteRenderer spacebarRight;
    GameManager gameManager;
    WaveManager waveManager;
    TrapManager trapManager;
    private bool heroesCanEnter;
    private bool endLevel;

    private void Start()
    {
        gameManager = gameObject.GetComponentInParent<GameManager>();
        waveManager = gameObject.GetComponentInParent<WaveManager>();
        trapManager = gameObject.GetComponentInParent<TrapManager>();
        trapManager.onSpacePressed += EndLevel;
        gameManager.OnSceneLoaded += OnSceneLoad;
        waveManager.OnFinalWave += SuckySucky;
    }

    private void OnSceneLoad(object sender, EventArgs e)
    {
        endLevel = false;
        AllowHeroEntry(gameManager.GetCurrentLevelDetails().enemiesCanWin);
    }

    public void AllowHeroEntry(bool value)
    {
        heroesCanEnter = value;
    }


    public void EndLevel(object sender, EventArgs e)
    {
        if (endLevel)
        {
            gameManager.EndLevel();
        }
    }

    private void SuckySucky(object sender, EventArgs e)
    {
        doorLeftAnim.SetTrigger("EndLevel");
        doorRightAnim.SetTrigger("EndLevel");
        doorLeft.SetActive(false);
        doorRight.SetActive(false);
        spacebarLeft.enabled = false;
        spacebarRight.enabled = false;

        StartCoroutine(EndLevelCoroutine());
    }

    private IEnumerator EndLevelCoroutine()
    {
        yield return new WaitForSeconds(1);
        endLevel = true;
        bigSpacebar.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == 6)
        {
            if (endLevel)
            {
                Hero hero = collision.gameObject.GetComponent<Hero>();
                if (hero != null)
                {
                    hero.KillMe();
                }

            }
            else if (heroesCanEnter)
            {
                //fail level
            }
        }
    }
}

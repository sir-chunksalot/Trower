using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gameManagerScript;
    LevelDetails level;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject coinTextBox;
    [SerializeField] GameObject devMenu;
    [SerializeField] GameObject dialogueManager;
    [SerializeField] GameObject continueHolder;
    [SerializeField] GameObject buildTabHolder;
    [SerializeField] GameObject coinCountHolder;
    [SerializeField] GameObject progressBarHolder;
    [SerializeField] GameObject buildTabObj;
    [SerializeField] GameObject towerTab;
    [SerializeField] GameObject cooldownTimer;
    [SerializeField] Sprite chainedBuildTab;
    [SerializeField] Sprite buildTab;
    [SerializeField] AudioSource denySound;

    //tabs
    [SerializeField] GameObject buildCards;
    [SerializeField] GameObject trapCards;
    RectTransform buildCardsTransform;
    RectTransform trapCardsTransform;
    List<GameObject> cards;
    RectTransform cooldownTimerTransform;
    TrapManager trapManager;
    WaveManager waveManager;
    Image buildImage;
    TMP_Text resourceText;
    TMP_Text coinText;

    public event EventHandler OnClickContinue;

    private bool denySwitch; //used for tutorials

    private void Awake()
    {
        gameManagerScript = gameObject.GetComponent<GameManager>();
        gameManagerScript.OnSceneChange += OnSceneLoad;
        trapManager = this.gameObject.GetComponent<TrapManager>();
        waveManager = gameObject.GetComponent<WaveManager>();
        waveManager.OnDefensePhaseStart += DefensePhase;
        waveManager.OnAttackPhaseStart += AttackPhase;
        Debug.Log("WAVE MANAGER ASSIGNED TO UI MANAGER");
        trapManager.onSelectedTrapChange += SelectedTrapChange;
        GameObject cooldownTimer = trapManager.GetCooldownTimer();
        if(cooldownTimer != null) {
            cooldownTimerTransform = cooldownTimer.GetComponent<RectTransform>();
        }
        if(buildCards != null) {
            buildCardsTransform = buildCards.GetComponent<RectTransform>();
        }
        if(trapCards != null) {
            trapCardsTransform = trapCards.GetComponent<RectTransform>();
        } 
        if(buildTabObj != null) {
            buildImage = buildTabObj.GetComponent<Image>();
        }
        if(coinTextBox != null)
        {
            Coins.onChangeCoin += GainCoin;
            UnlockManager.OnUnlock += OnUnlock;
            coinText = coinTextBox.GetComponent<TMP_Text>();
            GainCoin(gameObject, EventArgs.Empty);
        }

    }

    private void OnSceneLoad(object sender, EventArgs e)
    {
        Setup();
    }

    private void Setup()
    {
        level = gameManagerScript.GetCurrentLevelDetails();
        if(level.hideBuildMenu)
        {
            buildTabHolder.SetActive(false);
        }
        else
        {
            buildTabHolder.SetActive(true);
        }
        if(level.hideCoinCount)
        {
            coinCountHolder.SetActive(false);
        }
        else
        {
            coinCountHolder.SetActive(true);
        }
        if(level.hideProgressbar)
        {
            progressBarHolder.SetActive(false);
        }
        else
        {
            progressBarHolder.SetActive(true);
        }

        Debug.Log("doofen counts the cards: " + cards.Count);

        foreach(GameObject card in cards)
        {
            card.GetComponent<CardHolsterGraphics>().SetCharge(0);
        }

    }
    public void EnableCooldownTimer(bool activeStatus)
    {
        Debug.Log("Cooldown timer is set to :" + activeStatus);
        cooldownTimer.GetComponent<Image>().enabled = activeStatus;
    }

    public GameObject GetCard(int index)
    {
            int count = 0;
            foreach (Transform child in trapCards.transform)
            {
                if(count == index)
                {
                    return child.gameObject;
                }
                count++;
            }
        Debug.Log("ERROR! UIManager attempted to get the index of a non existent card.");
        return null;
    }


    public void OnUnlock(object unlockName, EventArgs e)
    {
        UnlockCard((string)unlockName);
    }
    public void UnlockCard(string unlockName)
    {
        foreach (GameObject card in cards)
        {
            if (unlockName == card.name)
            {
                Debug.Log("unlocked card");
                card.GetComponent<CardHolsterGraphics>().UnlockCard();
                return;
            }
        }
    }


    private void AttackPhase(object sender, EventArgs e)
    {
        Debug.Log("UI attack phase");
        if(buildCards != null)
        {
            MoveTabs(true);
            continueHolder.SetActive(false);
            buildImage.sprite = chainedBuildTab;
        }
        if(trapCards != null)
        {
            RegenUIElements();
            foreach (GameObject card in cards)
            {
                card.GetComponent<CardHolsterGraphics>().AttackPhase();
            }
        }
        
    }
    private void DefensePhase(object sender, EventArgs e)
    {
        Debug.Log("UI MANAGER TRIED TO START DEFENSE PHASE WAVE MANANGER");
        continueHolder.SetActive(true);
        if (buildTabObj == null) { return; }
        Debug.Log("UI defense phase WAVE MANAGER");
        buildImage.sprite = buildTab;
        foreach (GameObject card in cards)
        {
            card.GetComponent<CardHolsterGraphics>().DefensePhase();
        }
    }

    private void MoveTabs(bool forceTrap)
    {
        if (trapCardsTransform.localPosition.y < 0 || forceTrap) //build is out of view
        {
            trapCardsTransform.localPosition = new Vector3(0, 0, 0);
            buildCardsTransform.localPosition = new Vector3(0, -500, 0);
        }
        else
        {
            trapCardsTransform.localPosition = new Vector3(0, -500, 0);
            buildCardsTransform.localPosition = new Vector3(0, 0, 0);

        }
    }

    public void SwitchTabs()
    {
        if (buildImage.sprite == buildTab)
        {
            MoveTabs(false);
        }
        else
        {
            denySound.Play();
        }

    }

    public void UseCard(string name, bool success) //this is an intermediary method so that when a trap is placed the charge goes down for the trap card 
    {
        string lastNum = name.Substring(name.Length - 1);
        if (lastNum == "1" || lastNum == "2" || lastNum == "3") { //converts rotation names to the base of 0
            name = name.Substring(0, name.Length - 1) + "0";
        }

        foreach (GameObject card in cards)
        {
            Debug.Log("zoro" + card.name + " " + name);
            if(card.name == name)
            {
                CardHolsterGraphics cardHolster = card.GetComponent<CardHolsterGraphics>();
                    Debug.Log("TRAP FOUND AND IS BEING USED");
                    cardHolster.UseCard(success);
                break;
            }
        }
    }

    public void AddCardToList(GameObject card) //called by card holster graphics, a script located in every card
    {
        if(cards == null) { cards = new List<GameObject>(); }
        cards.Add(card);
    }

    public void RegenUIElements()
    {
        MoveCooldownTimer();
    }
    
    public GameObject GetTowerTab() //used by stupid level1-1 scripyt
    {
        return towerTab;
    }

    public void SelectedTrapChange(object sender, EventArgs e)
    {
        MoveCooldownTimer();
    }

    public GameObject GetDialogueManager()
    {
        return dialogueManager;
    }

    private void MoveCooldownTimer()
    {
        if (cooldownTimerTransform == null) { return; }

        GameObject trap = trapManager.GetSelectedTrap();
        Vector3 spawnPos = new Vector3(10000, 10000);
        if (trap != null)
        {
            spawnPos = trap.transform.position;
            spawnPos = new Vector2(spawnPos.x, spawnPos.y + 3);
        }
        cooldownTimerTransform.transform.position = Camera.main.WorldToScreenPoint(spawnPos);
    }

    public void SetDenySwitch(bool value) //used by tutorial sections 
    {
        denySwitch = value;
    }

    public bool GetDenySwitch()
    {
        return denySwitch;
    }

    public void ContinueToAttackPhase()
    {
        OnClickContinue?.Invoke(gameObject, EventArgs.Empty);
        if(!denySwitch)
        {
            waveManager.SwitchAttackPhase(true);
            continueHolder.SetActive(false);
        }
    }


    public void GainCoin(object coin, EventArgs e)
    {
        coinText.text = Coins.GetCoins().ToString();
    }

    public void SecretDevTools()
    {
        if (devMenu.activeSelf)
        {
            devMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            devMenu.SetActive(true);
            pauseMenu.SetActive(false);
        }
    }

    public void PauseGame()
    {
        Debug.Log("tried to pause");
        if (pauseMenu.activeSelf || devMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            devMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseMenu.SetActive(true);
            devMenu.SetActive(false);
            Time.timeScale = 0;
        }
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PauseGame();
        }
    }

    //public void CoinChanged(object sender, EventArgs e) //do not directly call this, call coin manager script instead
    //{
    //    ChangeCoin(Coins.GetCoins());
    //}
    //public void ResourceChanged(object sender, EventArgs e)
    //{
    //    ChangeResource(BuildingResources.GetResources());
    //}


    //public void ChangeResource(float resourceCount)
    //{
    //    resourceText.text = resourceCount.ToString();
    //}
    //public void ChangeCoin(float coinCount) //do not directly call this, call coin manager script instead
    //{
    //    coinText.text = coinCount.ToString();
    //}


    //[SerializeField] GameObject bloodChangeTextBox;
    //bloodChangeText = bloodChangeTextBox.GetComponent<TMP_Text>();
    //BloodLust.onChangeBloodLust += ShowBloodChange;

    //all of this is saved in case i want to add that little ui popup thing again 

    //public void ShowBloodChange(object sender, EventArgs e)
    //{
    //    if (this.isActiveAndEnabled)
    //    {
    //        string[] text = sender.ToString().Split('|');
    //        bloodChangeText.text = text[0];
    //        if (Mathf.Sign((float.Parse(text[0]))) == 1)
    //        {
    //            Color color = Color.green;
    //            color.a = 0;
    //            bloodChangeText.color = color;
    //        }
    //        else
    //        {
    //            Color color = Color.red;
    //            color.a = 0;
    //            bloodChangeText.color = color;
    //        }
    //        Vector3 spawnPos = new Vector3(bloodChangePos.x, UnityEngine.Random.Range(bloodChangePos.y - 1, bloodChangePos.y + 1), bloodChangePos.z);
    //        GameObject newTextBox = Instantiate(bloodChangeTextBox, spawnPos, Quaternion.identity);
    //        newTextBox.GetComponent<TMP_Text>().text = bloodChangeText.text;
    //        newTextBox.GetComponent<TMP_Text>().color = bloodChangeText.color;
    //        newTextBox.SetActive(true);
    //        StartCoroutine(BloodChangeFade(newTextBox.GetComponent<TMP_Text>(), 0, .02f));

    //    }

    //}

    //private void OnDestroy()
    //{
    //    BloodLust.onChangeBloodLust -= ShowBloodChange;
    //}

    //private IEnumerator BloodChangeFade(TMP_Text text, float count, float changeRate)
    //{
    //    Color color = text.color;
    //    color.a += changeRate;
    //    text.color = color;
    //    yield return new WaitForSeconds(.007f);
    //    count += 1;
    //    if (count < 100)
    //    {
    //        StartCoroutine(BloodChangeFade(text, count, changeRate));
    //    }
    //    else
    //    {
    //        StartCoroutine(BloodChangeLife(text, 0, -.02f));
    //    }
    //}
    //private IEnumerator BloodChangeLife(TMP_Text text, float count, float changeRate)
    //{
    //    yield return new WaitForSeconds(.4f);
    //    StartCoroutine(BloodChangeFade(text, count, changeRate));
    //}

}

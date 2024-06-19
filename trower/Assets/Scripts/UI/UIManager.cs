using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject devMenu;
    [SerializeField] GameObject cooldownTimer;
    [SerializeField] GameObject dialogueManager;

    List<GameObject> cards;
    RectTransform cooldownTimerTransform;
    TrapManager trapManager;
    TMP_Text resourceText;
    TMP_Text coinText;

    private void Awake()
    {
        cards = new List<GameObject>();
        trapManager = this.gameObject.GetComponent<TrapManager>();
        trapManager.onSelectedTrapChange += SelectedTrapChange;
        cooldownTimerTransform = cooldownTimer.GetComponent<RectTransform>();
        //resourceText = resourceTextBox.GetComponent<TMP_Text>();
        //coinText = coinTextBox.GetComponent<TMP_Text>();

        //UIObject.SetActive(true);


        //Coins.onChangeCoin += CoinChanged;
        //BuildingResources.onChangeResources += ResourceChanged;

        //BuildingResources.SetResourceCount(100000);


    }

    public void UseTrap(string name) 
    {
        foreach(GameObject card in cards)
        {
            Debug.Log("zoro" + card.name + " " + name);
            if(card.name == name)
            {
                Debug.Log("TRAP FOUND AND IS BEING USED");
                card.GetComponent<CardHolsterGraphics>().UseTrap();
                break;
            }
        }
    }

    public void AddCardToList(GameObject card)
    {
        cards.Add(card);
    }

    public void RegenUIElements()
    {
        MoveCooldownTimer();
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
        GameObject trap = trapManager.GetSelectedTrap();
        Vector3 spawnPos = new Vector3(10000, 10000);
        if (trap != null)
        {
            spawnPos = trap.transform.position;
            spawnPos = new Vector2(spawnPos.x, spawnPos.y + 3);
        }

        cooldownTimerTransform.transform.position = Camera.main.WorldToScreenPoint(spawnPos);
    }


    public void GainResoures(float count)
    {
        Coins.ChangeMaterial(count);
    }

    public void GainCoin(float count)
    {
        Coins.ChangeCoins(count);
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

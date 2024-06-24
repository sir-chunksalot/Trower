using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHolsterGraphics : MonoBehaviour
{
    [SerializeField] float cost;
    [SerializeField] bool isTrap;
    [SerializeField] GameObject costTextBox;
    [SerializeField] GameObject costButton;
    [SerializeField]  Sprite[] defensePhaseImages;
    [SerializeField] Sprite[] attackPhaseImages;
    [SerializeField] Sprite noChargeSprite;
    [SerializeField] Image chargeImage;
    [SerializeField] AudioSource selectTrap;
    [SerializeField] AudioSource badClick;
    [SerializeField] AudioSource place;
    [SerializeField] AudioSource buy;
    private Sprite[] activeSprite;
    TrapSelect trapSelect;
    private bool attackPhase;
    int count;

    private void Awake()
    {
        if(!isTrap) //this is so that when im using this script for the floors they dont have to worry about charge 
        {
            count = 9999999; //its a little scuffed i know but it doesnt matterrrrr
        }
        badClick = gameObject.GetComponent<AudioSource>();
        costTextBox.GetComponent<TMP_Text>().text = cost.ToString();
        activeSprite = attackPhaseImages;
        trapSelect = gameObject.GetComponentInParent<TrapSelect>();
        attackPhase = true;
    }
    private void Start()
    {
        gameObject.GetComponentInParent<UIManager>().AddCardToList(gameObject);
    }

    public void AttackPhase()
    {
        if(isTrap)
        {
            Debug.Log("attack Phase");
            activeSprite = attackPhaseImages;
            costButton.SetActive(false);
            costTextBox.SetActive(false);
            attackPhase = true;
            UpdateCount(0);
        }
    }

    public void DefensePhase()
    {
        attackPhase = false;
        if (isTrap)
        {
            Debug.Log("defense Phase");
            activeSprite = defensePhaseImages;
            costButton.SetActive(true);
            costTextBox.SetActive(true);
            UpdateCount(0);
        }
    }
    
    public void ManualGainCharge()
    {
        UpdateCount(1);
    }

    private void PurchaseCharge()
    {
        Debug.Log("cash = " + Coins.GetCoins());
        if(count <= 3 && Coins.GetCoins() >= cost)
        {
            Coins.ChangeCoins(-cost);
            buy.Play();
            UpdateCount(1);
        }
        else if(!isTrap && Coins.GetCoins() >= cost)
        {
            Coins.ChangeCoins(-cost);
            buy.Play();
        }
    }


    public void SelectTrap(string buttonName) //called by the button component in this card parent 
    {
        Debug.Log("DOOFEN coins" + Coins.GetCoins() + "cost" + cost);
        if(trapSelect.GetPlacing()) {
            return; 
        }
        if (isTrap && count > 0)
        {
            trapSelect.OnItemClicked(buttonName);
            selectTrap.Play();
        }
        else if (Coins.GetCoins() >= cost && !attackPhase)
        {
            Debug.Log("DOOFEN tried to buy");
            PurchaseCharge();
            trapSelect.OnItemClicked(buttonName);
            selectTrap.Play();
        }
        else
        {
            badClick.Play();
                Debug.Log("cant use effect");
                StartCoroutine(CantBuyEffect(noChargeSprite));

        }

    }

    private IEnumerator CantBuyEffect(Sprite sprite)
    {
        chargeImage.sprite = sprite;
        yield return new WaitForSeconds(.2f);
        chargeImage.sprite = activeSprite[0];
    }
    
    public void UseTrap(bool success) //called whenever a trap is placed by trap builder script or tower builder script
    {
        if(success)
        {
            place.Play();
            if(isTrap) {
                UpdateCount(-1);
            }

        }
        else
        {
            badClick.Play();
            Coins.ChangeCoins(cost);
            if (isTrap) { 
                UpdateCount(-1); 
            }
        }
        

    }

    private void UpdateCount(int charge)
    {
        count += charge;
        Debug.Log("PHASE" + activeSprite.Length);
        chargeImage.sprite = activeSprite[count];
    }
    public bool GetActiveStatus()
    {
        return true;
    }

    public float GetCost()
    {
        return cost;
    }

    public void MuteAudio(int index, float howLong)
    {
        float volume = 1;
        AudioSource audio = selectTrap;
        switch(index)
        {
            case 0:
                volume = selectTrap.volume;
                audio = selectTrap;
                selectTrap.volume = 0;
                break;
            case 1:
                volume = badClick.volume;
                audio = badClick;
                badClick.volume = 0;
                break;
            case 2:
                volume = place.volume;
                audio = place;
                place.volume = 0;
                break;
            case 3:
                volume = buy.volume;
                audio = buy;
                buy.volume = 0;
                break;
        }

        StartCoroutine(Unmute(howLong, volume, audio));
    }

    private IEnumerator Unmute(float length, float volume, AudioSource audio)
    {
        yield return new WaitForSeconds(length);
        audio.volume = volume;
    }

}

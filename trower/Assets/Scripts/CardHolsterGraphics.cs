using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHolsterGraphics : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] float cost;
    [SerializeField] bool isTrap;
    [SerializeField] string cardName;
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
    [SerializeField] GameObject lockedCard;
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

        gameObject.GetComponentInParent<UIManager>().AddCardToList(gameObject);
        if (UnlockManager.IsItemUnlocked(cardName))
        {
            UnlockCard();
        }
        Debug.Log("CARD AWAKE! doofen");
    }
    public void LockCard()
    {
        lockedCard.SetActive(true);
    }

    public void UnlockCard()
    {
        lockedCard.SetActive(false);
    }

    public void AttackPhase()
    {
        attackPhase = true;
        if (!gameObject.activeSelf) { return; }
        if (isTrap)
        {
            Debug.Log("attack Phase");
            activeSprite = attackPhaseImages;
            costButton.SetActive(false);
            costTextBox.SetActive(false);
            UpdateCount(0);
        }
    }

    public void DefensePhase()
    {
        Debug.Log("CARD DEFENSE PHASE DOOFEN");
        attackPhase = false;
        if (!gameObject.activeSelf) { return; }
        if (isTrap)
        {
            Debug.Log("defense Phase DOOFENB");
            activeSprite = defensePhaseImages;
            costButton.SetActive(true);
            costTextBox.SetActive(true);
            UpdateCount(0);
        }
    }
    
    public void ManualGainCharge()
    {
        if (!gameObject.activeSelf) { return; }
        UpdateCount(1);
    }

    public void PurchaseCharge()
    {
        if (!gameObject.activeSelf) { return; }
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

    public void SellCharge()
    {
        if(!gameObject.activeSelf) { return; }
        if(count >= 1)
        {
            Coins.ChangeCoins(cost);
            badClick.Play();
            UpdateCount(-1);
        }
    }
     
    public void SetCharge(int count)
    {
        if (!gameObject.activeSelf) { return; }
        this.count = count;
        UpdateCount(0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameObject.activeSelf) { return; }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right button pressed.");
        }
        else
        {
            Debug.Log("Right was not pressed");
        }
           
    }


    public void SelectTrap() //called by the button component in this card parent 
    {
        if (!gameObject.activeSelf) { return; }
        Debug.Log("DOOFEN coins" + Coins.GetCoins() + "cost" + cost);
        if(trapSelect.GetPlacing()) { //if player is  already placing, do not do anything
            return; 
        }
        if (isTrap && count > 0) //during any phase when a trap card has charge
        {
            trapSelect.OnItemClicked(cardName);
            selectTrap.Play();
        }
        else if (Coins.GetCoins() >= cost && !attackPhase) //bought and tried placing during defense phase
        {
            Debug.Log("DOOFEN tried to buy");
            PurchaseCharge();
            trapSelect.OnItemClicked(cardName);
            selectTrap.Play();
        }
        else //player has no money and no charge, L no buy for you
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
    
    public void UseCard(bool success) //called whenever a trap is placed by trap builder script or tower builder script
    {
        if(!gameObject.activeSelf) { return; }
        if(success)
        {
            place.Play();
            if(isTrap) {
                UpdateCount(-1);
            }

        }
        else
        {
            if(attackPhase && isTrap) { //when a card cant be placed but its the attack phase, dont refund them, dont take away the charge
                badClick.Play();
            }
            else //when a card cant be placed for whatever reason, play bad sound, refund them, and take away the charge
            {
                badClick.Play();
                Coins.ChangeCoins(cost);
                if (isTrap)
                {
                    UpdateCount(-1);
                }
            }

        }
        

    }

    private void UpdateCount(int charge)
    {
        count += charge;
        Debug.Log("WE ARE WE ARE " + gameObject + activeSprite);
        Debug.Log("PHASE" + activeSprite.Length);
        chargeImage.sprite = activeSprite[count];
    }

    public bool GetIsTrap()
    {
        return isTrap;
    }

    public float GetCost()
    {
        return cost;
    }

    public void MuteAudio(int index, float howLong)
    {
        if (!gameObject.activeSelf) { return; }

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

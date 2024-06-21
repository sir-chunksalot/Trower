using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHolsterGraphics : MonoBehaviour
{
    [SerializeField] float cost;
    [SerializeField] GameObject costTextBox;
    [SerializeField] Sprite[] chargeImages;
    [SerializeField] Image chargeImage;
    [SerializeField] AudioSource selectTrap;
    [SerializeField] AudioSource badClick;
    [SerializeField] AudioSource place;
    TrapSelect trapSelect;
    private bool isActive;
    int count;
    private void Start()
    {
        badClick = gameObject.GetComponent<AudioSource>();
        costTextBox.GetComponent<TMP_Text>().text = cost.ToString();
        isActive = true;
        gameObject.GetComponentInParent<UIManager>().AddCardToList(gameObject);
        trapSelect = gameObject.GetComponentInParent<TrapSelect>();
    }

    public void PurchaseTrap()
    {
        if(count <= 3)
        {
            UpdateCount(1);
        }
    }

    public void SelectTrap(string buttonName)
    {
        Debug.Log("coins" + Coins.GetCoins() + "cost" + cost);
        if (count > 0 && Coins.GetCoins() >= cost)
        {
            trapSelect.OnItemClicked(buttonName);
            selectTrap.Play();
        }
        else
        {
            badClick.Play();
        }

    }
    
    public void UseTrap() //called whenever a trap is placed
    {
        place.Play();
        UpdateCount(-1);

    }

    private void UpdateCount(int charge)
    {
        count += charge;
        chargeImage.sprite = chargeImages[count];
    }
    public bool GetActiveStatus()
    {
        return true;
    }

    public float GetCost()
    {
        return cost;
    }

    public void SetActiveStatus(bool active)
    {
        //isActive = active;
        //if (isActive)
        //{
        //    targetGraphic.sprite = baseSprite;
        //}
        //else
        //{
        //    targetGraphic.sprite = sleepySprite;
        //    timeLeft = cooldown;
        //}
    }

    //private void Update()
    //{
    //    if (timeLeft > 0)
    //    {
    //        timeLeft -= Time.deltaTime;
    //        progressBar.fillAmount = timeLeft / cooldown;
    //    }
    //    else
    //    {
    //        if (!isActive)
    //        {
    //            SetActiveStatus(true);
    //        }
    //    }
    //}


}

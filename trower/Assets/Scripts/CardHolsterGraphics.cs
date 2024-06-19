using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHolsterGraphics : MonoBehaviour
{
    [SerializeField] float cost;
    [SerializeField] GameObject costTextBox;
    [SerializeField] Sprite[] chargeImages;
    [SerializeField] Image chargeImage;
    TrapSelect trapSelect;
    private bool isActive;
    int count;
    private void Start()
    {
        costTextBox.GetComponent<TMP_Text>().text = cost.ToString();
        isActive = true;
        gameObject.GetComponentInParent<UIManager>().AddCardToList(gameObject);
        trapSelect = gameObject.GetComponentInParent<TrapSelect>();
    }

    private void Update()
    {
        Debug.Log("zoro " + count);
    }
    public void PurchaseTrap()
    {
        if(count <= 4)
        {
            UpdateCount(1);
        }
    }

    public void SelectTrap(string buttonName)
    {
        if(count > 0)
        {
            trapSelect.OnItemClicked(buttonName);
        }

    }
    
    public void UseTrap()
    {
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

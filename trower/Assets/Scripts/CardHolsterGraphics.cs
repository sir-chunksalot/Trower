using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardHolsterGraphics : MonoBehaviour
{
    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite sleepySprite;
    [SerializeField] Image targetGraphic;
    [SerializeField] float cost;
    [SerializeField] GameObject costTextBox;
    [SerializeField] float cooldown;
    [SerializeField] Image progressBar;
    float timeLeft;
    bool isActive;
    private void Start()
    {
        costTextBox.GetComponent<TMP_Text>().text = cost.ToString();
        isActive = true;
    }

    private void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            progressBar.fillAmount = timeLeft / cooldown;
        }
        else
        {
            if (!isActive)
            {
                SetActiveStatus(true);
            }
        }
    }

    public bool GetActiveStatus()
    {
        return isActive;
    }

    public float GetCost()
    {
        return cost;
    }

    public void SetActiveStatus(bool active)
    {
        isActive = active;
        if (isActive)
        {
            targetGraphic.sprite = baseSprite;
        }
        else
        {
            targetGraphic.sprite = sleepySprite;
            timeLeft = cooldown;
        }

    }
}

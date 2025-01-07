using TMPro;
using UnityEngine;

public class EndManager : MonoBehaviour
{
    [SerializeField] GameObject fader;
    [SerializeField] GameObject killsTextBox;
    [SerializeField] GameObject cashSpentTextbox;

    TMP_Text killsText;
    TMP_Text cashSpentText;
    private void Awake()
    {
        killsText = killsTextBox.GetComponent<TMP_Text>();
        cashSpentText = cashSpentTextbox.GetComponent<TMP_Text>();
    }
    public void DisplayStats(Statsheet statsheet)
    {
        //fader.GetComponent<MMFader>()
        killsText.text = statsheet.kills.ToString();
        cashSpentText.text = statsheet.cashSpent.ToString();
    }
}

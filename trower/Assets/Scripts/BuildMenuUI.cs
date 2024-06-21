using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] GameObject cardHolder;
    [SerializeField] GameObject attackMenuUI;
    [SerializeField] Button dropDown;
    [SerializeField] GameObject infoTab;
    [SerializeField] GameObject infoNameTextbox;
    [SerializeField] GameObject infoCostTextbox;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject materialCount;
    [SerializeField] GameObject materialTextBox;
    private bool isHiding;

    TMP_Text infoNameText;
    TMP_Text infoCostText;

    TMP_Text materialCountText;

    //private void Awake()
    //{
    //    Button btn = dropDown.GetComponent<Button>();
    //    btn.onClick.AddListener(HideMenu);

    //    infoNameText = infoNameTextbox.GetComponent<TMP_Text>();
    //    infoCostText = infoCostTextbox.GetComponent<TMP_Text>();
    //    materialCountText = materialTextBox.GetComponent<TMP_Text>();

    //    gameManager.GetComponent<TowerBuilder>().onTowerPlace += DisplayInfo;
    //    Coins.onChangeMaterial += EditMaterialCountText;
    //}
    //public void HideMenu()
    //{
    //    if (isHiding)
    //    {
    //        RevealMenu();

    //        //hides attack menu stuff
    //        attackMenuUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -128);
    //        cardHolder.GetComponent<RectTransform>().Rotate(0, 180, 0);
    //        cardHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(1883, 0);
    //    }
    //    else //hides build menu stuff
    //    {
    //        Debug.Log("Tried to hide build menu.");
    //        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(6.55f, -555);
    //        infoTab.GetComponent<RectTransform>().anchoredPosition = new Vector2(-703.7f, -81);
    //        materialCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(-75, 532);
    //        isHiding = true;

    //        //reveals attack menu stuff
    //        attackMenuUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    //        cardHolder.GetComponent<RectTransform>().Rotate(0, 180, 0);
    //        cardHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    //    }

    //}

    //public void RevealMenu()
    //{
    //    Debug.Log("Tried to reveal build menu.");
    //    gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(6.55f, -471f);
    //    infoTab.GetComponent<RectTransform>().anchoredPosition = new Vector2(-703.7f, 34.4f);
    //    materialCount.GetComponent<RectTransform>().anchoredPosition = new Vector2(-75, 430);
    //    isHiding = false;
    //}

    //public void DisplayInfo(object gameObject, EventArgs e)
    //{
    //    DisplayInfo(".");
    //}

    //public void DisplayInfo(string input)
    //{

    //    string[] inputs = input.Split('.');
    //    Debug.Log("sanjiro" + inputs.Length);
    //    if (gameManager.GetComponent<TowerBuilder>().GetIsPlacingFloor()) return;
    //    infoNameText.text = inputs[0];
    //    infoCostText.text = inputs[1];
    //}

    //private void EditMaterialCountText(object material, EventArgs e)
    //{
    //    materialCountText.text = material.ToString();
    //}

}

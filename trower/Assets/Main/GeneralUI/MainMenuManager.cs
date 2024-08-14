using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject levelCam;
    [SerializeField] GameObject mainCam;
    [SerializeField] GameObject levelTextObj;
    [SerializeField] GameObject amogusButtonObj;
    [SerializeField] GameObject logo;
    [SerializeField] List<GameObject> amogusGraphics;
    [SerializeField] Sprite susLogo;
    [SerializeField] Sprite regularLogo;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject levelsMenu;
    [SerializeField] GameObject extrasMenu;
    Button amogusButton;
    CameraController camController;
    OptionsManager optionsManager;
    GameManager gameManagerScript;
    LevelLoader levelLoader;
    TMP_Text textBox;
    private int selectedLevel;
    private int selectedWorld;

    private bool mainView;
    private void Awake()
    {
        levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        optionsManager = gameManager.GetComponent<OptionsManager>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
        gameManagerScript.OnSceneLoaded += OnSceneLoad;
        selectedLevel = 1;
        selectedWorld = 1;
        amogusButton = amogusButtonObj.GetComponent<Button>();
        textBox = levelTextObj.GetComponent<TMP_Text>();
        camController = gameObject.GetComponent<CameraController>();
        mainView = true;
    }

    public void OnSceneLoad(object sender, EventArgs e)
    {
        if (this == null) return;
        if (optionsManager.isSusMode)
        {
            AmogusSwitch();
        }
    }
    public void SideBar()
    {
        if (mainView == false)
        {
            Debug.Log("went to BackToMain");
            mainView = true;
            MainMenu();
        }
        else
        {
            Debug.Log("went to level select");
            camController.ActivateCamera(levelCam);
            mainView = false;
        }
    }
    public void ToggleLevelsMenu()
    {
        if (levelsMenu.activeSelf) { SideBar(); }
        else
        {
            levelsMenu.SetActive(true);
            extrasMenu.SetActive(false);
            if (mainView)
            {
                SideBar();
            }
        }

    }
    public void ToggleExtrasMenu()
    {
        if (extrasMenu.activeSelf) { SideBar(); }
        else
        {
            levelsMenu.SetActive(false);
            extrasMenu.SetActive(true);
            if (mainView)
            {
                SideBar();
            }
        }
    }
    public void MainMenu()
    {
        camController.ActivateCamera(mainCam);
        mainView = true;
    }

    public void SwitchSelectedLevel(string text)
    {
        if (text.Substring(1, 1) == "-")
        {
            string textNum;
            textNum = text.Substring(0, 1);
            selectedLevel = int.Parse(textNum);
            textNum = text.Substring(2, 1);
            selectedWorld = int.Parse(textNum);
        }

        textBox.text = text;
    }

    public void PlayMinigame(string game)
    {
        levelLoader.ChangeScene(game);
    }

    public void ToggleOptions()
    {
        Debug.Log("Toggled");
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }


    public void Continue()
    {
        levelLoader.ChangeLevel(selectedWorld, selectedLevel);
    }

    public void AmogusSwitch()
    {
        if (amogusGraphics[0].activeSelf)
        {
            amogusGraphics[1].SetActive(true);
            amogusButton.targetGraphic = amogusGraphics[1].GetComponent<Image>();

            amogusGraphics[0].SetActive(false);

            logo.GetComponent<Image>().sprite = susLogo;

            optionsManager.isSusMode = true;
        }
        else
        {
            amogusGraphics[0].SetActive(true);
            amogusButton.targetGraphic = amogusGraphics[0].GetComponent<Image>();

            amogusGraphics[1].SetActive(false);

            logo.GetComponent<Image>().sprite = regularLogo;

            optionsManager.isSusMode = false;
        }

    }
}

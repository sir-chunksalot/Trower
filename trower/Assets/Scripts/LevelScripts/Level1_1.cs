//using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level1_1 : MonoBehaviour
{
    [SerializeField] bool inIntroduction;
    [SerializeField] GameObject gameManager;

    [SerializeField] GameObject pressSpaceUI;
    [SerializeField] GameObject pressSpaceShadow;
    //intro
    [SerializeField] GameObject shadow;
    [SerializeField] GameObject coffin;
    [SerializeField] GameObject VCAM1;
    [SerializeField] GameObject VCAM2;
    [SerializeField] GameObject MainView;
    [SerializeField] GameObject MainView2;
    [SerializeField] GameObject VCAMPanRight;
    [SerializeField] SpriteRenderer crack;
    [SerializeField] Sprite[] cracks;
    [SerializeField] GameObject flower;
    [SerializeField] Sprite[] flowerGrowthStages;
    //[SerializeField] MMFeedbacks growFlowerEffect;
    //[SerializeField] MMFeedbacks superShakeEffect;
    [SerializeField] GameObject lightsOut;
    [SerializeField] GameObject bigCrackleft;
    [SerializeField] GameObject bigCrackRight;
    [SerializeField] GameObject bigCrackMid1;
    [SerializeField] GameObject bigCrackMid2;
    [SerializeField] GameObject vineGrowth;
    [SerializeField] GameObject inside;
    [SerializeField] Sprite inside2;
    [SerializeField] GameObject doors;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject blackBG;
    [SerializeField] GameObject tvScreen;
    [SerializeField] GameObject firstEnemy;

    //ui
    [SerializeField] GameObject doorsUI;
    [SerializeField] GameObject highlightUI;
    [SerializeField] GameObject dialogueTriggerSlowpokes1;
    [SerializeField] GameObject dialogueTriggerSlowpokes2;
    [SerializeField] GameObject extraArrowForSlowpokes1;
    [SerializeField] GameObject extraArrowForSlowpokes2;
    [SerializeField] GameObject extraArrowForSlowpokes3;
    [SerializeField] GameObject extraArrowForSlowpokes4;
    [SerializeField] GameObject dialogueTriggerSecondSpike;
    [SerializeField] GameObject dialogueTriggerSecondSpike1;
    [SerializeField] GameObject dialogueTriggerSecondSpike2;
    [SerializeField] GameObject dialogueTriggerHoverOverEnemy;
    [SerializeField] GameObject dialogueTriggerHigh5;
    [SerializeField] GameObject dialogueTriggerWhoops;
    [SerializeField] GameObject dialogueTriggerkillThatGuyToo;
    [SerializeField] GameObject dialogueTriggerTabSwitch;
    [SerializeField] GameObject dialogueTriggerDefendEverywhere;
    [SerializeField] GameObject dialogueTriggerAreYouSureAboutThat;

    //torches
    [SerializeField] GameObject[] redTorches;
    [SerializeField] GameObject[] greenTorches;
    [SerializeField] Sprite emptyTorch;

    //level
    [SerializeField] GameObject secondEnemy;
    [SerializeField] GameObject rightFloor;
    [SerializeField] GameObject arrow2;
    [SerializeField] GameObject arrowSpear;
    [SerializeField] GameObject arrowSpear2;
    [SerializeField] GameObject arrowBuild;
    [SerializeField] GameObject firstTrap;
    [SerializeField] GameObject arrowContinue;
    [SerializeField] GameObject card;
    [SerializeField] GameObject cardHolder;
    [SerializeField] GameObject spearsPrefab;
    [SerializeField] GameObject spaceEffect;
    [SerializeField] Sprite goofyHeroFace;
    [SerializeField] GameObject buildTab;



    //particle effects
    [SerializeField] GameObject rubbleParticle1;
    [SerializeField] GameObject rubbleParticle2;

    //tooltips
    [SerializeField] GameObject dialogueManagerObj;
    [SerializeField] GameObject trapTeacher;
    [SerializeField] GameObject trapDaddy;


    //starterEnemies
    [SerializeField] GameObject[] starterEnemies;

    CardHolsterGraphics cardHolster;
    CameraController camController;
    WaveManager waveManager;
    TrapBuilder trapBuilder;
    LevelManager levelManager;
    UIManager uiManager;
    int shakeCount;
    bool gottaWait;
    bool beenThereDoneThat;
    bool beenThereDoneThat2;
    bool checkForClick;
    bool tryAndArm;
    bool secondTrapPlaced;
    bool defensePhase;
    bool areYouSure;

    private void Awake()
    {
        uiManager = gameManager.GetComponent<UIManager>();
        uiManager.OnClickContinue += TryContinue;
    }
    void Start()
    {
        levelManager = gameManager.GetComponent<LevelManager>();
        cardHolster = card.GetComponent<CardHolsterGraphics>();
        Debug.Log(gameObject + "snoop");
        waveManager = gameManager.GetComponent<WaveManager>();
        waveManager.OnDefensePhaseStart += InstructDefensePhase;
        Debug.Log(waveManager + "snoopy");
        camController = this.GetComponent<CameraController>();
        trapBuilder = gameManager.GetComponent<TrapBuilder>();
        trapBuilder.onTrapPlace += DisableTrap;
        trapBuilder.onTrapPlace += CheckPos;
        
        if (inIntroduction)
        {
            GoInside();
            camController.ActivateCamera(VCAM1);

        }
        else
        {
            GoOutside();
            camController.ActivateCamera(MainView);

        }
    }





    public void Grow(InputAction.CallbackContext context)
    {
        if (context.performed && inIntroduction)
        {
            if (shakeCount <= 2 && !gottaWait)
            {
                pressSpaceShadow.SetActive(true);
                //growFlowerEffect.PlayFeedbacks();
                coffin.GetComponent<Animator>().SetTrigger("Shake");
                Debug.Log("shook");
                shakeCount++;
                flower.GetComponent<SpriteRenderer>().sprite = flowerGrowthStages[shakeCount];
                crack.sprite = cracks[shakeCount];
                StartCoroutine(ShakeWait(.5f));
            }
            if (shakeCount >= 2 && !gottaWait)
            {
                gottaWait = true;
                flower.GetComponent<Animator>().enabled = true;
                pressSpaceShadow.SetActive(false);
                pressSpaceUI.SetActive(false);
                StartCoroutine(PanDown(1.4f));
            }
            flower.GetComponent<Animator>().SetFloat("ShakeCount", shakeCount);
        }

        if (context.canceled && inIntroduction)
        {
            pressSpaceShadow.SetActive(false);
        }
    }

    private IEnumerator PanDown(float time)
    {
        //camController.ActivateCamera(VCAM2);
        yield return new WaitForSeconds(time);
        coffin.SetActive(true);
        shadow.SetActive(false);
        shakeCount = 0;
        //StartCoroutine(SecondShaking(3f));
    }

    private IEnumerator DelayBeforeOtherEffects(float time)
    {
        yield return new WaitForSeconds(.5f);
        //rubbleParticle1.GetComponent<ParticleSystem>().Play();
        rubbleParticle2.GetComponent<ParticleSystem>().Play();
        //rubbleParticle.GetComponent<ParticleSystem>().Stop();
        if (shakeCount == 1)
        {
            bigCrackleft.SetActive(true);
        }
        if (shakeCount == 2)
        {
            bigCrackRight.SetActive(true);
            bigCrackMid1.SetActive(true);
        }
        if (shakeCount == 3)
        {
            bigCrackMid2.SetActive(true);
            vineGrowth.SetActive(true);
        }
        if (shakeCount <= 2)
        {
            StartCoroutine(SecondShaking(time));
        }
        else
        {
            //finale
            StartCoroutine(TorchesEffect(3.5f));
        }
    }
    private IEnumerator SecondShaking(float time)
    {
        shakeCount++;
        Debug.Log("fart");
        yield return new WaitForSeconds(time);
        //superShakeEffect.PlayFeedbacks();f
        StartCoroutine(DelayBeforeOtherEffects(time));

    }
    private IEnumerator ShakeWait(float time)
    {
        gottaWait = true;
        yield return new WaitForSeconds(time);
        gottaWait = false;
    }

    private IEnumerator TorchesEffect(float time)
    {
        yield return new WaitForSeconds(time);
        //superDarkEffect.PlayFeedbacks();g
        lightsOut.GetComponent<Animator>().enabled = true;
        redTorches[0].GetComponent<Animator>().enabled = false;
        redTorches[1].GetComponent<Animator>().enabled = false;
        redTorches[0].GetComponent<SpriteRenderer>().sprite = emptyTorch;
        redTorches[1].GetComponent<SpriteRenderer>().sprite = emptyTorch;

        StartCoroutine(TorchesWait(1.5f));
    }

    private IEnumerator TorchesWait(float time)
    {
        yield return new WaitForSeconds(time);
        redTorches[0].SetActive(false);
        redTorches[1].SetActive(false);
        greenTorches[0].SetActive(true);
        greenTorches[1].SetActive(true);

        StartCoroutine(EnemiesRun(1));
    }

    private IEnumerator EnemiesRun(float time)
    {
        yield return new WaitForSeconds(time);
        //rock fall anim
        //enemies run anim
        doors.GetComponent<Animator>().enabled = true;
        inside.GetComponent<SpriteRenderer>().sprite = inside2;
        StartCoroutine(UIPause(.5f));
    }

    private IEnumerator UIPause(float time)
    {
        yield return new WaitForSeconds(time);
        arrow.SetActive(true);
        doorsUI.SetActive(true);
        highlightUI.SetActive(true);
    }

    //SECOND HALF
    //-----------------------------------------------------------------

    public void GoOutside() //triggered on button press
    {
        pressSpaceUI.SetActive(false);
        inside.SetActive(false);
        greenTorches[0].SetActive(false);
        greenTorches[1].SetActive(false);
        redTorches[0].SetActive(false);
        redTorches[1].SetActive(false);
        doorsUI.SetActive(false);
        highlightUI.SetActive(false);
        bigCrackleft.SetActive(false);
        bigCrackRight.SetActive(false);
        bigCrackMid1.SetActive(false);
        bigCrackMid2.SetActive(false);
        vineGrowth.SetActive(false);
        arrow.SetActive(false);
        coffin.SetActive(false);
        shadow.SetActive(false);
        doors.SetActive(false);
        flower.SetActive(false);
        blackBG.SetActive(false);
        tvScreen.SetActive(true);
        lightsOut.SetActive(false);


        arrow2.SetActive(true);
        firstTrap.GetComponent<Trap>().DisableTrap();
        //StartCoroutine(ExtraInfoForSlowpokes());
        //re enable later
        dialogueTriggerHigh5.GetComponentInChildren<DialogueBox>().ManualReadMessage();
        cardHolster.MuteAudio(3, .2f);
        cardHolster.ManualGainCharge();
        StartCoroutine(CheckForHover(true));

    }


    private IEnumerator CheckForHover(bool first)
    {
        bool isFirst = first;
        yield return new WaitForSeconds(.2f);
        if (spaceEffect.activeInHierarchy)
        {
            dialogueTriggerHoverOverEnemy.SetActive(false);
            dialogueTriggerHigh5.SetActive(true);
        }
        else
        {
            dialogueTriggerHigh5.SetActive(false);
            if (isFirst)
            {
                isFirst = false;
                dialogueTriggerHoverOverEnemy.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            }
            dialogueTriggerHoverOverEnemy.SetActive(true);

        }
        StartCoroutine(CheckForHover(isFirst));
    }
    public void GoInside()
    {
        pressSpaceUI.SetActive(true);
        flower.SetActive(true);
        blackBG.SetActive(true);
        tvScreen.SetActive(false);
        doors.SetActive(true);
        shadow.SetActive(true);
        redTorches[0].SetActive(true);
        redTorches[1].SetActive(true);
        inside.SetActive(true);
        lightsOut.SetActive(true);
        arrow2.SetActive(false);
    }

    //private void OnTriggerEnter2D(Collider2D collision) //WHEN ENEMY BREACHES DEFENSES
    //{
    //    if(collision.tag == "PrisonGuard")
    //    {
    //        if (firstEnemy == null && !checkForClick) 
    //        {
    //            cardHolder.SetActive(true);
    //            cardHolster.PurchaseTrap();
    //            dialogueTriggerSecondSpike.GetComponentInChildren<DialogueBox>().ManualReadMessage();
    //            waveManager.SwitchDefensePhase(false);
    //            arrowSpear.SetActive(true);
    //            checkForClick = true;
    //        }
    //    }
    //}

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (checkForClick && !secondTrapPlaced)
            {
                StartCoroutine(WaitForClick());
            }
            else
            {
                Debug.Log("quepp");
            }

            if(defensePhase)
            {
                if(arrowBuild.activeInHierarchy)
                {
                    StartCoroutine(CheckForBuildTab());
                }
                else
                {
                    dialogueTriggerDefendEverywhere.SetActive(false);
                }

                if(areYouSure)
                {
                    dialogueTriggerAreYouSureAboutThat.SetActive(false);
                    defensePhase = false;
                    dialogueTriggerDefendEverywhere.SetActive(false);
                    arrowBuild.SetActive(false);
                    uiManager.SetDenySwitch(false);
                }
            }
        }

    }

    private IEnumerator CheckForBuildTab()
    {
        yield return new WaitForSeconds(.2f);
        {
            if(buildTab.GetComponent<RectTransform>().localPosition.y >= 0)
            {
                dialogueTriggerTabSwitch.SetActive(false);
                arrowBuild.SetActive(false);
                dialogueTriggerDefendEverywhere.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            }
        }
    }

    public void DisableTrap(object trap, EventArgs e)
    {
        if(trapDaddy.transform.childCount >= 1 && !secondTrapPlaced)
        {
            trapDaddy.GetComponentInChildren<Trap>().DisableTrap();
            secondTrapPlaced = true;
        }

    }

    public void CheckPos(object trap, EventArgs e)
    {
        if(defensePhase)
        {
            GameObject trapPos = (GameObject)trap;
            if(trapPos.transform.position.x < 0)
            {
                uiManager.SetDenySwitch(false);
            }
        }
    }

    private IEnumerator WaitForClick()
    {
        yield return new WaitForSeconds(.2f);
        if (trapBuilder.GetPlacingTrap())
        {
            Debug.Log("TRAP placing trap");
            dialogueTriggerWhoops.SetActive(false);
            dialogueTriggerSecondSpike.SetActive(false);
            dialogueTriggerSecondSpike2.SetActive(false);
            dialogueTriggerSecondSpike1.SetActive(true);
            arrowSpear.SetActive(false);
            arrowSpear2.SetActive(true);
            if (!dialogueTriggerSecondSpike1.GetComponentInChildren<DialogueBox>().GetHasBeenRead())
            {
                dialogueTriggerSecondSpike1.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            }
            StartCoroutine(WaitForClick());

        }
        else if (trapDaddy.transform.childCount >= 1)
        {
            Debug.Log("TRAP PLACED");
            arrowSpear.SetActive(false);
            dialogueTriggerSecondSpike1.SetActive(false);
            dialogueTriggerSecondSpike.SetActive(false);
            dialogueTriggerSecondSpike2.SetActive(true);
            if(!dialogueTriggerSecondSpike2.GetComponentInChildren<DialogueBox>().GetHasBeenRead())
            {
                dialogueTriggerSecondSpike2.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            }
            trapDaddy.GetComponentInChildren<Trap>().DisableTrap();
            secondTrapPlaced = true;
        }
        else
        {
            Debug.Log("TRAP DEFAULT");
            arrowSpear.SetActive(true);
            arrowSpear2.SetActive(false);
            dialogueTriggerSecondSpike1.SetActive(false);
            dialogueTriggerSecondSpike2.SetActive(false);
            dialogueTriggerSecondSpike.SetActive(true);
            if(!dialogueTriggerSecondSpike.GetComponentInChildren<DialogueBox>().GetHasBeenRead())
            {
                dialogueTriggerSecondSpike.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            }
            StartCoroutine(WaitForClick());
        }
    }

    public void InstructDefensePhase(object sender, EventArgs e)
    {
        camController.ActivateCamera(MainView2);
        arrowBuild.SetActive(true);
        StartCoroutine(WaitForInstruction());
    }

    private IEnumerator WaitForInstruction()
    {
        yield return new WaitForSeconds(1.5f);
        dialogueTriggerTabSwitch.GetComponentInChildren<DialogueBox>().ManualReadMessage();
        defensePhase = true;
        uiManager.SetDenySwitch(true);
    }


    public void OnSpacePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(!beenThereDoneThat)
            {
                if (spaceEffect.activeInHierarchy)
                {
                    StopAllCoroutines();
                    dialogueTriggerHigh5.SetActive(false);
                    dialogueTriggerHoverOverEnemy.SetActive(false);
                    beenThereDoneThat = true;
                    firstTrap.GetComponent<Trap>().ManualTrapActivate();
                    secondEnemy.GetComponent<Hero>().AttackPhase();
                    secondEnemy.GetComponent<Hero>().ChangeSpeed(6);
                    StartCoroutine(MakeHeroStop());
                    StartCoroutine(WaitForJoke());
                    //firstTrap.GetComponent<Trap>().DisableTrap();
                    //Debug.Log("they killed him bois");
                    //DialogueManager dialogueManager = dialogueManagerObj.GetComponent<DialogueManager>();
                    //dialogueManager.DiscoverCursorSwitch(false);

                    //arrow2.SetActive(false);
                    //dialogueTriggerSlowpokes1.SetActive(false);
                    //dialogueTriggerSlowpokes2.SetActive(false);
                    //extraArrowForSlowpokes1.SetActive(false);
                    //extraArrowForSlowpokes2.SetActive(false);
                    //extraArrowForSlowpokes3.SetActive(false);
                    //extraArrowForSlowpokes4.SetActive(false);
                    //StartCoroutine(DelayBeforeCam());
                }
            }
            if(secondTrapPlaced && !beenThereDoneThat2 && trapDaddy.GetComponentInChildren<Trap>().GetSpaceEffect().activeInHierarchy)
            {
                trapDaddy.GetComponentInChildren<Trap>().ManualTrapActivate();
                beenThereDoneThat2 = true;
                StartCoroutine(CheckForSecondDeath());
                
            }
           
        }
    }

    private IEnumerator CheckForSecondDeath()
    {
        yield return new WaitForSeconds(2);
        if (secondEnemy == null)
        {
            dialogueTriggerSecondSpike2.SetActive(false);
            dialogueTriggerkillThatGuyToo.SetActive(false);
            arrowSpear2.SetActive(false);
            camController.ActivateCamera(VCAMPanRight, 5);
            StartCoroutine(WaitForCam());
        }
        else
        {
            beenThereDoneThat2 = false;
        }
    }

    private IEnumerator MakeHeroStop()
    {
        yield return new WaitForSeconds(4f);
        secondEnemy.GetComponent<Hero>().DefensePhase();
        secondEnemy.GetComponent<Animator>().enabled = false;
        secondEnemy.GetComponent<SpriteRenderer>().sprite = goofyHeroFace;
    }

    private IEnumerator WaitForJoke()
    {
        yield return new WaitForSeconds(1.5f);
        if(trapDaddy.transform.childCount < 1)
        {
            arrow2.SetActive(false);
            dialogueTriggerWhoops.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            StartCoroutine(InstructSecondSpear());
        }
        else
        {
            trapDaddy.GetComponentInChildren<Trap>().DisableTrap();
            arrow2.SetActive(false);
            dialogueTriggerkillThatGuyToo.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            dialogueTriggerWhoops.SetActive(false);
            secondTrapPlaced = true;
            beenThereDoneThat2 = false;
        }

    }

    private void TryContinue(object sender, EventArgs e)
    {
        if(uiManager.GetDenySwitch())
        {
            dialogueTriggerAreYouSureAboutThat.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            areYouSure = true;
        }
        else
        {
            defensePhase = false;
        }

    }

    private IEnumerator InstructSecondSpear()
    {
        yield return new WaitForSeconds(2);
        if(trapDaddy.transform.childCount < 1)
        {
            dialogueTriggerWhoops.SetActive(false);
            dialogueTriggerSecondSpike.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            arrowSpear.SetActive(true);
            StartCoroutine(WaitForClick());
        }
        else
        {
            trapDaddy.GetComponentInChildren<Trap>().DisableTrap();
            dialogueTriggerkillThatGuyToo.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            dialogueTriggerWhoops.SetActive(false);
            secondTrapPlaced = true;
            beenThereDoneThat2 = false;
        }
    }
    
    private IEnumerator WaitForCam()
    {
        yield return new WaitForSeconds(7);
        waveManager.SwitchAttackPhase(true);
        StartCoroutine(WaitABit());
        StartCoroutine(RunForestRun());

    }

    private IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(4.2f);
        firstTrap.GetComponent<Trap>().EnableTrap();
        trapDaddy.GetComponentInChildren<Trap>().EnableTrap();
    }

    private IEnumerator RunForestRun()
    {
        yield return new WaitForSeconds(4);
        foreach (GameObject enemy in starterEnemies)
        {
            enemy.GetComponent<Animator>().enabled = true;
        }

    }

    private IEnumerator ExtraInfoForSlowpokes()
    {
        yield return new WaitForSeconds(20);
        if (firstEnemy != null)
        {
            Debug.Log("Manual read");
            dialogueTriggerSlowpokes1.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            StartCoroutine(ExtraInfoForSlowpokes2());
        }
    }

    private IEnumerator ExtraInfoForSlowpokes2()
    {
        yield return new WaitForSeconds(12);
        if(firstEnemy != null)
        {
            Debug.Log("manual read 2");
            dialogueTriggerSlowpokes1.SetActive(false);
            dialogueTriggerSlowpokes2.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            StartCoroutine(ExtraInfoForSlowpokes3());
        }

    }

    private IEnumerator ExtraInfoForSlowpokes3()
    {
        yield return new WaitForSeconds(12);
        if (firstEnemy != null)
        {
            extraArrowForSlowpokes1.SetActive(true);
            StartCoroutine(ExtraInfoForSlowpokes4());
        }

    }
    private IEnumerator ExtraInfoForSlowpokes4()
    {
        yield return new WaitForSeconds(12);
        if (firstEnemy != null)
        {
            extraArrowForSlowpokes2.SetActive(true);
            StartCoroutine(ExtraInfoForSlowpokes5());
        }
    }
    private IEnumerator ExtraInfoForSlowpokes5()
    {
        yield return new WaitForSeconds(12);
        if (firstEnemy != null)
        {
            extraArrowForSlowpokes3.SetActive(true);
            StartCoroutine(ExtraInfoForSlowpokes6());
        }
    }
    private IEnumerator ExtraInfoForSlowpokes6()
    {
        yield return new WaitForSeconds(12);
        if (firstEnemy != null)
        {
            extraArrowForSlowpokes4.SetActive(true);
        }
    }


}

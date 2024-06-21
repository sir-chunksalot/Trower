using MoreMountains.Feedbacks;
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
    [SerializeField] GameObject VCAMPanRight;
    [SerializeField] SpriteRenderer crack;
    [SerializeField] Sprite[] cracks;
    [SerializeField] GameObject flower;
    [SerializeField] Sprite[] flowerGrowthStages;
    [SerializeField] MMFeedbacks growFlowerEffect;
    [SerializeField] MMFeedbacks superShakeEffect;
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
    [SerializeField] GameObject dialogueTriggerSecondSpike3;

    //torches
    [SerializeField] GameObject[] redTorches;
    [SerializeField] GameObject[] greenTorches;
    [SerializeField] Sprite emptyTorch;

    //level
    [SerializeField] GameObject rightFloor;
    [SerializeField] GameObject arrow2;
    [SerializeField] GameObject arrowSpear;
    [SerializeField] GameObject firstTrap;
    [SerializeField] GameObject arrowContinue;
    [SerializeField] GameObject card;
    [SerializeField] GameObject cardHolder;


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
    int shakeCount;
    bool gottaWait;
    bool beenThereDoneThat;
    bool checkForClick;
    bool tryAndArm;
    bool finalPhase;
    void Start()
    {
        cardHolster = card.GetComponent<CardHolsterGraphics>();
        Debug.Log(gameObject + "snoop");
        waveManager = gameManager.GetComponent<WaveManager>();
        Debug.Log(waveManager + "snoopy");
        camController = this.GetComponent<CameraController>();
        trapBuilder = gameManager.GetComponent<TrapBuilder>();

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

        Coins.SetCoin(10000);
    }





    public void Grow(InputAction.CallbackContext context)
    {
        if (context.performed && inIntroduction)
        {
            if (shakeCount <= 2 && !gottaWait)
            {
                pressSpaceShadow.SetActive(true);
                growFlowerEffect.PlayFeedbacks();
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
        superShakeEffect.PlayFeedbacks();
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
        //superDarkEffect.PlayFeedbacks();
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
        gameManager.GetComponent<TowerBuilder>().AddPlacedFloor(rightFloor);


        arrow2.SetActive(true);

        //StartCoroutine(ExtraInfoForSlowpokes());
        //re enable later

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

    private void OnTriggerEnter2D(Collider2D collision) //WHEN ENEMY BREACHES DEFENSES
    {
        if(collision.tag == "PrisonGuard")
        {
            if (firstEnemy == null && !checkForClick) 
            {
                cardHolder.SetActive(true);
                cardHolster.PurchaseTrap();
                dialogueTriggerSecondSpike.GetComponentInChildren<DialogueBox>().ManualReadMessage();
                waveManager.SwitchDefensePhase(false);
                arrowSpear.SetActive(true);
                checkForClick = true;
            }
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (checkForClick && !finalPhase)
            {
                StartCoroutine(WaitForClick());
            }
            else
            {
                Debug.Log("quepp");
            }
            if(finalPhase)
            {
                dialogueTriggerSecondSpike3.SetActive(false);
                arrowContinue.SetActive(false);
            }
        }

    }

    private IEnumerator WaitForClick()
    {
        yield return new WaitForSeconds(.2f);
        if (trapBuilder.GetPlacingTrap())
        {
            dialogueTriggerSecondSpike.SetActive(false);
            dialogueTriggerSecondSpike2.SetActive(false);
            dialogueTriggerSecondSpike1.SetActive(true);
            dialogueTriggerSecondSpike1.GetComponentInChildren<DialogueBox>().ManualReadMessage();

        }
        else if (trapDaddy.transform.childCount >= 1)
        {
            arrowSpear.SetActive(false);
            dialogueTriggerSecondSpike1.SetActive(false);
            dialogueTriggerSecondSpike.SetActive(false);
            dialogueTriggerSecondSpike2.SetActive(true);
            dialogueTriggerSecondSpike2.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            StartCoroutine(CheckForArm(trapDaddy.transform.GetComponentsInChildren<Transform>()[1].gameObject)); ;
        }
        else
        {
            dialogueTriggerSecondSpike1.SetActive(false);
            dialogueTriggerSecondSpike2.SetActive(false);
            dialogueTriggerSecondSpike.SetActive(true);
            dialogueTriggerSecondSpike.GetComponentInChildren<DialogueBox>().ManualReadMessage();
        }
    }

    private IEnumerator CheckForArm(GameObject trap)
    {
        yield return new WaitForSeconds(.2f);
        if (trap.GetComponent<Trap>().GetIsArmed())
        {
            dialogueTriggerSecondSpike1.SetActive(false);
            dialogueTriggerSecondSpike.SetActive(false);
            dialogueTriggerSecondSpike2.SetActive(false);
            dialogueTriggerSecondSpike3.SetActive(true);
            dialogueTriggerSecondSpike3.GetComponentInChildren<DialogueBox>().ManualReadMessage();
            arrowContinue.SetActive(true);
            finalPhase = true;
            StopCoroutine(CheckForArm(trap));
        }
        else
        {
            StartCoroutine(CheckForArm(trap));
        }
    }


    public void OnSpacePressed(InputAction.CallbackContext context)
    {
        if (context.performed && !beenThereDoneThat)
        {
            if (firstTrap.GetComponent<Trap>().GetCanAttack())
            {
                beenThereDoneThat = true;
                firstTrap.GetComponent<Trap>().ManualTrapActivate();
                firstTrap.GetComponent<Trap>().DisableTrap();
                Debug.Log("they killed him bois");
                DialogueManager dialogueManager = dialogueManagerObj.GetComponent<DialogueManager>();
                dialogueManager.DiscoverCursorSwitch(false);

                arrow2.SetActive(false);
                dialogueTriggerSlowpokes1.SetActive(false);
                dialogueTriggerSlowpokes2.SetActive(false);
                extraArrowForSlowpokes1.SetActive(false);
                extraArrowForSlowpokes2.SetActive(false);
                extraArrowForSlowpokes3.SetActive(false);
                extraArrowForSlowpokes4.SetActive(false);
                StartCoroutine(DelayBeforeCam());
            }

        }
    }

    private IEnumerator DelayBeforeCam()
    {
        yield return new WaitForSeconds(1);
        
            camController.ActivateCamera(VCAMPanRight, 5);
            StartCoroutine(WaitForCam());

    }

    private IEnumerator WaitForCam()
    {
        yield return new WaitForSeconds(7);
        waveManager.SwitchAttackPhase(true);
        firstTrap.GetComponent<Trap>().EnableTrap();
        StartCoroutine(RunForestRun());

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

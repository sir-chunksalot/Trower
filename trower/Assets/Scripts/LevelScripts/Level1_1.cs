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

    //torches
    [SerializeField] GameObject[] redTorches;
    [SerializeField] GameObject[] greenTorches;
    [SerializeField] Sprite emptyTorch;

    //level
    [SerializeField] GameObject rightFloor;
    [SerializeField] GameObject arrow2;


    //particle effects
    [SerializeField] GameObject rubbleParticle1;
    [SerializeField] GameObject rubbleParticle2;

    //tooltips
    [SerializeField] GameObject dialogueManagerObj;
    [SerializeField] GameObject trapTeacher;

    //starterEnemies
    [SerializeField] GameObject[] starterEnemies;

    CameraController camController;
    int shakeCount;
    bool gottaWait;
    bool beenThereDoneThat;
    void Start()
    {
        Debug.Log(gameObject + "snoop");
        WaveManager waveManager = gameManager.GetComponent<WaveManager>();
        Debug.Log(waveManager + "snoopy");
        foreach (GameObject enemy in starterEnemies)
        {
            Debug.Log(enemy + " " + waveManager + " " + starterEnemies + "squad");
            waveManager.AddEnemyToList(enemy);
        }
        camController = this.GetComponent<CameraController>();

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
                growFlowerEffect.PlayFeedbacks();
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
        camController.ActivateCamera(VCAM2);
        yield return new WaitForSeconds(time);
        coffin.SetActive(true);
        shadow.SetActive(false);
        shakeCount = 0;
        StartCoroutine(SecondShaking(3f));
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

        StartCoroutine(ExtraInfoForSlowpokes());

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

    private IEnumerator ExtraInfoForSlowpokes()
    {
        yield return new WaitForSeconds(5);
    }

    public void OnSpacePressed(InputAction.CallbackContext context)
    {
        if (context.performed && !beenThereDoneThat)
        {
            beenThereDoneThat = true;
            StartCoroutine(CheckIfGuyDied());
        }
    }

    private IEnumerator CheckIfGuyDied()
    {
        yield return new WaitForSeconds(1);
        if (firstEnemy == null)
        {
            Debug.Log("they killed him bois");
            DialogueManager dialogueManager = dialogueManagerObj.GetComponent<DialogueManager>();
            dialogueManager.DiscoverCursorSwitch(false);
            trapTeacher.SetActive(false);
            camController.ActivateCamera(VCAMPanRight, 5);
            StartCoroutine(WaitForCam());
        }
        else
        {
            StartCoroutine(CheckIfGuyDied());
            Debug.Log("we didnt get him :sob: BOIS");
        }
    }

    private IEnumerator WaitForCam()
    {
        yield return new WaitForSeconds(7);
        gameManager.GetComponent<WaveManager>().SwitchAttackPhase();
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

}

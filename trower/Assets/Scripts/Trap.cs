using System;
using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] GameObject trap;
    [SerializeField] float yOffset;
    [SerializeField] bool hasAbility;
    [SerializeField] bool isLarge;
    [SerializeField] float cooldown;
    [SerializeField] GameObject spaceEffect;
    [SerializeField] bool active;
    [SerializeField] bool canUse;
    [SerializeField] Sprite pressSpace;
    [SerializeField] Sprite pressedSpace;
    [SerializeField] GameObject armedSprite;
    [SerializeField] GameObject trapObj;
    [SerializeField] GameObject trapSprite;

    RectTransform cooldownTimerTransform;
    TrapManager trapManager;
    WaveManager waveManager;
    public event EventHandler onActivate;
    public event EventHandler onDeactivate;
    public event EventHandler onMouseLeave;
    public event EventHandler onCooldownOver;
    bool canAttack;
    bool defensePhase;
    bool attackPhase;
    bool armed;
    float cooldownCount;
    int objectID;
    Vector3 cooldownPos;

    private void Start()
    {
        spaceEffect.SetActive(false);
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        trapManager = gameManager.GetComponent<TrapManager>();
        trapManager.onSpacePressed += TrapActivated;
        trapManager.onSpaceReleased += TrapDeactivated;
        trapManager.AddTrapToList(gameObject, this);

        waveManager = gameManager.GetComponent<WaveManager>();
        waveManager.OnAttackPhaseStart += AttackPhase;
        waveManager.OnDefensePhaseStart += DefensePhase;
        defensePhase = waveManager.GetIsDefensePhase();
        attackPhase = waveManager.GetIsAttackPhase();

        objectID = gameObject.GetInstanceID();
        Debug.Log("gameObject parent " + gameObject + "id" + objectID);
    }

    private void DefensePhase(object sender, EventArgs e)
    {
        defensePhase = true;
        attackPhase = false;
        StopAllCoroutines();
    }
    private void AttackPhase(object sender, EventArgs e)
    {
        defensePhase = false;
        attackPhase = true;

        if (cooldownCount > 0)
        {
            StartCoroutine(Cooldown());
        }
        if(armed)
        {
            onActivate?.Invoke(objectID, EventArgs.Empty);
            armed = false;
            armedSprite.SetActive(false);
        }
    }

    public void ManualTrapActivate()
    {
        armedSprite.SetActive(false);
        armed = false;
        onActivate?.Invoke(objectID, EventArgs.Empty);
    }

    private void TrapActivated(object sender, EventArgs e)
    {
        Debug.Log("Trap activated!");
        spaceEffect.GetComponent<SpriteRenderer>().sprite = pressedSpace;
        if (!canAttack || !active || (!attackPhase && !defensePhase)) {
            Debug.Log("trap activation cancelled" + canAttack + active + attackPhase + defensePhase);
            return;
        }

        if (defensePhase && cooldownCount <= 0)
        {
            armed = !armed;
            armedSprite.SetActive(armed);
        }
        else
        {
            onActivate?.Invoke(objectID, EventArgs.Empty);
        }
    }
    private void TrapDeactivated(object sender, EventArgs e)
    {
        if(active)
        {
            Debug.Log("can attack CRIMINAL");
            spaceEffect.GetComponent<SpriteRenderer>().sprite = pressSpace;
            if (canAttack )
            {
                onDeactivate?.Invoke(objectID, EventArgs.Empty);
            }
  
        }

    }

    public void CooldownOn() //called by specific trap script, tells this script wether or not the trap is on or off 
    {
        if (cooldownCount <= 0)
        {
            canAttack = false;
            cooldownCount = cooldown;
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        if(defensePhase) { yield break; }
        yield return new WaitForSeconds(.01f);
        cooldownCount -= .01f;
        Debug.Log(cooldownCount + "COOLDOWN");
        if (cooldownCount <= 0)
        {
            onCooldownOver?.Invoke(objectID, EventArgs.Empty);
        }
        else
        {
            StartCoroutine(Cooldown());
        }

    }

    public float GetYOffset()
    {
        return yOffset;
    }

    public float GetCurrentCooldown()
    {
        return cooldownCount;
    }

    public float GetTotalCooldown()
    {
        return cooldown;
    }

    public void DisableTrap() //for when placement happens
    {
        Debug.Log("danny");
        active = false;
    }
    public void EnableTrap()//for when placement happens
    {
        Debug.Log("gonzales" + trapManager);

        active = true;
        //gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
    }
    public void Rotate()
    {
        trapObj.transform.localRotation = new Quaternion(0, 180, 0, 0);
        trapSprite.transform.localRotation = new Quaternion(0, 180, 0, 0);
        Debug.Log("fartfartafrt");
    }

    public float GetRotation()
    {
        return trapObj.transform.rotation.y;
    }
    public bool GetTrapSize()
    {
        return isLarge;
    }

    public bool GetIsArmed()
    {
        return armed;
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public GameObject GetSpaceEffect()
    {
        return spaceEffect;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            Debug.Log("MOUSECOLLIDED");
            spaceEffect.SetActive(true);
            if (active)
            {
                canAttack = true;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) //assigns this trap as the active trap so the cooldown element can move posisitons 
    {
        if (collision.tag == "Mouse" && active)
        {
            trapManager.SelectNewTrap(this);
        }
        if(collision.tag == "Bolt")
        {
            if (!active || (!attackPhase && !defensePhase))
            {
                Debug.Log("trap activation cancelled" + active + attackPhase + defensePhase);
                return;
            }
            if (defensePhase && cooldownCount <= 0)
            {
                armed = !armed;
                armedSprite.SetActive(armed);
            }
            else
            {
                onActivate?.Invoke(objectID, EventArgs.Empty);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            if(active)
            {
                trapManager.DeselectTrap(this.gameObject);
                canAttack = false;
                onMouseLeave?.Invoke(objectID, EventArgs.Empty);
            }
            spaceEffect.SetActive(false);
            Debug.Log("MOUSE LEFT");

        }
        if(collision.tag == "Bolt")
        {
            if(active)
            {
                onDeactivate?.Invoke(objectID, EventArgs.Empty);
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("BEW BYE!");
        if(trapManager == null) { return;}
        trapManager.onSpacePressed -= TrapActivated;
        trapManager.onSpaceReleased -= TrapDeactivated;
        waveManager.OnAttackPhaseStart -= AttackPhase;
        waveManager.OnDefensePhaseStart -= DefensePhase;
        Debug.Log("fartBalls" + gameObject + this);
        trapManager.RemoveTrapFromList(gameObject, this);

    }

}

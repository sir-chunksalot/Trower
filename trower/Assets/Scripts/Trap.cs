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
    [SerializeField] GameObject cardHolster;
    [SerializeField] GameObject spaceEffect;
    [SerializeField] bool active;
    [SerializeField] Sprite pressSpace;
    [SerializeField] Sprite pressedSpace;

    RectTransform cooldownTimerTransform;
    TrapManager trapManager;
    bool canAttack;
    bool onCooldown;
    float cooldownCount;
    Vector3 cooldownPos;

    private void Start()
    {
        spaceEffect.SetActive(false);
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        trapManager = gameManager.GetComponent<TrapManager>();
        trapManager.onSpacePressed += TrapActivated;
        trapManager.onSpaceReleased += TrapDeactivated;
        trapManager.AddTrapToList(this.gameObject);
    }

    private void TrapActivated(object sender, EventArgs e)
    {
        Debug.Log("tried to attack CRIMINAL");
        if (canAttack)
        {

            spaceEffect.GetComponent<SpriteRenderer>().sprite = pressedSpace;
        }

    }
    private void TrapDeactivated(object sender, EventArgs e)
    {
        Debug.Log("can attack CRIMINAL");
        spaceEffect.GetComponent<SpriteRenderer>().sprite = pressSpace;
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
        yield return new WaitForSeconds(.01f);
        cooldownCount -= .01f;
        Debug.Log(cooldownCount + "COOLDOWN");
        if (cooldownCount <= 0)
        {
            onCooldown = false;
        }
        else
        {
            StartCoroutine(Cooldown());
        }

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
        Debug.Log("gonzales");
        active = true;
    }
    public void Rotate()
    {
        gameObject.transform.Rotate(0, 180, 0);
        Debug.Log("fartfartafrt");
    }

    public float GetRotation()
    {
        return gameObject.transform.rotation.y;
    }
    public bool GetTrapSize()
    {
        return isLarge;
    }

    public float GetCost()
    {
        return cardHolster.GetComponent<CardHolsterGraphics>().GetCost();
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            Debug.Log("MOUSECOLLIDED");
            if (active)
            {
                spaceEffect.SetActive(true);
                canAttack = true;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            trapManager.SelectNewTrap(this.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            trapManager.DeselectTrap(this.gameObject);
            spaceEffect.SetActive(false);
            Debug.Log("MOUSE LEFT");
            canAttack = false;
        }
    }

}

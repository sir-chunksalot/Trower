using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] GameObject trap;
    [SerializeField] float yOffset;
    [SerializeField] bool hasAbility;
    [SerializeField] bool isLarge;
    [SerializeField] bool reloadTime;
    [SerializeField] GameObject cardHolster;
    [SerializeField] bool active;

    bool canAttack;

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
        if(collision.tag == "Mouse")
        {
            Debug.Log("MOUSECOLLIDED");
            if (active)
            {
                canAttack = true;
            }
        }
       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            Debug.Log("MOUSE LEFT");
            canAttack = false;
        }
    }

}

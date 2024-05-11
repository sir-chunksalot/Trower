using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] GameObject trap;
    [SerializeField] GameObject range;
    [SerializeField] SpriteRenderer baseSprite;
    [SerializeField] Animator idleAnim;
    [SerializeField] float yOffset;
    [SerializeField] bool hasAbility;
    [SerializeField] bool isLarge;
    [SerializeField] bool reloadTime;
    [SerializeField] GameObject cardHolster;

    private void Awake()
    {
        DisableTrap();
        gameObject.transform.position = gameObject.transform.position + (Vector3.up * yOffset);
    }
    public void DisableTrap()
    {
        Debug.Log("danny");
        range.SetActive(false);
    }
    public void EnableTrap()
    {
        Debug.Log("gonzales");
        range.SetActive(true);
    }
    public void Activate()
    {
        trap.SetActive(true);
        baseSprite.enabled = false;
        if (idleAnim != null) idleAnim.enabled = false;
    }

    public void Deactivate()
    {
        trap.SetActive(false);
        baseSprite.enabled = true;
        if (idleAnim != null) idleAnim.enabled = true;
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

}

using Array2DEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] Array2DBool trapSize;
    [SerializeField] float damageAmount;
    [SerializeField] Vector3 offset; //trap builder uses this
    [SerializeField] bool hasAbility;
    [SerializeField] bool isWallTrap;
    [SerializeField] float cooldown;
    [SerializeField] bool active;
    [SerializeField] bool canUse;
    [SerializeField] Sprite pressSpace;
    [SerializeField] Sprite pressedSpace;
    [SerializeField] GameObject armedSprite;
    [SerializeField] GameObject trapObj;
    [SerializeField] GameObject trapSprite;
    [SerializeField] Animator anim;

    RectTransform cooldownTimerTransform;
    TrapManager trapManager;
    WaveManager waveManager;
    StatusEffect statusEffect;

    float cooldownCount;
    int objectID;
    Vector3 cooldownPos;

    Tuple<int, int>[] trapSizeTuple;


    private void Start()
    {
        statusEffect = gameObject.GetComponent<StatusEffect>();
        objectID = gameObject.GetInstanceID();
        Debug.Log("gameObject parent " + gameObject + "id" + objectID);

        int xBounds = trapSize.GridSize.x;
        int yBounds = trapSize.GridSize.y;


        //sets trap size
        bool[,] trapBounds = trapSize.GetCells();
        if (xBounds % 2 == 0 || yBounds % 2 == 0)
        { //even number, if there is no center then we ignore the given parameters. i aint messin with all that
            trapBounds = new bool[3, 3];
            trapBounds[1, 1] = true;

        }

        List<Tuple<int, int>> takenCells = new List<Tuple<int, int>>();

        for (int y = yBounds - 1; y >= 0; y--)
        {

            for (int x = 0; x < xBounds; x++)
            {
                if (trapBounds[x, y]) { takenCells.Add(new Tuple<int, int>(x - 1, y - 1)); }
            }
        }

        trapSizeTuple = takenCells.ToArray();
    }

    public Tuple<int, int>[] GetTrapSize()
    {
        return trapSizeTuple;
    }

    public void UseTrap()
    {
        Debug.Log("used trap!");
        anim.SetBool("TrapActivated", true);
    }

    public void TryDamage(GameObject target)
    {
        IDamageable damage = target.GetComponent<IDamageable>();
        if (damage != null)
        {
            if (statusEffect != null)
            {
                statusEffect.ApplyStatusEffect(target);
            }
            damage.Damage(damageAmount);
        }
    }
    public Vector3 GetOffset()
    {
        return offset;
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
    }

    public bool GetIsFacingLeft()
    {
        return trapSprite.transform.rotation.y == 1;
    }

    public void Rotate(bool left)
    {
        if (left)
        {
            trapObj.transform.localRotation = new Quaternion(0, 180, 0, 0);
            trapSprite.transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            trapObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            trapSprite.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        Debug.Log("fartfartafrt");
    }

    public void Arm()
    {
        armedSprite.SetActive(true);
    }
    public void Disarm()
    {
        armedSprite.SetActive(false);
    }

    public Vector3 GetRotation()
    {
        return new Vector3(trapObj.transform.localEulerAngles.x, trapObj.transform.localEulerAngles.y, trapObj.transform.localEulerAngles.z);
    }
    public bool GetIsWallTrap()
    {
        return isWallTrap;
    }
}

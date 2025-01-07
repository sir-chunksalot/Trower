using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapManager : MonoBehaviour
{
    [SerializeField] GameObject spaceEffect;
    [SerializeField] Sprite pressedSpace;
    [SerializeField] Sprite releasedSpace;

    public event EventHandler onSpacePressed;
    public event EventHandler onSpaceReleased;
    public event EventHandler onSelectedTrapChange;
    private List<Trap> trapScripts;
    private GameObject selectedTrap;
    Trap trapScript;
    TurnManager turnManager;
    GameManager gameManager;
    SpriteRenderer spaceEffectSprite;

    GridSpace targetCell;
    List<Trap> targetTraps;

    private void Awake()
    {
        turnManager = gameObject.GetComponent<TurnManager>();
        gameManager = gameObject.GetComponent<GameManager>();
        turnManager.OnEnemiesEvaluated += UseTraps;
        spaceEffectSprite = spaceEffect.GetComponent<SpriteRenderer>();
        trapScripts = new List<Trap>();
        targetTraps = new List<Trap>();
    }

    private void UseTraps(object sender, EventArgs e)
    {
        Debug.Log("traps used");

        foreach (Trap trap in targetTraps)
        {
            trap.UseTrap();
            trap.Disarm();
        }
        targetTraps.Clear();
        turnManager.TrapsEvaluated();
    }
    private void Update()
    {
        Vector2 mousePos = gameManager.GetMousePos();
        GridSpace gridSpace = gameManager.GetClosestGridSpace();
        if (gridSpace == null) { targetCell = null; return; }

        if (Vector2.Distance(gridSpace.GetPos(), mousePos) < 5)
        {
            targetCell = gridSpace;
            Trap trap = targetCell.GetCurrentTrap();
            if (trap != null)
            {
                spaceEffect.SetActive(true);
                spaceEffect.transform.position = trap.transform.position;
            }
            else
            {
                spaceEffect.SetActive(false);
            }
        }
        else
        {
            targetCell = null;
        }
    }

    public void AddTrapToList(Trap trapScript)
    {
        Debug.Log("scrypt" + trapScript);
        trapScripts.Add(trapScript);
        Debug.Log("scrpyt" + trapScripts[0]);
    }
    public void RemoveTrapFromList(Trap trapScript)
    {
        Debug.Log("scrypt" + trapScript);
        if (trapScripts == null) { return; }
        if (trapScripts.Contains(trapScript))
        {
            trapScripts.Remove(trapScript);
        }
    }

    public GameObject GetSelectedTrap()
    {
        return selectedTrap;
    }

    public List<Trap> GetAllTrapScripts()
    {
        return trapScripts;
    }

    public void SelectNewTrap(Trap trap) //called by individual trap scripts
    {
        Debug.Log("select new trap");
        selectedTrap = trap.gameObject;
        trapScript = trap;
        float currentCooldown = trapScript.GetCurrentCooldown();
        float totalCooldown = trapScript.GetTotalCooldown();
        onSelectedTrapChange?.Invoke(gameObject, EventArgs.Empty);
    }

    public void DeselectTrap(GameObject trap) //called by individual trap scripts
    {
        if (selectedTrap == trap)
        {
            selectedTrap = null;
            onSelectedTrapChange?.Invoke(gameObject, EventArgs.Empty);
        }
    }

    public void ActivateTraps(InputAction.CallbackContext context) //called on space press
    {
        Debug.Log("sanja first");
        if (!turnManager.CanArmTraps()) { return; }
        if (context.performed)
        {
            spaceEffectSprite.sprite = pressedSpace;
            if (targetCell != null)
            {
                Trap trap = targetCell.GetCurrentTrap();
                if (trap == null) { return; }
                targetTraps.Add(trap);
                trap.Arm();
            }
            //onSpacePressed?.Invoke(gameObject, EventArgs.Empty);
        }
        if (context.canceled)
        {
            spaceEffectSprite.sprite = releasedSpace;
            //onSpaceReleased?.Invoke(gameObject, EventArgs.Empty);
        }
    }



}

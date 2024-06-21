using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TrapManager : MonoBehaviour
{
    [SerializeField] GameObject trapCooldown;
    public event EventHandler onSpacePressed;
    public event EventHandler onSpaceReleased;
    public event EventHandler onSelectedTrapChange;
    private List<GameObject> traps;
    private GameObject selectedTrap;
    Trap trapScript;
    Image trapCooldownUI;
    private void Awake()
    {
        traps = new List<GameObject>();
        trapCooldownUI = trapCooldown.GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        if (selectedTrap != null)
        {
            float currentCooldown = trapScript.GetCurrentCooldown();
            float totalCooldown = trapScript.GetTotalCooldown();
            trapCooldownUI.fillAmount = currentCooldown / totalCooldown;
        }
    }


    public void AddTrapToList(GameObject trap)
    {
        traps.Add(trap);
    }

    public GameObject GetSelectedTrap()
    {
        return selectedTrap;
    }

    public void SelectNewTrap(GameObject trap)
    {
        selectedTrap = trap;
        trapScript = selectedTrap.GetComponent<Trap>();
        onSelectedTrapChange?.Invoke(gameObject, EventArgs.Empty);
    }

    public void DeselectTrap(GameObject trap)
    {
        if (selectedTrap == trap)
        {
            selectedTrap = null;
            trapCooldownUI.fillAmount = 0;
            onSelectedTrapChange?.Invoke(gameObject, EventArgs.Empty);
        }
    }

    public void ActivateTraps(InputAction.CallbackContext context)
    {
        Debug.Log("sanja first");
        if (context.performed)
        {
            onSpacePressed?.Invoke(gameObject, EventArgs.Empty);
        }
        if (context.canceled)
        {
            onSpaceReleased?.Invoke(gameObject, EventArgs.Empty);
        }
    }



}

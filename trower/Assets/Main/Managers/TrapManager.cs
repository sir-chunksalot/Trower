using System;
using System.Collections;
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
    private List<Trap> trapScripts;
    private GameObject selectedTrap;
    Trap trapScript;
    WaveManager waveManager;
    Image trapCooldownUI;
    private void Awake()
    {
        waveManager = gameObject.GetComponent<WaveManager>();
        waveManager.OnAttackPhaseStart += FindSelectedTrap;
        traps = new List<GameObject>();
        trapScripts = new List<Trap>();
        if (trapCooldown != null)
        {
            trapCooldownUI = trapCooldown.GetComponent<Image>();
        }
    }

    private void FindSelectedTrap(object sender, EventArgs e)
    {
        StartCoroutine(CheckForSelect());
    }

    private IEnumerator CheckForSelect()
    {
        yield return new WaitForSeconds(.5f);

        if (selectedTrap == null)
        {
            foreach (Trap trap in trapScripts)
            {
                Debug.Log("FARTPPselect trap loop" + trap);
                if (trap.GetCanAttack())
                {
                    Debug.Log("select trap found" + trap);
                    SelectNewTrap(trap);
                    break;
                }
            }
        }

    }

    public GameObject GetCooldownTimer()
    {
        return trapCooldown;
    }

    private void FixedUpdate()
    {
        if (selectedTrap != null)
        {
            Debug.Log("selectedd trap:" + selectedTrap);
            float currentCooldown = trapScript.GetCurrentCooldown();
            float totalCooldown = trapScript.GetTotalCooldown();
            UpdateTrapCooldownUI(currentCooldown / totalCooldown);
        }
    }


    public void AddTrapToList(GameObject trap, Trap trapScript)
    {
        Debug.Log("selectballs" + trap + "scrypt" + trapScript);
        traps.Add(trap);
        trapScripts.Add(trapScript);
        Debug.Log("selectBalls2" + traps[0] + "scrpyt" + trapScripts[0]);
    }
    public void RemoveTrapFromList(GameObject trap, Trap trapScript)
    {
        Debug.Log("selectpenis" + trap + "scrypt" + trapScript);
        if (traps == null || trapScripts == null) { return; }
        if (traps.Contains(trap))
        {
            traps.Remove(trap);
        }
        if (trapScripts.Contains(trapScript))
        {
            trapScripts.Remove(trapScript);
        }
    }

    public GameObject GetSelectedTrap()
    {
        return selectedTrap;
    }


    public List<GameObject> GetAllTraps()
    {
        return traps;
    }
    public List<Trap> GetAllTrapScripts()
    {
        return trapScripts;
    }

    private void UpdateTrapCooldownUI(float fillAmount)
    {
        if (trapCooldownUI == null)
        {
            return;
        }
        trapCooldownUI.fillAmount = fillAmount;
    }


    public void SelectNewTrap(Trap trap)
    {
        Debug.Log("select new trap");
        selectedTrap = trap.gameObject;
        trapScript = trap;
        float currentCooldown = trapScript.GetCurrentCooldown();
        float totalCooldown = trapScript.GetTotalCooldown();
        UpdateTrapCooldownUI(currentCooldown / totalCooldown);
        onSelectedTrapChange?.Invoke(gameObject, EventArgs.Empty);
    }

    public void DeselectTrap(GameObject trap)
    {
        if (selectedTrap == trap)
        {
            selectedTrap = null;
            UpdateTrapCooldownUI(0);
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

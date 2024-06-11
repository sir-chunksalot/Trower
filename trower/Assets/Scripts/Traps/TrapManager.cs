using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapManager : MonoBehaviour
{
    public event EventHandler onActivateTraps;
    
    public void ActivateTraps(InputAction.CallbackContext context)
    {
        Debug.Log("sanja first");
        if (context.performed)
        {
            onActivateTraps?.Invoke(gameObject, EventArgs.Empty);
        }
    }
}

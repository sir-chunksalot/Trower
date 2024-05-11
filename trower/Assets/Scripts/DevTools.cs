using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevTools : MonoBehaviour
{
    public void ShowViableFloors(InputAction.CallbackContext context)
    {
        GenerateViableFloors genViableFloors = gameObject.GetComponent<GenerateViableFloors>();
        genViableFloors.Test();
    }
}

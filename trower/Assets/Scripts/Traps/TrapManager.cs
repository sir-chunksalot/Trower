using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapManager : MonoBehaviour
{
    //traps
    [SerializeField] GameObject door;
    [SerializeField] GameObject fireSpitter;
    [SerializeField] GameObject spears;
    [SerializeField] GameObject spider;
    [SerializeField] GameObject fakeMouse;
    [SerializeField] float bloodChargeRate;
    [SerializeField] float startBloodCharge;

    HeroSpawner heroSpawner;
    UIManager uiManager;
    public event EventHandler onActivateTraps;
    public event EventHandler onDeactivateTraps;

    void Start()
    {
        uiManager = gameObject.GetComponent<UIManager>();

        heroSpawner = gameObject.GetComponent<HeroSpawner>();
    }
    public void ActivateTraps(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            if (context.control.name.Equals("a"))
            {
                onActivateTraps?.Invoke("left", EventArgs.Empty);
            }
            if (context.control.name.Equals("d"))
            {
                onActivateTraps?.Invoke("right", EventArgs.Empty);
            }
        }

        if (context.canceled)
        {
            if (context.control.name.Equals("a"))
            {
                onDeactivateTraps?.Invoke("left", EventArgs.Empty);
            }
            if (context.control.name.Equals("d"))
            {
                onDeactivateTraps?.Invoke("right", EventArgs.Empty);
            }

        }
    }


    private void FixedUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        fakeMouse.transform.position = new Vector3(mousePos.x, mousePos.y, 10);
    }

    private void MoveCamera(Vector3 dir)
    {
        Camera.main.transform.position += dir * 30;
    }
}

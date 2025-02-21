using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] Trap papaTrap;
    [SerializeField] bool topEntry;
    [SerializeField] bool rightEntry;
    [SerializeField] bool leftEntry;
    [SerializeField] bool bottomEntry;

    [SerializeField] bool topExit;
    [SerializeField] bool rightExit;
    [SerializeField] bool leftExit;
    [SerializeField] bool bottomExit;

    private void Start()
    {
        if(papaTrap == null) { papaTrap = transform.GetComponentInParent<Trap>(); }
    }

    public Trap GetTrapDaddy()
    {
        return papaTrap;
    }

    public void RotateSensor()
    {
        bool tempLeft = leftEntry;
        bool tempRight = rightEntry;

        leftEntry = tempRight;
        rightEntry = tempLeft;

        tempLeft = leftExit;
        tempRight = rightExit;

        leftExit = tempRight;
        rightExit = tempLeft;
    }

    public bool TriggerSensor(string moveType)
    {
        bool enteringTop = false;
        bool enteringRight = false;
        bool enteringLeft = false;
        bool enteringBottom = false;

        if(moveType == "enter_top") { enteringTop = true; }
        if(moveType == "enter_right") { enteringRight = true; }
        if(moveType == "enter_left") { enteringLeft = true; }
        if(moveType == "enter_bottom") { enteringBottom = true; }

        if((enteringTop && topEntry) || (enteringRight && rightEntry) || (enteringLeft && leftEntry) || (enteringBottom || bottomEntry))
        {
            papaTrap.UseTrap();
            return true;
        }



        bool exitingTop = false;
        bool exitingRight = false;
        bool exitingLeft = false;
        bool exitingBottom = false;

        if (moveType == "exit_top") { exitingTop = true; }
        if (moveType == "exit_right") { exitingRight = true; }
        if (moveType == "exit_left") { exitingLeft = true; }
        if (moveType == "exit_bottom") { exitingBottom = true; }

        if ((exitingTop && topExit) || (exitingRight && rightExit) || (exitingLeft && leftExit) || (exitingBottom || bottomExit))
        {
            papaTrap.UseTrap();
            return true;
        }

        return false;

    }
}

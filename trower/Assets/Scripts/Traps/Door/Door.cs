using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    [SerializeField] object test;
    [SerializeField] int tes2t;

    Smack smackLeft;
    Smack smackRight;

    [SerializeField] float doorOpenTime;
    void Start()
    {
        smackLeft = leftDoor.GetComponent<Smack>();
        smackRight = rightDoor.GetComponent<Smack>();
    }

    //public void Activate(bool isRight, bool buttonRelease = false)
    //{
    //    if (isRight)
    //    {
    //        if (buttonRelease)
    //        {
    //            smackRight.UseDoor(false);
    //        }
    //        else
    //        {
    //            smackRight.UseDoor(true);
    //        }

    //    }
    //    else
    //    {
    //        if (buttonRelease)
    //        {
    //            smackLeft.UseDoor(false);
    //        }
    //        else
    //        {
    //            smackLeft.UseDoor(true);
    //        }

    //    }
    //}
}

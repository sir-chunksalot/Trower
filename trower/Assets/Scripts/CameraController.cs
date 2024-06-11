using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[SerializeField] Camera cam;
    //[SerializeField] float shakeAmount;
    //[SerializeField] float decreaseFactor;
    [SerializeField] List<GameObject> vCams;
    [SerializeField] List<float> times; //leave blank if you dont want them to transistion automatically


    private void Start()
    {
        int count = 0;
        foreach(float time in times)
        {
            if(time != 0)
            {
                //StartCoroutine(AutoCameraTransistion(time, vCams[count]));
                //for testing uncomment later
            }
            count++;
        }
    }
    private IEnumerator AutoCameraTransistion(float time, GameObject cam)
    {
        yield return new WaitForSeconds(time);
        ActivateCamera(cam);
    }

    public void ActivateCamera(GameObject cam)
    {
        cam.GetComponent<CinemachineVirtualCamera>().Priority = 1;
        foreach (GameObject vcam in vCams)
        {
            if(vcam != cam)
            {
                vcam.GetComponent<CinemachineVirtualCamera>().Priority = 0;
            }
            
        }
    }

    class CamTransistions
    {
        public float time;
        public GameObject vCams;
    }
    //private float shake;
    //private Vector3 cameraPos;

    //private void Update()
    //{
    //    if (shake > 0)
    //    {
    //        Debug.Log("shaking" + shake);
    //        Vector2 newPos = Random.insideUnitSphere * (shakeAmount * shake);
    //        newPos.y = newPos.y + cameraPos.y;
    //        cam.transform.localPosition = newPos;
    //        shake -= (Time.deltaTime * Time.deltaTime) * decreaseFactor;
    //    }
    //    else
    //    {
    //        shake = 0.0f;
    //        //gameObject.transform.position = cameraPos;
    //    }
    //}

    //public void AddShake(float value)
    //{
    //    shake += value;
    //}

    //public void SetCameraPos(Vector3 newPos)
    //{
    //    cameraPos = newPos;
    //}


    //[SerializeField] float newY;
    //[SerializeField] float speed;
    //private float decreaseFactor;
    //private float vcamYOffset;
    //private float vcamXOffset;
    //private void Start()
    //{
    //    CinemachineVirtualCamera vCam = GetComponent<CinemachineVirtualCamera>();
    //    vcamYOffset = vCam.GetCinemachineComponent<CinemachineCameraOffset>();
    //    vcamYOffset = GetComponent<CinemachineCameraOffset>().transform.position.y;
    //    vcamXOffset = GetComponent<CinemachineCameraOffset>().transform.position.x;
    //    MoveCameraUp(newY, speed);
    //}

    //private void MoveCameraUp(float amount, float speed)
    //{
    //    decreaseFactor = amount * speed;

    //}

    //private void Update()
    //{
    //    if(newY > vcamYOffset)
    //    {
    //        if(vcamYOffset + Time.deltaTime * decreaseFactor >= newY)
    //        {
    //            vcamYOffset = newY;
    //        }
    //        else
    //        {
    //            vcamYOffset += Time.deltaTime * decreaseFactor;
    //        }
    //        GetComponent<CinemachineCameraOffset>().transform.position = new Vector2(vcamXOffset, vcamYOffset);

    //    }
    //}
}

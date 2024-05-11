using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float shakeAmount;
    [SerializeField] float decreaseFactor;

    private float shake;
    private Vector3 cameraPos;

    private void Update()
    {
        if (shake > 0)
        {
            Debug.Log("shaking" + shake);
            Vector2 newPos = Random.insideUnitSphere * (shakeAmount * shake);
            newPos.y = newPos.y + cameraPos.y;
            cam.transform.localPosition = newPos;
            shake -= (Time.deltaTime * Time.deltaTime) * decreaseFactor;
        }
        else
        {
            shake = 0.0f;
            gameObject.transform.position = cameraPos;
        }
    }

    public void AddShake(float value)
    {
        shake += value;
    }

    public void SetCameraPos(Vector3 newPos)
    {
        cameraPos = newPos;
    }
}

using UnityEngine;

public class FireSpitters : MonoBehaviour
{
    [SerializeField] GameObject fire;
    private void OnEnable()
    {
        fire.SetActive(true);
    }

    private void OnDisable()
    {
        fire.SetActive(false);
    }

}

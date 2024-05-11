using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] GameObject webRight;
    [SerializeField] GameObject webLeft;
    [SerializeField] Animator anim;
    private void OnEnable()
    {
        anim.SetBool("ShootWeb", true);
    }

    public void ShootWebs()
    {
        webRight.SetActive(true);
        webLeft.SetActive(true);
    }

    private void OnDisable()
    {
        webRight.SetActive(false);
        webLeft.SetActive(false);
    }
}

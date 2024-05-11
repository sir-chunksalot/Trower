using System.Collections;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    [SerializeField] float shootDelay;
    [SerializeField] Animator anim;
    Vector3 spawnPos;

    private void Start()
    {
        spawnPos = new Vector3(gameObject.transform.position.x, 1.5f + gameObject.transform.position.y, gameObject.transform.position.z);
    }

    private void OnEnable()
    {
        StartCoroutine(ShootDelay());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void Shoot() //called by animation event
    {
        Instantiate(arrow, spawnPos, gameObject.GetComponentInParent<Transform>().rotation);
        anim.enabled = false;
    }

    private IEnumerator ShootDelay()
    {
        PlayAnimation();
        yield return new WaitForSeconds(shootDelay);
        StartCoroutine(ShootDelay());
    }

    private void PlayAnimation()
    {
        anim.enabled = true;
    }
}

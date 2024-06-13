using System.Collections;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    [SerializeField] GameObject bloodAnim;
    [SerializeField] GameObject bloodEffect;
    [SerializeField] Sprite[] bloodEffects;
    public void KillHero(GameObject hero)
    {
        Debug.Log("hero is being killed :sob:");
        Vector3 spawnPos = hero.transform.position;
        Destroy(hero);
        GameObject blood = Instantiate(bloodAnim, spawnPos, Quaternion.identity, gameObject.transform);
        StartCoroutine(BloodAnim(blood, spawnPos));
    }

    private IEnumerator BloodAnim(GameObject bloodAnim, Vector3 spawnPos)
    {
        yield return new WaitForSeconds(.4f);
        Destroy(bloodAnim);
        GameObject newBlood = Instantiate(bloodEffect, spawnPos, Quaternion.identity, gameObject.transform);
        newBlood.GetComponent<SpriteRenderer>().sprite = PickBloodEffect();
    }

    private Sprite PickBloodEffect()
    {
        int count = bloodEffects.Length - 1;
        int randomNum = Random.Range(0, count);
        return bloodEffects[randomNum];
    }
}

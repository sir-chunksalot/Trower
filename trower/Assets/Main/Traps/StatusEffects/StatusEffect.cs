using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] GameObject particleEffect;
    [SerializeField] bool isFlame;
    [SerializeField] bool isSticky;
    [SerializeField] float damage;
    [SerializeField] float duration;
    [SerializeField] float damageInterval;
    public void ApplyStatusEffect(GameObject target)
    {
        if(isFlame)
        {
            IBurnable burn = target.GetComponent<IBurnable>();
            if(burn != null) { 
                burn.Burn(duration, damage, damageInterval);
                Vector3 spawnPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - 1);
                GameObject flameEffect = Instantiate(particleEffect, spawnPos, Quaternion.identity, target.transform);
                flameEffect.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1;
                flameEffect.GetComponent<DestroyAfterTime>().DestroyMeAfter(duration);
            }
        }
    }
}

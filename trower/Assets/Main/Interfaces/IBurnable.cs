using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBurnable
{
    void Burn(float length, float damage, float damageInterval);
}

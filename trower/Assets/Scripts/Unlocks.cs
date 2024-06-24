using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Unlocks
{
    static List<GameObject> unlockedTraps;
    static bool established;
    public static void UnlockTrap(GameObject trap)
    {
        if (!established) { 
            established = true;
            unlockedTraps = new List<GameObject>();
        }

        unlockedTraps.Add(trap);
    }

    public static List<GameObject> GetUnlockedTraps()
    {
        return unlockedTraps;
    }

    public static void LogAllUnlocks()
    {
        string unlocks = "";
        foreach(GameObject traps in unlockedTraps)
        {
            unlocks += traps.name + ", ";
        }
        Debug.Log("Unlocked Traps: " + unlocks);
    }

}

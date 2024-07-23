using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnlockManager
{
    public static event EventHandler OnUnlock;

    public static List<string> unlocks;
    static bool listExists;
    public static void Unlock(string item)
    {
        if (!listExists)
        {
            listExists = true;
            unlocks = new List<string>();
        }

        unlocks.Add(item);

        OnUnlock?.Invoke(item, EventArgs.Empty);
        Debug.Log("Unlock event fired.");
    }

    public static List<string> GetUnlockedItems()
    {
        if (!listExists)
        {
            listExists = true;
            unlocks = new List<string>();
        }
        return unlocks;
    }

    public static bool IsItemUnlocked(string name)
    {
        if (unlocks == null) { return false; }
        foreach (string item in unlocks)
        {
            if (item == name)
            {
                return true;
            }
        }
        return false;
    }

    public static void LogAllUnlocks()
    {
        string unlocksString  = "";
        foreach (string items in unlocks)
        {
            unlocksString += items + ", ";
        }
        Debug.Log("Unlocked items: " + unlocksString);
    }
}

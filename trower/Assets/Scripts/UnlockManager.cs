using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public event EventHandler OnUnlock;

    List<string> unlocks;
    bool listExists;
    public void Unlock(string item)
    {
        if (!listExists)
        {
            listExists = true;
            unlocks = new List<string>();
        }

        unlocks.Add(item);

        OnUnlock?.Invoke(item, EventArgs.Empty);
    }

    public List<string> GetUnlockedItems()
    {
        return unlocks;
    }

    public bool IsItemUnlocked(string name)
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

    public void LogAllUnlocks()
    {
        string unlocksString  = "";
        foreach (string items in unlocks)
        {
            unlocksString += items + ", ";
        }
        Debug.Log("Unlocked items: " + unlocksString);
    }
}

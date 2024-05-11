using System;
using UnityEngine;

public static class BuildingResources
{
    private static float resources;
    public static event EventHandler onChangeResources;
    public static float GetResources()
    {
        return resources;
    }

    public static void ChangeResources(float count)
    {
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIManager>().isActiveAndEnabled)
        {
            resources += count;
            if (Mathf.Sign(count) == 1)
            {
                onChangeResources?.Invoke(("+" + count + "|"), EventArgs.Empty);
            }
            else
            {
                onChangeResources?.Invoke((count + "|"), EventArgs.Empty);
            }
        }
    }

    public static void SetResourceCount(float count)
    {
        ChangeResources(count - resources);
    }
}

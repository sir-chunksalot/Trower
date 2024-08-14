using System;
using UnityEngine;

public static class Coins
{
    private static float coins;
    public static event EventHandler onChangeCoin;

    private static float material;
    public static event EventHandler onChangeMaterial;
    public static float GetCoins()
    {
        return coins;
    }

    public static void ChangeCoins(float count)
    {
        Debug.Log("INVOKING COIN");
        coins += count;
        if (Mathf.Sign(count) == 1)
        {
            Debug.Log("INVOKING COIN ");
            onChangeCoin?.Invoke(("+" + count + "|"), EventArgs.Empty);
        }
        else
        {
            Debug.Log("INVOKING COIN");
            onChangeCoin?.Invoke(("-" + count + "|"), EventArgs.Empty);
        }
    }

    public static void SetCoin(float count)
    {
        coins = count;
        onChangeCoin?.Invoke(("+" + count + "|"), EventArgs.Empty);
    }






    public static float GetMaterial()
    {
        return material;
    }

    public static void ChangeMaterial(float count)
    {
        Debug.Log("INVOKING MATERIAL");
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIManager>().isActiveAndEnabled)
        {
            material += count;
            Debug.Log("INVOKING MATERIAL");
            onChangeMaterial?.Invoke((material), EventArgs.Empty);
        }
    }
    public static void SetMaterial(float count)
    {
        material = count;
        onChangeMaterial?.Invoke((count), EventArgs.Empty);
    }
}


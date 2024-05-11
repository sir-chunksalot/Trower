using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private float cost;

    public float GetCost()
    {
        return cost;
    }
}

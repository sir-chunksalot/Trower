using UnityEngine;

public class EnemyWallCollision : MonoBehaviour
{
    public void TurnOn()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
}

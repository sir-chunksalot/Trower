using UnityEngine;

public class HeroFeet : MonoBehaviour
{
    bool isTouchingGround;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("collision FART ATTORNEY" + collision.gameObject);
        if (collision.gameObject.tag == "EnemyFloor")
        {
            Debug.Log("POOP ATTORNEY");
            isTouchingGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("EXIT FART ATTORNEY");
        if (collision.gameObject.tag == "EnemyFloor")
        {
            isTouchingGround = false;
        }
    }


    public bool GetIsTouchingGround()
    {
        return isTouchingGround;
    }
}

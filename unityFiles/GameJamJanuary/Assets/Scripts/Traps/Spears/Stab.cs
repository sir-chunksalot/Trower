using UnityEngine;

public class Stab : MonoBehaviour
{
    Collider2D col;
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] Animator anim;

    CameraController cam;
    private void Start()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        //collision.gameObject.GetComponent<Hero>().ChangeHealth(-10, false);
        //cam.AddShake(1);

        anim.Play("spera");
    }

    public void SpearUp() //this is to move spear collider up along with spears
    {
        Debug.Log("FART");
        col.offset = new Vector3(offsetX, offsetY + 3);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class FakeMouse : MonoBehaviour
{
    /// <summary>
    /// PUT A PIN IN THIS SCRIPT. ITS A GOOD IDEA AND THE CODE WORKS BUT THE ART AINT THERE YET. ONCE I HAVE A GOOD POINTER SPRITE I WILL COME BACK TO THIS
    /// </summary>




    //this script is located fake mouse UI object inside game manager UI
    [SerializeField] private GameObject fakeMouse;
    [SerializeField] Canvas parentCanvas;
    [SerializeField] RawImage mouseCursor;
    [SerializeField] Texture baseMouse;
    [SerializeField] Texture discoverMouse;
    [SerializeField] GameObject mouseHitbox;



    public void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
    }


    public void Update()
    {
        Vector2 movePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);

        Vector3 mousePos = parentCanvas.transform.TransformPoint(movePos);

        //Set fake mouse Cursor
        mouseCursor.transform.position = mousePos;
        mouseHitbox.transform.position = movePos;


        RectTransform mouseCursorTransform = mouseCursor.GetComponent<RectTransform>();
        Transform mouseHitboxTransform = mouseHitbox.GetComponent<Transform>();

        Vector3 pos = Camera.main.ScreenToWorldPoint(mouseCursorTransform.position);
        pos = new Vector3(pos.x, pos.y, -10);
        mouseHitboxTransform.position = pos;
    }

    public void SwitchCursor(string cursor)
    {
        if (cursor == "base")
        {
            mouseCursor.texture = baseMouse;
        }
        if (cursor == "discover")
        {
            mouseCursor.texture = discoverMouse;
        }
    }

}

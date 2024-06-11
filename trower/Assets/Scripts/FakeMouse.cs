using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeMouse : MonoBehaviour
{
    //this script is located inside game manager, it simply controls the small mage hand that follows the mouse
    [SerializeField] private GameObject fakeMouse;
    [SerializeField] Canvas parentCanvas;
    [SerializeField] RawImage mouseCursor;
    [SerializeField] Texture baseMouse;
    [SerializeField] Texture discoverMouse;
    [SerializeField] GameObject mouseHitbox;



    public void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
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
        if(cursor == "base")
        {
            mouseCursor.texture = baseMouse;
        }
        if(cursor == "discover")
        {
            mouseCursor.texture = discoverMouse;
        }
    }

}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class GUIClickController : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onLeft;
    public UnityEvent onRight;
    public UnityEvent onMiddle;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onLeft.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRight.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            onMiddle.Invoke();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public event EventHandler OnCursorEnter;
    public event EventHandler OnCursorExit;
    public event EventHandler OnCursorClick;
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnCursorEnter?.Invoke(gameObject, EventArgs.Empty);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnCursorExit?.Invoke(gameObject, EventArgs.Empty);
    }

}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public UnityEvent _onButtonDown;
    public UnityEvent _onButtonUp;
    public UnityEvent onButtonExit;
    public UnityEvent onButtonEnter;
    public UnityEvent onButtonStay;
    // 当按钮被按下时调用
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (_onButtonDown != null)
        {
            _onButtonDown.Invoke();
        }
    }

    // 当按钮被抬起时调用
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (_onButtonUp != null)
        {
            _onButtonUp.Invoke();
        }
    }

    // 当鼠标从按钮上离开时调用
    public override void OnPointerExit(PointerEventData eventData)
    {
        //base.OnPointerExit(eventData);
        if (onButtonExit != null)
        {
            onButtonExit.Invoke();
        }
    }

    // 当鼠标从外面进入到按钮上方时调用
    public override void OnPointerEnter(PointerEventData eventData)
    {
        //base.OnPointerEnter(eventData);
        if (onButtonEnter != null)
        {
            onButtonEnter.Invoke();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (onButtonStay != null)
        {
            onButtonStay.Invoke();
        }
    }
}

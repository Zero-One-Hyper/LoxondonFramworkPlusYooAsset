using System;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.UI;

public class StylusEventReceive : MonoBehaviour
{
    private IEventHandleService _eventService;

    void Awake()
    {
        _eventService = Context.GetApplicationContext().GetService<IEventHandleService>();
        _eventService.AddListener(StylusEvent.Enter, OnPointEnter);
        _eventService.AddListener(StylusEvent.Exit, OnPointExit);
        _eventService.AddListener(StylusEvent.Press, OnPointPress);
        _eventService.AddListener(StylusEvent.Release, OnPointRelease);

        #region 以下为多种应用场景的演示

        _eventService.AddListener(StylusEvent.Enter, OnPointEnter); //演示重复添加
        _eventService.AddListener(StylusEvent.Exit, OnPointExitAddition); //演示叠加添加
        _eventService.AddListener(StylusEvent.Press, OnPointPressAddition); //叠加事件，用于演示DelLitener(Enum _type)
        _eventService.DelListener(StylusEvent.Exit, NoRegisterEvent); //演示移除未注册的事件
        _eventService.DelListener(NoRegisterEventAndNoEventTypeAssigned); //演示不指定EventType移除未注册的事件

        #endregion
    }

    private void NoRegisterEventAndNoEventTypeAssigned(EventArgs obj)
    {
    }

    private void OnPointPressAddition(EventArgs obj)
    {
        XLog.I("OnPointPressAddition ---Press [D] will remove it as all of the press event removed");
    }

    private void NoRegisterEvent(EventArgs obj)
    {
    }

    private void OnPointExitAddition(EventArgs obj)
    {
        XLog.I("OnPointExitAddition ~ Press 【Q】 Remove This Listener");
    }

    private void OnPointPress(EventArgs obj)
    {
        StylusEventArgs args = obj as StylusEventArgs;
        SetColor(args.Selected, Color.green);
        XLog.I("鼠标按下--selected:{0},buttonID:{1}", args.Selected.name, args.ButtonID);
    }

    private void OnPointRelease(EventArgs obj)
    {
        StylusEventArgs args = obj as StylusEventArgs;
        SetColor(args.Selected, Color.white);
        XLog.I("鼠标释放--selected:{0},buttonID:{1}", args.Selected.name, args.ButtonID);
    }

    private void OnPointExit(EventArgs obj)
    {
        StylusEventArgs args = obj as StylusEventArgs;
        SetColor(args.Selected, Color.white);
        XLog.I("光标退出--selected:{0},buttonID:{1}", args.Selected.name, args.ButtonID);
    }

    private void OnPointEnter(EventArgs obj)
    {
        StylusEventArgs args = obj as StylusEventArgs;
        SetColor(args.Selected, Color.red);
        XLog.I("光标进入--selected:{0},buttonID:{1}---Press 【R】 Remove This Listener", args.Selected.name, args.ButtonID);
    }

    public void SetColor(GameObject obj, Color color)
    {
        obj.GetComponent<MeshRenderer>().material.color = color;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            XLog.I("R");
            _eventService.DelListener(OnPointEnter); //不怎么建议使用，因为涉及到迭代字典并修改数据的需求，曲线救国未考虑性能问题。
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            XLog.I("Q");
            _eventService.DelListener(StylusEvent.Exit, OnPointExitAddition);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            XLog.I("D");
            _eventService.DelListener(StylusEvent.Press);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            XLog.I("A");
            _eventService.RemoveAllListener();
        }
    }

    //建议在此处写上移除指定的事件，但不建议移除全部哈
    void OnDestroy()
    {
        _eventService.DelListener(StylusEvent.Enter, OnPointEnter); //移除时，可以指定事件类型
        _eventService.DelListener(OnPointExit); //移除时也可以不指定事件类型
        _eventService.RemoveAllListener(); //可以使用该方法全部移除
    }
}
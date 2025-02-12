using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class CameraEventDispatch: MonoBase,ICameraEventDispatch
{
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="v3"></param>
    public void CameraMove(Vector3 v3)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<CameraEventArgs>()
            .Register(CameraEvent.Move,gameObject, v3)
            .Invoke();
    }
    
    /// <summary>
    /// 旋转
    /// </summary>
    /// <param name="v3"></param>
    public void CameraRotate(Vector3 v3)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<CameraEventArgs>()
            .Register(CameraEvent.Rotate,gameObject, v3)
            .Invoke();
    }
    
    /// <summary>
    /// 缩放
    /// </summary>
    /// <param name="v3"></param>
    public void CameraZoom(Vector3 v3)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<CameraEventArgs>()
            .Register(CameraEvent.Zoom,gameObject, v3)
            .Invoke();
    }
}

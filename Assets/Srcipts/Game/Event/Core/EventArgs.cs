using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 事件信息基类
/// </summary>
public abstract class EventArgs :IDisposable
{
    public Enum EventType { protected set; get; }
    public GameObject Sender { protected set; get; }

    public virtual void Register(Enum t, GameObject sender)
    {
        EventType = t;
        Sender = sender;
    }
    
    public virtual void Register(Enum t)
    {
        EventType = t;
    }

    /// <summary>
    /// 事件信息类倍回收时调用
    /// </summary>
    public virtual void Dispose()
    {
        this.Sender = null;
    }
}

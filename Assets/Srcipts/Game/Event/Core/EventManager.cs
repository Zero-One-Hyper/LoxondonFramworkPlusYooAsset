using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
   public class EventManager: IEventHandleService
{
    public Dictionary<Type, Queue<EventArgs>> Recycled { get; set; }
    public int Capacity { get; set; }
    public bool Interrupt { get; set; }

    /// <summary>
    /// 事件链
    /// </summary>
    private Dictionary<Enum, Action<EventArgs>> _listeners = null;

    public EventManager()
    {
        Init();
    }
    
    public void Init()
    {
        this.Capacity = 60;
        InitEvent();
    }

    #region 私有方法
    /// <summary>
    /// 初始化事件链
    /// </summary>
    private void InitEvent()
    {
        Recycled = new Dictionary<Type, Queue<EventArgs>>();
        _listeners = new Dictionary<Enum, Action<EventArgs>>();
    }
    
    /// <summary>
    /// 得到指定枚举项的所有事件链
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private Action<EventArgs> GetEventList(Enum type)
    {
        _listeners.TryAdd(type, default(Action<EventArgs>));
        return _listeners[type];
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    private void AddEvent(Enum type, Action<EventArgs> action)
    {
        var actions = GetEventList(type);
        if (null != action)
        {
            Delegate[] delegates = actions?.GetInvocationList();
            if (null != delegates)
            {
                if (!Array.Exists(delegates, rt => rt == (Delegate)action))
                {
                    actions += action;
                }
                else
                {
                    XLog.E($"重复的事件监听:{type.GetType().Name}.{type.ToString()}");
                }
            }
            else
            {
                actions = action;
            }

            _listeners[type] = actions;
        }
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    /// <param name="args"></param>
    void CallEvent(EventArgs args)
    {
        var actions = GetEventList(args.EventType);
        actions?.Invoke(args);
        Recycle(args);
    }

    /// <summary>
    /// 删除指定事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    void DelEvent(Enum type, Action<EventArgs> action)
    {
        if (null != action)
        {
            var actions = GetEventList(type);
            if (null != action)
            {
                actions -= action;
            }

            _listeners[type] = actions;
        }
        else
        {
            XLog.E("指定移除的事件为null!");
        }
    }
    
    /// <summary>
    /// 删除指定事件
    /// </summary>
    /// <param name="action"></param>
    private void DelEvent(Action<EventArgs> action)
    {
        if (null != action)
        {
            List<Action<EventArgs>> actionsArr = new List<Action<EventArgs>>(_listeners.Values);
            List<Enum> eventTypeArr = new List<Enum>(_listeners.Keys);
            Predicate<Action<EventArgs>> predicate = v =>
            {
                Delegate[] d = v?.GetInvocationList();
                return (null != d && Array.Exists(d, vv => vv == (Delegate)action));
            };
            int index = actionsArr.FindIndex(predicate);
            if (index != -1)
            {
                DelEvent(eventTypeArr[index], action);
            }
            else
            {
                XLog.E("移除的事件未曾注册过 ！");
            }
        }
        else
        {
            XLog.E("指定移除的事件为 null ！");
        }
    }
    
    /// <summary>
    /// 删除指定事件类型的所有事件
    /// </summary>
    /// <param name="eventType">指定的事件类型</param>
    private void DelEvent(Enum eventType)
    {
        _listeners.Remove(eventType);
    }
    #endregion

    #region 公有接口方法

    /// <summary>
    /// 添加监听事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public void AddListener(Enum type, Action<EventArgs> action)
    {
        AddEvent(type, action);
    }

    /// <summary>
    /// 事件分发
    /// </summary>
    /// <param name="args"></param>
    public void Invoke(EventArgs args)
    {
        if (Interrupt)
        {
            return;
        }
        CallEvent(args);
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="type"></param>
    /// <param name="action"></param>
    public void DelListener(Enum type, Action<EventArgs> action)
    {
        DelEvent(type, action);
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    /// <param name="action"></param>
    public void DelListener(Action<EventArgs> action)
    {
        DelEvent(action);
    }

    /// <summary>
    /// 移除指定类型的所有事件监听
    /// </summary>
    /// <param name="type"></param>
    public void DelListener(Enum type)
    {
        DelEvent(type);
    }

    /// <summary>
    /// 移除所有事件监听
    /// </summary>
    public void RemoveAllListener()
    {
        InitEvent();
    }

    public T Allocate<T>() where T : EventArgs, new()
    {
        Type type = typeof(T);
        Queue<EventArgs> args;
        if (Recycled.TryGetValue(type, out args))
        {
            if (null != args && args.Count == Capacity)//回收使用最老
            {
                T arg = args.Dequeue() as T;
                arg.Dispose();
                return arg;
            }
        }
        return new T() as T;
    }

    public void Recycle(EventArgs target)
    {
        Type type = target.GetType();
        Queue<EventArgs> args;
        if (!Recycled.TryGetValue(type, out args))
        {
            args = new Queue<EventArgs>();
        }
        if (args.Count < Capacity && !args.Contains(target))
        {
            args.Enqueue(target);
        }
        else
        {
            target.Dispose();
        }
        Recycled[type] = args;
    }

    
    #endregion
} 
}



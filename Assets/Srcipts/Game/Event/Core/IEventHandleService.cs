using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 事件中心服务
    /// </summary>
    public interface IEventHandleService : IService
    {
        /// <summary>
        /// 事件对象池
        /// </summary>
        Dictionary<Type,Queue<EventArgs>> Recycled { get; set; }
        
        /// <summary>
        /// 对象池大小
        /// </summary>
        int Capacity { get; set; } 
        
        /// <summary>
        /// 是否中断事件分发，默认不中断
        /// </summary>
        bool Interrupt { get; set; }
        
        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        void AddListener(Enum type, Action<EventArgs> action);
    
        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="args"></param>
        void Invoke(EventArgs args);
    
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        void DelListener(Enum type, Action<EventArgs> action);
    
        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="action"></param>
        void DelListener(Action<EventArgs> action);
    
        /// <summary>
        /// 移除指定类型的所有事件监听
        /// </summary>
        /// <param name="type"></param>
        void DelListener(Enum type);
    
        /// <summary>
        /// 移除所有事件监听
        /// </summary>
        void RemoveAllListener();
    
        /// <summary>
        /// 分配事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Allocate<T>() where T : EventArgs, new();
    
        /// <summary>
        /// 回收参数类型的实例，并释放
        /// </summary>
        /// <param name="target"></param>
        void Recycle(EventArgs target);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LP.Framework
{
    /// <summary>
    /// Web与Unity交互接口
    /// </summary>
    public interface IWebService : INative
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        string ID { get; set; }
    
        /// <summary>
        /// Web 调用Unity 事件注册表
        /// </summary>
        Dictionary<string, List<Action<string>>> ListenEventList { get; set; }
    
        /// <summary>
        /// Unity 调用Web 回调事件
        /// </summary>
        Dictionary<string, TaskCompletionSource<string>> CallBacks { get; set; }
    
        /// <summary>
        /// Unity Call Web
        /// </summary>
        void UnityCallWeb(string type, string json);
    
        /// <summary>
        /// Web Call Unity
        /// </summary>
        void EmitUnity(string events);
    
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        void RegisterEvent(string type, Action<string> action);
    
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        void RemoveEvent(string type, Action<string> action);
    
        /// <summary>
        /// 调用Web回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        Task<string> CallWebWithBack(string type, string json);
    }
}

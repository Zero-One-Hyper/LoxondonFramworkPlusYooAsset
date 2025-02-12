using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Http接口请求服务
/// </summary>
public interface IHttpService
{
    Dictionary<EOPERATION, Action<TokenMsg>> Handlers { get; set; }

    /// <summary>
    /// 初始化
    /// </summary>
    void Init();

    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="op"></param>
    /// <param name="callback"></param>
    void HttpGet(EOPERATION op,Action<string> callback);
    
    /// <summary>
    /// Post请求
    /// </summary>
    /// <param name="op"></param>
    /// <param name="jsonData">string 参数</param>
    /// <param name="callback"></param>
    void HttpPost(EOPERATION op,string jsonData,Action<string> callback=null);

    /// <summary>
    /// Post 传字典
    /// </summary>
    /// <param name="op"></param>
    /// <param name="dic"></param>
    /// <param name="callback"></param>
    void HttpPost(EOPERATION op, Dictionary<string, string> dic, Action<string> callback = null);
}

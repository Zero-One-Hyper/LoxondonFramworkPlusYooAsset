using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回调事件接口
/// </summary>
public interface IHttpRspHandle
{
    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="handlers"></param>
    void RegisterMsg(Dictionary<EOPERATION, Action<TokenMsg>> handlers);
    
}

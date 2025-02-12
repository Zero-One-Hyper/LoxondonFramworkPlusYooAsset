using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 回退事件
/// </summary>
public class HttpRspHander:IHttpRspHandle
{
    public void RegisterMsg(Dictionary<EOPERATION, Action<TokenMsg>> handlers)
    {
        handlers.Add(EOPERATION.LOGIN,OnRspLogin);
        handlers.Add(EOPERATION.REGISTER,OnRspRegister);
    }

    private void OnRspLogin(TokenMsg data)
    {
        
    }
    
    private void OnRspRegister(TokenMsg data)
    {
        
    }
}

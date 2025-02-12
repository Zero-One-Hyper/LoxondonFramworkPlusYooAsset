using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Loxodon.Framework.Contexts;
using UnitData.Const;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Http 接口请求管理器
/// </summary>
public class HttpControl : IHttpService
{
    public Dictionary<EOPERATION, Action<TokenMsg>> Handlers { get; set; }
    private bool _isStartReq;
    private UnityWebRequest _request;

    private IBuiltInFuncService _builtInService;

    private IHttpRspHandle _rspHandle;
    
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        Handlers = new Dictionary<EOPERATION, Action<TokenMsg>>();
        _builtInService = Context.GetApplicationContext().GetService<IBuiltInFuncService>();
        _rspHandle = new HttpRspHander();
        _rspHandle.RegisterMsg(Handlers);
    }

    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="op"></param>
    /// <param name="callback"></param>
    public void HttpGet(EOPERATION op, Action<string> callback)
    {
        _builtInService.OnStartCoroutine(Get(op,callback));
    }

    /// <summary>
    /// Post请求 传Json
    /// </summary>
    /// <param name="op"></param>
    /// <param name="jsonData"></param>
    /// <param name="callback"></param>
    public void HttpPost(EOPERATION op, string jsonData, Action<string> callback = null)
    {
        _builtInService.OnStartCoroutine(Post(op,jsonData,callback));
    }

    /// <summary>
    /// Post 传字典
    /// </summary>
    /// <param name="op"></param>
    /// <param name="dic"></param>
    /// <param name="callback"></param>
    public void HttpPost(EOPERATION op, Dictionary<string, string> dic, Action<string> callback = null)
    {
        _builtInService.OnStartCoroutine(Post(op,dic,callback));
    }

    /// <summary>
    /// Get Url
    /// </summary>
    /// <param name="op"></param>
    /// <returns></returns>
    string GetUrl(EOPERATION op)
    {
        return Constant.HTTP_REQ_ADD_RESS+Constant.HTTP_URL_DIC[op];
    }

    IEnumerator Get(EOPERATION op,Action<string> callback)
    {
        var url = GetUrl(op);
        XLog.I($"Get url:{url}");
        if (!string.IsNullOrEmpty(url))
        {
            using (_request=UnityWebRequest.Get(url))
            {
                _isStartReq = true;
                _request.timeout = 1000;
                yield return _request.SendWebRequest();
                _isStartReq = false;
                if (_request.isHttpError || _request.isNetworkError)
                {
                    XLog.E(_request.error);
                    callback?.Invoke(null);
                }
                else
                {
                    callback?.Invoke(_request.downloadHandler.text);
                    Handlers[op](new TokenMsg { name = op.ToString(), handler = _request.downloadHandler });
                }
            }
        }
    }

    IEnumerator Post(EOPERATION op, string jsonData, Action<string> callback)
    {
        var url = GetUrl(op);
        XLog.I($"Get url:{url}");
        if (!string.IsNullOrEmpty(url))
        {
            using (_request=new UnityWebRequest(url,UnityWebRequest.kHttpVerbPOST))
            {
                _isStartReq = true;
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                _request.uploadHandler = new UploadHandlerRaw(data);
                _request.downloadHandler = new DownloadHandlerBuffer();
                _request.certificateHandler = new HttpRepSkipCert();
                _request.timeout = 1000;
                yield return _request.SendWebRequest();
                _isStartReq = false;
                if (_request.isHttpError || _request.isNetworkError)
                {
                    Debug.Log(_request.error);
                    callback?.Invoke(null);
                }
                else
                {
                    Handlers[op](new TokenMsg { handler = _request.downloadHandler });
                    callback?.Invoke(_request.downloadHandler.text);
                }
            }
        }
    }
    
    IEnumerator Post(EOPERATION op, Dictionary<string,string> dic, Action<string> callback)
    {
        var url = GetUrl(op);
        XLog.I($"Get url:{url}");
        if (!string.IsNullOrEmpty(url))
        {
            WWWForm form = new WWWForm();
            foreach (var item in dic)
            {
                form.AddField(item.Key,item.Value);
            }
            using (_request=UnityWebRequest.Post(url,form))
            {
                _isStartReq = true;
                _request.timeout = 1000;
                yield return _request.SendWebRequest();
                _isStartReq = false;
                if (_request.isHttpError || _request.isNetworkError)
                {
                    Debug.Log(_request.error);
                    callback?.Invoke(null);
                }
                else
                {
                    Handlers[op](new TokenMsg { handler = _request.downloadHandler });
                    callback?.Invoke(_request.downloadHandler.text);
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity 内置函数管理器
/// </summary>
public class BuiltInFuncManager :MonoBehaviour, IBuiltInFuncService
{
    public Action OnAwakeEvent { get; set; }
    public Action OnStartEvent { get; set; }
    public Action OnEnableEvent { get; set; }
    public Action OnUpdateEvent { get; set; }
    public Action OnDisEnableEvent { get; set; }
    public Action OnDestroyEvent { get; set; }

    public Action OnFixedUpdateEvent { get; set; }
    
    public Action OnLateUpdateEvent { get; set; }
    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        OnAwakeEvent?.Invoke();
    }

    public void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    public void Start()
    {
        OnStartEvent?.Invoke();
    }

    public void FixedUpdate()
    {
        OnFixedUpdateEvent?.Invoke();
    }

    public void Update()
    {
        OnUpdateEvent?.Invoke();
    }

    public void LateUpdate()
    {
        OnLateUpdateEvent?.Invoke();
    }

    public void OnDisEnable()
    {
        OnDisEnableEvent?.Invoke();
    }

    public void Destroy()
    {
        OnDestroyEvent?.Invoke();
    }

    public void OnStartCoroutine(IEnumerator cor)
    {
        StartCoroutine(cor);
    }

}

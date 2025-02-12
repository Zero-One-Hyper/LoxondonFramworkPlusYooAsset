using System.Collections;
using System;

/// <summary>
/// Unity 内置函数和事件
/// </summary>
public interface IBuiltInFuncService
{
    Action OnAwakeEvent { get; set; }

    Action OnStartEvent { get; set; }
    
    Action OnEnableEvent { get; set; }
    Action OnUpdateEvent { get; set; }

    Action OnDisEnableEvent { get; set; }

    Action OnDestroyEvent { get; set; }

    Action OnFixedUpdateEvent { get; set; }
    
    Action OnLateUpdateEvent { get; set; }

    void Awake();

    void OnEnable();
    
    void Start();
    
    /// 固定帧调用
    /// </summary>
    void FixedUpdate();
    
    /// <summary>
    /// 渲染帧调用
    /// </summary>
    void Update();
    
    /// <summary>
    /// 帧后调用
    /// </summary>
    void LateUpdate();

    void OnDisEnable();

    void Destroy();

    void OnStartCoroutine(IEnumerator cor);

}

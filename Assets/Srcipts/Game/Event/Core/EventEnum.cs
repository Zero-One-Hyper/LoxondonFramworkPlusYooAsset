using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 鼠标事件类型
/// </summary>
public enum StylusEvent 
{
    Enter,
    Press,
    Stay,
    Release,
    Exit
}

/// <summary>
/// 脚本事件类型
/// </summary>
public enum ScriptEvent
{
    Amount,
    Remove
}

/// <summary>
/// UI事件类型
/// </summary>
public enum UIEvent
{
    PopUp
}

/// <summary>
/// 颜色事件类型 
/// </summary>
public enum ColorEvent
{
    /// <summary>
    /// 使用指定颜色改变对象颜色
    /// </summary>
    ChangeTo,
}

/// <summary>
/// 相机事件类型
/// </summary>
public enum CameraEvent
{
    Move,
    Rotate,
    Zoom
}

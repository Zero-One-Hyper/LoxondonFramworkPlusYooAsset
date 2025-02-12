using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 颜色需求事件参数类
/// </summary>
public class ColorEventArgs : EventArgs
{
    /// <summary>
    /// 指定的颜色 
    /// </summary>
    public Color Color { private set; get; }

    /// <summary>
    /// 鼠标或者触笔事件
    /// </summary>
    /// <param name="_t">事件类型</param>
    /// <param name="_sender">事件发送者</param>
    /// <param name="_selected">被选中的游戏对象</param>
    /// <param name="_buttonID">按键编号</param>
    /// <param name="_hit">碰撞点信息</param>
    public ColorEventArgs Register(ColorEvent _t, GameObject _sender, Color color)
    {
        base.Register(_t, _sender);
        this.Color = color;
        return this;
    }
}


using UnityEngine;

public class ShanXiUIEventArgs : EventArgs
{
    /// <summary>
    /// web传入数据
    /// </summary>
    public string Json { get; set; }

    /// <summary>
    /// 列车相关事件注册
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sender"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public ShanXiUIEventArgs Register(ShanXiUIEvent type, string json)
    {
        base.Register(type);
        this.Json = json;
        return this;
    }
}

public enum ShanXiUIEvent
{
    SwitchUIMask = 0,
}
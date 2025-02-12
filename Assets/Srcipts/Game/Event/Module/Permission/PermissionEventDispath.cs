using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class PermissionEventArgs : EventArgs
{
    /// <summary>
    /// web传入数据
    /// </summary>
    public string Json { get; set; }

    /// <summary>
    /// 事件注册
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sender"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public PermissionEventArgs Register(PermissionEvent type, string json)
    {
        base.Register(type);
        this.Json = json;
        return this;
    }
}
public enum PermissionEvent
{
    CheckRoomPermission,
    CheckGuildRoomPermission,
}

public static class PermissionEventDispath
{
    public static void DispatchSingleRoomPermission(string json)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<PermissionEventArgs>()
            .Register(PermissionEvent.CheckRoomPermission, json)
            .Invoke();
    }
}

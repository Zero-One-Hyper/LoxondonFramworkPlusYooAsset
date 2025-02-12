using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public static class UIEventDispatch
{

    /// <summary>
    /// 开关Mask
    /// </summary>
    /// <param name="json"></param>
    public static void DispatchSwitchUIMask(string json)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<ShanXiUIEventArgs>()
            .Register(ShanXiUIEvent.SwitchUIMask, json)
            .Invoke();
    }
}

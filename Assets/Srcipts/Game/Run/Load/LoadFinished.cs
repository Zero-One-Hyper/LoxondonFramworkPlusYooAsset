using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;

public class LoadFinished : IRunLoad
{
    public async Task Load()
    {
        //向web发送unity加载完成消息
        var webService = Context.GetApplicationContext().GetService<IWebService>();
        webService.UnityCallWeb(Constant.UNITY_CALL_LOAD_COMPLETE, "");
        XLog.I("unity加载完成");
    }
}
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using UnitData.Const;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 与Web交互部分初始化
    /// </summary>
    public class BridgeRunLoad : IRunLoad
    {
        public async Task Load()
        {
            GameObject webObj = new GameObject();
            webObj.name = Constant.WEB_CALL_OBJ_NAME;
            IWebService webService = webObj.AddComponent<WebBridgeCtrl>();
            Context.GetApplicationContext().GetContainer().Register(webService);
            webService.Init();
        }
    }
}
using System.Threading.Tasks;
// using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 启动加载所需UI
    /// </summary>
    public class UIRunLoad : IRunLoad
    {
        //private IAssetLoadUtil loadUtil;
        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();
            var viewService = context.GetService<IViewService>();
            var main = viewService.OpenView<MainPanel>(1111, "2123") as MainPanel;

            //var maskPanel = viewService.OpenView<MaskPanel>() as MaskPanel;
        }
    }
}
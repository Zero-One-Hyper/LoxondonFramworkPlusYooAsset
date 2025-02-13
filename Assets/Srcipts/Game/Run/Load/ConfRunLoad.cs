using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 启动加载所需配置
    /// </summary>
    public class ConfRunLoad : IRunLoad
    {
        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();
            //资源加载辅助类
            var resLoader = context.GetService<IAssetLoadUtil>();
            MeshLightingDataService test = new MeshLightingDataService();
            test.Init();
        }
    }
}
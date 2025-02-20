using System.Collections.Generic;
using System.Threading.Tasks;
// using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 项目启动加载资源
    /// </summary>
    public class SceneGameObjectRunLoad : IRunLoad
    {
        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();

            ISceneGameObjectService sceneGameObjectService = new SceneGameObjectService();
            sceneGameObjectService.Init();
            context.GetContainer().Register<ISceneGameObjectService>(sceneGameObjectService);
            sceneGameObjectService.LoadOfficeCence();
        }
    }
}
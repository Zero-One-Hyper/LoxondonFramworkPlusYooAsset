using Loxodon.Framework.Contexts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace LP.Framework
{
    /// <summary>
    /// 特效控制器(不需要继承MonoBehaviour)
    /// </summary>
    public class EffectControl : IEffectService
    {
        private IAssetLoadUtil _assetLoadUtil;

        public void Init()
        {
            ApplicationContext context = Context.GetApplicationContext();
            _assetLoadUtil = context.GetService<IAssetLoadUtil>();

            _assetLoadUtil.LoadAssetAsync<GameObject>("VFXes", handle =>
            {
                if (handle != null)
                {
                    GameObject vfxRoot = handle.InstantiateSync();

                    ISceneGameObjectService sceneGameObjectService = context.GetService<ISceneGameObjectService>();
                    sceneGameObjectService.AddGameObject("VFXes", vfxRoot);
                }
                else
                {
                    XLog.E($"未找到预制体VFXes");
                }
            });
        }
    }
}
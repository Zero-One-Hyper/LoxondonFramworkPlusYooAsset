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

            var effectsPrefab = _assetLoadUtil.ResourcesLoad<GameObject>("Prefabs/VFXes.prefab");
            var vfxRoot = GameObject.Instantiate(effectsPrefab);
            ISceneGameObjectService sceneGameObjectService = context.GetService<ISceneGameObjectService>();
            sceneGameObjectService.AddGameObject("VFXes", vfxRoot);
        }
    }
}
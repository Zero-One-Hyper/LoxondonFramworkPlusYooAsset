using Loxodon.Framework.Contexts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using YooAsset;

namespace LP.Framework
{
    /// <summary>
    /// 特效控制器(不需要继承MonoBehaviour)
    /// </summary>
    public class EffectControl : IEffectService
    {
        private ApplicationContext _context;
        private IAssetLoadUtil _assetLoadUtil;
        private GuildLines _guildLine;

        public async void Init()
        {
            _context = Context.GetApplicationContext();
            _assetLoadUtil = _context.GetService<IAssetLoadUtil>();

            var effectsPrefab = await _assetLoadUtil.ResourceLoadAsync<GameObject>("VFXes");
            //var effectsPrefab = _assetLoadUtil.ResourceLoadAsync<GameObject>("VFXes");
            GameObject vfxRoot = GameObject.Instantiate(effectsPrefab);
            vfxRoot.name = "VFXes";

            //_assetLoadUtil.LoadAsset<GameObject>("VFXes", LoadVFXGameObject);
            //var vfxRoot = GameObject.Instantiate(effectsPrefab);
            ISceneGameObjectService sceneGameObjectService = _context.GetService<ISceneGameObjectService>();
            sceneGameObjectService.AddGameObject("VFXes", vfxRoot);

            ////导引线
            GameObject guildLineGo = vfxRoot.transform.GetChild(0).gameObject;
            _guildLine = guildLineGo.AddComponent<GuildLines>();
            _guildLine.Init();
            _guildLine.gameObject.SetActive(false);
        }

        private void LoadVFXGameObject(AssetHandle handle)
        {
            if (handle != null)
            {
                GameObject vfxRoot = handle.InstantiateSync();

                ISceneGameObjectService sceneGameObjectService = _context.GetService<ISceneGameObjectService>();
                sceneGameObjectService.AddGameObject("VFXes", vfxRoot);
                //导引线
                GameObject guildLineGo = vfxRoot.transform.GetChild(0).gameObject;
                _guildLine = guildLineGo.AddComponent<GuildLines>();
                _guildLine.Init();
                _guildLine.gameObject.SetActive(false);
            }
            else
            {
                XLog.E("VFX game object 加载失败");
            }
        }

        public void ShowGuildLine(NavMeshPath path)
        {
            _guildLine.gameObject.SetActive(true);
            _guildLine.SetLinePath(path);
        }

        public void HideGuildLine()
        {
            _guildLine.gameObject.SetActive(false);
        }
    }
}
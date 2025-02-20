using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using UnityEngine;
using YooAsset;

namespace LP.Framework
{
    /// <summary>
    /// 项目启动相机相关设置
    /// </summary>
    public class CameraRunLoad : IRunLoad
    {
        private IAssetLoadUtil _assetLoadUtil;

        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();
            _assetLoadUtil = context.GetService<IAssetLoadUtil>();

            //_assetLoadUtil.LoadAsset<GameObject>("Cameras", LoadCamera);
            var cameraPrefab = await _assetLoadUtil.ResourceLoadAsync<GameObject>("Cameras");
            var go = GameObject.Instantiate(cameraPrefab);
            go.name = "Cameras";

            var cameraController = go.AddComponent<CameraController>();
            cameraController.Init();
            Context.GetApplicationContext().GetContainer().Register<ICameraService>(cameraController);
        }

        private void LoadCamera(AssetHandle handle)
        {
            if (handle != null)
            {
                GameObject camera = handle.InstantiateSync();
                camera.name = "Cameras";

                var cameraController = camera.AddComponent<CameraController>();
                cameraController.Init();
                Context.GetApplicationContext().GetContainer().Register<ICameraService>(cameraController);
            }
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;

namespace LP.Framework
{
    /// <summary>
    /// 应用启动主入口，加载各种资产和数据
    /// </summary>
    public class AppRun : IAppRun
    {
        public List<IRunLoad> AppLoadList { get; set; }

        public async void StartUp()
        {
            XLog.I("game is run");
            AppLoadList = new List<IRunLoad>();
#if Bundle
            await RegisterAsync(new BundleRunLoad());
#endif
            await RegisterAsync(new BridgeRunLoad());
            await RegisterAsync(new BuiltInRunLoad());
            await RegisterAsync(new ConfRunLoad());
            await RegisterAsync(new PermissionsLoad());
            await RegisterAsync(new InputRunLoad());
            await RegisterAsync(new SceneDataLoader()); //IMeshLightingDataService  IRoomService
            await RegisterAsync(new SceneGameObjectRunLoad());
            await RegisterAsync(new UIRunLoad());
            await RegisterAsync(new IconRunLoad());
            await RegisterAsync(new EffectControlLoad());
            await RegisterAsync(new CameraRunLoad());
            await RegisterAsync(new EmployeeCubicleRunLoad());
            await RegisterAsync(new GuildTourRunLoad());
            await RegisterAsync(new SceneLogicLoad()); //INavigationService  ISceneLogicService IIPService 
            await RegisterAsync(new RayCastServiceLoad());
            await RegisterAsync(new DoorplateLoad());
            await RegisterAsync(new LoadFinished());
        }

        public void Register(IRunLoad runLoad)
        {
            if (!AppLoadList.Contains(runLoad))
                AppLoadList.Add(runLoad);
        }

        async Task RegisterAsync(IRunLoad load)
        {
            if (!AppLoadList.Contains(load))
            {
                AppLoadList.Add(load);
                await load.Load();
            }
        }

        public void Stop()
        {
            XLog.I("game is stop");
            var eventService = Context.GetApplicationContext().GetService<IEventHandleService>();
            eventService.RemoveAllListener();
        }
    }
}
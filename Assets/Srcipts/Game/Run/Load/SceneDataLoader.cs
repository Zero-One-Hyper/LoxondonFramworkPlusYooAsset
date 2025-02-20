using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class SceneDataLoader : IRunLoad
{
    public async Task Load()
    {
        ApplicationContext context = Context.GetApplicationContext();

        //加载光照信息
        IMeshLightingDataService lightingDataService = new MeshLightingDataService();
        lightingDataService.Init();
        context.GetContainer().Register<IMeshLightingDataService>(lightingDataService);


        //加载场景点位信息
        IRoomService roomDataService = new RoomService();
        roomDataService.Init();
        context.GetContainer().Register<IRoomService>(roomDataService);
    }
}
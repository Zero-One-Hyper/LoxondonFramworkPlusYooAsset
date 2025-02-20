using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class SceneLogicLoad : IRunLoad
{
    public async Task Load()
    {
        ApplicationContext context = Context.GetApplicationContext();

        //寻路模块目的地
        INavigationService navigationService = new NavigationService();
        navigationService.Init();
        context.GetContainer().Register(navigationService);

        ISceneLogicService sceneLogic = new SceneLogic();
        //注册接口
        context.GetContainer().Register<ISceneLogicService>(sceneLogic);
        sceneLogic.Init();
        IIPService iPService = new IPService();
        context.GetContainer().Register<IIPService>(iPService);
        iPService.Init();
    }
}
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class RayCastServiceLoad : IRunLoad
{
    public async Task Load()
    {
        ApplicationContext context = Context.GetApplicationContext();

        IRayCastService rayCastService = new RayCastService();
        //注册接口
        context.GetContainer().Register<IRayCastService>(rayCastService);
        rayCastService.Init();
    }
}

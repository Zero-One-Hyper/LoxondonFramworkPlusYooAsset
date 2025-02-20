using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class DoorplateLoad : IRunLoad
{
    public async Task Load()
    {

        ApplicationContext context = Context.GetApplicationContext();

        IDoorplateService doorplateService = new DoorplateService();
        doorplateService.Init();

        context.GetContainer().Register<IDoorplateService>(doorplateService);
    }
}

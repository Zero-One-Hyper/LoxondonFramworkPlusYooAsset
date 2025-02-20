using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class PermissionsLoad : IRunLoad
{
    public async Task Load()
    {
        var context = Context.GetApplicationContext();

        IPermissionsManagement permissionsManagement = new PermissionsManagement();

        permissionsManagement.Init();

        context.GetContainer().Register<IPermissionsManagement>(permissionsManagement);
    }
}
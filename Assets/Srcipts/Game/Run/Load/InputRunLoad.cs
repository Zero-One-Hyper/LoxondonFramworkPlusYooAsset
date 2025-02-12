using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace LP.Framework
{
    public class InputRunLoad : IRunLoad
    {
        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();

            //新建
            IInputService inputService = new InputController();
            //注册
            inputService.Init();
            context.GetContainer().Register(inputService);
        }
    }
}
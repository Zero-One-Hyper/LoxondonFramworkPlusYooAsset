using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

namespace LP.Framework
{
    public class EffectControlLoad : IRunLoad
    {
        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();

            IEffectService effectControl = new EffectControl();
            effectControl.Init();

            context.GetContainer().Register<IEffectService>(effectControl);
        }
    }
}
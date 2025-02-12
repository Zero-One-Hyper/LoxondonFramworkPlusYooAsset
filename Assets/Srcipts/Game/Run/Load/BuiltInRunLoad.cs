using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace LP.Framework
{
    public class BuiltInRunLoad : IRunLoad
    {
        public async Task Load()
        {
            GameObject bunilt = new GameObject();
            bunilt.name = "BuiltIn";
            var builtIn = bunilt.AddComponent<BuiltInFuncManager>();
            Context.GetApplicationContext().GetContainer().Register<IBuiltInFuncService>(builtIn);
        }
    }
}
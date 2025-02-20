using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using UnitData.Const;
using UnityEngine;

namespace LP.Framework
{
    public class IconRunLoad : IRunLoad
    {
        // private IAssetLoadUtil _assetLoadUtil;
        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();
            // _assetLoadUtil = context.GetService<IAssetLoadUtil>();
        }
    }
}

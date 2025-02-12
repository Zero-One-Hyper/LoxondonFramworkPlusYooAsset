using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using UnityEngine;
using System.Threading.Tasks;

namespace LP.Framework
{
    public class BundleRunLoad : IRunLoad
    {
        public async Task Load()
        {
            ApplicationContext context = Context.GetApplicationContext();
            //var bundleService = context.GetService<IAssetBundleService>();
            //var result=await bundleService.Run();
            //if (result)
            //{
            //    XLog.I("AB初始化完成！可加载需要资源");
            //}
            //else
            //{
            //    XLog.E("AB初始化失败！！！");
            //}
        }
    }
}
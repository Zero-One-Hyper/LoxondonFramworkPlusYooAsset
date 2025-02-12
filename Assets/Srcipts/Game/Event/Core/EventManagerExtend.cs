using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace LP.Framework
{
    public static class EventManagerExtend
    {
        /// <summary>
        /// 使用该参数类型的实例分发事件
        /// </summary>
        /// <param name="args">参数实例</param>
        public static void Invoke(this EventArgs args)
        {
            Context.GetApplicationContext().GetService<IEventHandleService>().Invoke(args);
        }
    }
}

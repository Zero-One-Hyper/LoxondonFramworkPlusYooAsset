using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Loxodon.Framework.Asynchronous;
//using Loxodon.Framework.Bundles;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源加载
/// </summary>
public class LoadAssetMgr : IAssetLoadUtil
{
    //private Dictionary<string, IBundle> _bundles = new Dictionary<string, IBundle>();
    //private IResources _resources;

    public LoadAssetMgr()
    {
    }

    public T ResourcesLoad<T>(string name) where T : Object
    {
        var result = Resources.Load<T>(this.GetFilePathWithoutExtension(name));
        return result;
    }

    public string GetFilePathWithoutExtension(string path)
    {
        int index = path.IndexOf('.');
        if (index < 0)
            return path;
        return path.Substring(0, index);
    }
}
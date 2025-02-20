using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Contexts;
using LP.Framework;
//using Loxodon.Framework.Bundles;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

/// <summary>
/// 资源加载
/// </summary>
public class LoadAssetMgr : IAssetLoadUtil
{
    private IAssetBundleService assetBundleService;

    public LoadAssetMgr()
    {
    }

    public void LoadAsset<T>(string assetName, Action<AssetHandle> callback) where T : Object
    {
        if (assetBundleService == null)
        {
            assetBundleService = Context.GetApplicationContext().GetService<IAssetBundleService>();
        }

        assetBundleService.LoadAsync<T>(assetName, callback);
    }

    public async Task<T> ResourceLoadAsync<T>(string name) where T : Object
    {
        if (assetBundleService == null)
        {
            assetBundleService = Context.GetApplicationContext().GetService<IAssetBundleService>();
        }

        var res = await assetBundleService.LoadAsync<T>(name);
        return res;
    }
    /*
    public void LoadAssetAsync(string name, Action<AssetHandle> callBack)
    {
        if (assetBundleService == null)
        {
            assetBundleService = Context.GetApplicationContext().GetService<IAssetBundleService>();
        }

        assetBundleService.LoadConfigs(name, callBack);
    }
    */

    public string GetFilePathWithoutExtension(string path)
    {
        int index = path.IndexOf('.');
        if (index < 0)
            return path;
        return path.Substring(0, index);
    }
}
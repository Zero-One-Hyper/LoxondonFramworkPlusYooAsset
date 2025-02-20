using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Loxodon.Framework.Bundles;
using LP.Framework;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

/// <summary>
/// 资源加载辅助工具
/// </summary>
public interface IAssetLoadUtil : ILoad
{
    /// <summary>
    /// Resources 资源加载
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    //T ResourcesLoad<T>(string name) where T : Object;
    void LoadAsset<T>(string assetName, Action<AssetHandle> callback) where T : Object;

    /// <summary>
    /// Resources 资源异步加载
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> ResourceLoadAsync<T>(string name) where T : Object;

    /// <summary>
    /// 加载配置文件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callBack"></param>
    //void LoadAssetAsync(string name, Action<AssetHandle> callBack);
}
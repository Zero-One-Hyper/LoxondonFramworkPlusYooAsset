using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callBack"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public void LoadAssetAsync<T>(string name, Action<AssetHandle> callBack) where T : Object;
}
using System;
using System.Collections;
using System.Collections.Generic;
//using Loxodon.Framework.Bundles;
using LP.Framework;
using UnityEngine;
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
    T ResourcesLoad<T>(string name) where T : Object;
}
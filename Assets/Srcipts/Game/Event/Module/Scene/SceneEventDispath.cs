using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public static class SceneEventDispath
{
    public static void DispatchScene(string data)
    {

    }

    public static void DispatchSwitchScene(string data)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<SceneEventArgs>()
            .Register(SceneEvent.SceneType, data)
            .Invoke();
    }

    public static void DispatchAutoRounding(string data)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<SceneEventArgs>()
            .Register(SceneEvent.AutoRoaming, data)
            .Invoke();
    }

    public static void DispatchSwitchIP(string data)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<SceneEventArgs>()
            .Register(SceneEvent.SwitchIP, data)
            .Invoke();
    }

    public static void DispatchAllDoorplateData(string data)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<SceneEventArgs>()
           .Register(SceneEvent.AllDoorplateData, data)
           .Invoke();
    }

    public static void DispatchSingleDoorplateData(string data)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<SceneEventArgs>()
           .Register(SceneEvent.SingleDoorplateData, data)
           .Invoke();
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class SceneEventRecv : MonoBehaviour
{
    private void Start()
    {
        var eventService = Context.GetApplicationContext().GetService<IEventHandleService>();
        eventService.AddListener(SceneEvent.SceneType, LoadScene);
    }

    void LoadScene(EventArgs args)
    {
        var sceneArgs = args as SceneEventArgs;
        XLog.I(sceneArgs.Data);
    }
}

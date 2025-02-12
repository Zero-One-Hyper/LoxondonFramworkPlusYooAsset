using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEventArgs : EventArgs
{
    public string Data { get; set; }

    public SceneEventArgs Register(SceneEvent type, string data)
    {
        base.Register(type);
        this.Data = data;
        return this;
    }
}

public enum SceneEvent
{
    SceneType,
    AutoRoaming,
    SwitchIP,
    SingleDoorplateData,
    AllDoorplateData,
}
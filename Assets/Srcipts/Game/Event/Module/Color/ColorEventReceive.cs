using System;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ColorEventReceive : MonoBehaviour
{
    void Awake()
    {
        Context.GetApplicationContext().GetService<IEventHandleService>()
            .AddListener(ColorEvent.ChangeTo, OnColorChangeRequired);
    }

    private void OnColorChangeRequired(EventArgs obj)
    {
        ColorEventArgs args = obj as ColorEventArgs;
        GetComponent<MeshRenderer>().material.color = args.Color;
    }
}
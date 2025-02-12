using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class CameraEventReceive : MonoBehaviour
{
    private IEventHandleService _eventHandleService;

    private void Awake()
    {
        _eventHandleService = Context.GetApplicationContext().GetService<IEventHandleService>();
        _eventHandleService.AddListener(CameraEvent.Move, OnCameraMove);
        _eventHandleService.AddListener(CameraEvent.Move, OnCameraRotate);
        _eventHandleService.AddListener(CameraEvent.Move, OnCameraZoom);
    }

    void OnCameraMove(EventArgs args)
    {
        var camArgs = args as CameraEventArgs;
        Camera.main.transform.position = camArgs.V3;
    }
    void OnCameraRotate(EventArgs args)
    {
        var camArgs = args as CameraEventArgs;
        Camera.main.transform.rotation =Quaternion.Euler(camArgs.V3);
    }
    void OnCameraZoom(EventArgs args)
    {
        var camArgs = args as CameraEventArgs;
        Camera.main.transform.localScale = camArgs.V3;
    }
}

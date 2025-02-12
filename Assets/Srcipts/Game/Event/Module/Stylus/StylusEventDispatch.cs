using System;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class StylusEventDispatch : MonoBehaviour
{
    private GameObject selected;
    public LayerMask layerMask = 1;
    public float maxDistance = 100;

    private IEventHandleService _eventService;

    private void Awake()
    {
        _eventService = Context.GetApplicationContext().GetService<IEventHandleService>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, maxDistance, layerMask))
        {
            GameObject cached = hit.collider.gameObject;
            if (null == selected) //从无到有
            {
                selected = cached;
                _eventService.Allocate<StylusEventArgs>()
                    .Register(StylusEvent.Enter, gameObject, selected)
                    .Invoke();
            }
            else //从有到有
            {
                if (selected != cached)
                {
                    _eventService.Allocate<StylusEventArgs>()
                        .Register(StylusEvent.Exit, gameObject, selected)
                        .Invoke();
                    selected = cached;
                    _eventService.Allocate<StylusEventArgs>()
                        .Register(StylusEvent.Enter, gameObject, selected)
                        .Invoke();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                _eventService.Allocate<StylusEventArgs>()
                    .Register(StylusEvent.Press, gameObject, selected, 0)
                    .Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                _eventService.Allocate<StylusEventArgs>()
                    .Register(StylusEvent.Release, gameObject, selected, 0)
                    .Invoke();
            }
        }
        else
        {
            if (null != selected)
            {
                _eventService.Allocate<StylusEventArgs>()
                    .Register(StylusEvent.Exit, gameObject, selected)
                    .Invoke();
                selected = null;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public interface IRayCastService : IService
{
    bool DoMouseEnterRayCast(Vector2 mousePosition, out IInteractCollider interactCollider);
}

//射线检测管理
public class RayCastService : IRayCastService
{
    private ApplicationContext _context;
    private IViewService _viewService;
    private Camera _mainCamera;
    private Ray _ray;
    private int _layerMask = (1 << 0) | (1 << 8) | (1 << 7) | (1 << 6) | (1 << 5);//检测第0、7、8、6层 以及第5层UI


    public void Init()
    {
        _context = Context.GetApplicationContext();
        var inputService = _context.GetService<IInputService>();
        inputService.RegisterMouseInteractive(this.OnCheckRayCast);
        _viewService = _context.GetService<IViewService>();

        _mainCamera = Camera.main;
    }

    private bool CanRayCast(Vector2 mousePosition)
    {
        if (_viewService.CheckMouseOnUI(mousePosition))
        {
            return false;
        }
        return true;
    }

    private void OnCheckRayCast(Vector2 mousePosition)
    {
        if (!CanRayCast(mousePosition))
        {
            return;
        }
        _ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(_ray, out RaycastHit raycast, 100.0f, _layerMask))
        {
            //XLog.I($"点击{raycast.transform.name}");
            if (raycast.transform.TryGetComponent(out IInteractCollider interactCollider))
            {
                interactCollider.ColliderInteractive(InteractiveColliderType.MouseClick);
            }
        }
    }
    public bool DoMouseEnterRayCast(Vector2 mousePosition, out IInteractCollider interactCollider)
    {
        _ray = _mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(_ray, out RaycastHit raycast, 100.0f, _layerMask))
        {
            //XLog.I($"点击{raycast.transform.name}");
            if (raycast.transform.TryGetComponent(out interactCollider))
            {
                return true;
            }
            else
            {
                interactCollider = null;
                return false;
            }
        }
        interactCollider = null;
        return false;
    }
}

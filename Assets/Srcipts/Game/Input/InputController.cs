using System;
using System.Collections.Generic;
using System.Text;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public enum InputActionType
{
    Move = 0,
    Rotate = 1,
    Scale = 2,
}

namespace LP.Framework
{
    public interface IInputService : IService
    {
        void ChangeInputType(GameScene inputType);
        void RegisterCameraControl(InputActionType inputActionType, GameScene scene, Action<Vector2> callBack);
        void RegisterMouseInteractive(Action<Vector2> callBack);
        void RegisterMouseDoubleClickInteractive(Action<Vector2> callBack);
        void SetMouseControlActive(bool active);
        void RegisterShortCutKeyStopAutoRoaming(Action callBack);
        void RegisterUIEnterButtonClick(Action callBack);
        void DisRegisterUIEnterButtonClick(Action callBcak);
        void DisableMoveInput();
        void EnableMoveInput();
    }
}

public class InputController : IInputService, InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions _inputSystemActions;
    private GameScene _currentInputType = GameScene.Panorama;

    private Action<Vector2>[] _panoramaActions = new Action<Vector2>[3];
    private Action<Vector2>[] _roamingActions = new Action<Vector2>[3];
    private Action<Vector2> _mouseClickActions;
    private Action<Vector2> _mouseDoubleClickActions;
    private Action _shortCutKeyStopAutoRoaming;
    private Action _uiInputEnter;

    private Vector2 _moveDir;
    private Vector2 _mouseRotate;
    private float _lastClickTime = float.MaxValue;
    private bool _mouseControl = true;
    private bool _keyBoardInput = true;
    private bool _isDragging = false;

    public void Init()
    {
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.SetCallbacks(this);
        _inputSystemActions.Player.Enable();
        XLog.I("初始化输入系统");
    }

    public void RegisterCameraControl(InputActionType inputActionType, GameScene scene, Action<Vector2> callBack)
    {
        switch (scene)
        {
            case GameScene.Panorama:
                _panoramaActions[(int)inputActionType] += callBack;
                break;
            case GameScene.Roaming:
                _roamingActions[(int)inputActionType] += callBack;
                break;
            case GameScene.None:
                break;
        }
    }

    public void RegisterMouseInteractive(Action<Vector2> callBack)
    {
        this._mouseClickActions += callBack;
    }

    public void RegisterMouseDoubleClickInteractive(Action<Vector2> callBack)
    {
        this._mouseDoubleClickActions += callBack;
    }

    //注册R键结束自动漫游
    public void RegisterShortCutKeyStopAutoRoaming(Action callBack)
    {
        this._shortCutKeyStopAutoRoaming += callBack;
    }

    public void RegisterUIEnterButtonClick(Action callBack)
    {
        this._uiInputEnter += callBack;
    }

    public void DisRegisterUIEnterButtonClick(Action callBack)
    {
        this._uiInputEnter -= callBack;
    }

    public void ChangeInputType(GameScene inputType)
    {
        this._currentInputType = inputType;
    }

    //省图中的移动
    public void OnOverViewMove(InputAction.CallbackContext context)
    {
        if (this._currentInputType != GameScene.Panorama || !_mouseControl)
        {
            return;
        }

        _moveDir = context.ReadValue<Vector2>().normalized;
        _panoramaActions[(int)InputActionType.Move]?.Invoke(_moveDir);
    }

    //省图中的旋转
    public void OnOverViewRotate(InputAction.CallbackContext context)
    {
        if (this._currentInputType != GameScene.Panorama || !_mouseControl)
        {
            return;
        }

        if (context.phase == InputActionPhase.Started && !_isDragging)
        {
            _isDragging = true;
        }

        _mouseRotate = context.ReadValue<Vector2>().normalized;
        _panoramaActions[(int)InputActionType.Rotate]?.Invoke(_mouseRotate);
    }

    //办公室场景中的移动
    public void OnWalkThroughMove(InputAction.CallbackContext context)
    {
        if (this._currentInputType != GameScene.Roaming || !_keyBoardInput)
        {
            return;
        }

        _moveDir = context.ReadValue<Vector2>().normalized;
        if (AutoRoaming.IsAutoRoaming && _moveDir.magnitude > 0.1f)
        {
            // 显示操作提示
            StringBuilder sb = new StringBuilder("当前为固定路线漫游，不可调整漫游方向，若想要自由控制方向，可切换为自由漫游模式");
            //_viewService.OpenView<UITip>(sb);

            var _viewService = ApplicationContext.GetApplicationContext().GetService<IViewService>();
            var uITip = _viewService.GetView<UITip>();
            if (uITip == null)
            {
                _viewService.OpenView<UITip>(sb);
            }
            else
            {
                UITip tip = uITip as UITip;
                tip.RefreshData(sb);
            }

            return;
        }

        _roamingActions[(int)InputActionType.Move]?.Invoke(_moveDir);
    }

    //办公室场景中的镜头旋转
    public void OnWalkThroughRotate(InputAction.CallbackContext context)
    {
        if (this._currentInputType != GameScene.Roaming || !_keyBoardInput)
        {
            return;
        }

        if (context.phase == InputActionPhase.Started && !_isDragging)
        {
            _isDragging = true;
        }

        _mouseRotate = context.ReadValue<Vector2>();
        if (AutoRoaming.IsAutoRoaming && _mouseRotate.magnitude > 0.1f)
        {
            return;
        }

        _roamingActions[(int)InputActionType.Rotate]?.Invoke(_mouseRotate);
    }

    //鼠标单击
    public void OnMouseInteractive(InputAction.CallbackContext context)
    {
        if (_isDragging)
        {
            _isDragging = false;

            return;
        }

        if (context.phase == InputActionPhase.Performed && context.interaction
                is PressInteraction && _mouseControl)
        {
            float clickTime = Mathf.Abs(Time.realtimeSinceStartup - _lastClickTime);
            _lastClickTime = Time.realtimeSinceStartup;
            if (clickTime < 0.2f)
            {
                //双击
                return;
            }
#if UNITY_EDITOR
            XLog.I("Editor Only 鼠标点击，检测互动");
#endif
            _mouseClickActions?.Invoke(GetMouseCurrentPosition());
        }
    }

    //UI输入 按下Enter键
    public void OnUIEnter(InputAction.CallbackContext context)
    {
        _uiInputEnter?.Invoke();
    }

    //鼠标位置
    private Vector2 GetMouseCurrentPosition()
    {
        return Mouse.current == null ? Vector2.zero : Mouse.current.position.ReadValue();
    }

    //鼠标双击
    public void OnMouseDoubleClickInteractive(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && _mouseControl)

        {
#if UNITY_EDITOR
            XLog.I("Editor Only 鼠标双击，检测互动");
#endif
            _mouseDoubleClickActions?.Invoke(GetMouseCurrentPosition());
        }
    }

    public void OnScale(InputAction.CallbackContext context)
    {
    }

    //R键快速结束自动漫游
    public void OnStopAutoRoaming(InputAction.CallbackContext context)
    {
        if (_keyBoardInput)
        {
            _shortCutKeyStopAutoRoaming?.Invoke();
        }
    }

    public void SetMouseControlActive(bool active)
    {
        this._mouseControl = active;
    }

    public void DisableMoveInput()
    {
        _keyBoardInput = false;

        _roamingActions[(int)InputActionType.Move]?.Invoke(Vector2.zero);
    }

    public void EnableMoveInput()
    {
        _keyBoardInput = true;
    }
}
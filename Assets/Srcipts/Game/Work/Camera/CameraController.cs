using System;
using Unity.Cinemachine;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

/// <summary>
/// 相机服务接口
/// </summary>
public interface ICameraService : IService
{
    void SwitchCamera(GameScene scene);

    /// <summary>
    /// 移动
    /// </summary>
    void Move(Vector2 input);

    /// <summary>
    /// 旋转
    /// </summary>
    void Rotate(Vector2 input);

    /// <summary>
    /// 缩放
    /// </summary>
    void Scale();

    void SetCameraFollowTarget(Transform followTarget);
    void SetCameraFocus(Transform target, Action callBack);
    void SetCameraUnFocus(Action callBack);
}

/// <summary>
/// 相机控制器
/// </summary>
public class CameraController : MonoBehaviour, ICameraService
{
    private GameScene _gameScene;
    private ApplicationContext _context;

    private Camera _mainCamera;
    private WalkThroughCamera _walkThroughCamera;
    private OverViewCamera _overViewCamera;

    private BaseCamera _currentCamera;

    private IInputService _inputService;

    public void Init()
    {
        _context = Context.GetApplicationContext();
    }

    private void Start()
    {
        var context = Context.GetApplicationContext();
        this._inputService = context.GetService<IInputService>();

        _mainCamera = Camera.main;

        _overViewCamera = this.transform.GetChild(0).gameObject.AddComponent<OverViewCamera>();
        _overViewCamera.gameObject.SetActive(false);
        _overViewCamera.Init(_mainCamera);

        _walkThroughCamera = this.transform.GetChild(1).gameObject.AddComponent<WalkThroughCamera>();
        _walkThroughCamera.gameObject.SetActive(true);
        _walkThroughCamera.Init(_mainCamera);

        //默认相机
        _currentCamera = _walkThroughCamera;

        RegistInputAction();
        //SwitchCamera(GameScene.Roaming);
        //SceneEventDispath.DispatchSwitchScene("Roaming");
    }

    private void RegistInputAction()
    {
        _inputService.RegisterCameraControl(InputActionType.Move, GameScene.Panorama, Move);
        _inputService.RegisterCameraControl(InputActionType.Rotate, GameScene.Panorama, Rotate);
    }

    public void SetCameraFollowTarget(Transform followTransform)
    {
        _currentCamera.SetFollowTarget(followTransform);
    }

    public void SwitchCamera(GameScene scene)
    {
        if (_currentCamera == null)
        {
            return;
        }

        _currentCamera.DisableCamera();

        switch (scene)
        {
            case GameScene.Panorama:
                //切换到省图
                this._currentCamera = _overViewCamera;
                SetCameraUnFocus(() => { });
                break;
            case GameScene.Roaming:
                this._currentCamera = _walkThroughCamera;
                break;
            default:
                break;
        }

        this._gameScene = scene;
        if (_currentCamera != null)
        {
            _currentCamera.EnableCamera();
        }
    }

    public void Move(Vector2 input)
    {
        _currentCamera.Move(Time.deltaTime, input);
    }

    public void Rotate(Vector2 input)
    {
        _currentCamera.Rotate(Time.deltaTime, input);
    }

    public void Scale()
    {
        _currentCamera.Scale();
    }

    public void SetCameraFocus(Transform target, Action callBack)
    {
        if (this._gameScene == GameScene.Panorama)
        {
            _overViewCamera.SetFocus(target, callBack);
        }
    }

    public void SetCameraUnFocus(Action callBack)
    {
        if (this._gameScene == GameScene.Panorama)
        {
            _overViewCamera.UnFocus(callBack);
        }
    }
}
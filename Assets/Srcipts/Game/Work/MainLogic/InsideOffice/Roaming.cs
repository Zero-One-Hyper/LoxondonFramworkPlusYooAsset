using System;
using System.Text;
using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Roaming : MonoBehaviour
{
    private ICameraService _cameraService;
    private IInputService _inputService;
    private IViewService _viewService;
    private Camera _mainCamera;
    private Transform _cameraFollowPoint;
    private Transform _player;
    private AutoRoaming _autoRoaming;

    private float _moveSpeed = 1.5f;
    private Vector2 _rotateSpeed = new Vector2(20f, 10f); //旋转速度 水平 竖直
    private Vector2 _rotateVerticalLimit = new Vector2(-10f, 20f); //角度显示 仰角 俯角

    private Vector2 _moveInput;
    private Vector3 _moveDestination;
    private Vector2 _rotateInput;
    private float _verticalRotate;
    private float _deltaTime;

    private void Awake()
    {
        this._mainCamera = Camera.main;
        this._player = this.transform.GetChild(0);
        this._cameraFollowPoint = this._player.GetChild(0);
        _verticalRotate = _cameraFollowPoint.eulerAngles.x;

        var context = Context.GetApplicationContext();
        _cameraService = context.GetService<ICameraService>();
        _inputService = context.GetService<IInputService>();
        _viewService = context.GetService<IViewService>();
    }

    private CharacterController _characterController;

    public void Init()
    {
    }

    private void Start()
    {
        RegistInputAction();
        _autoRoaming = this._player.gameObject.AddComponent<AutoRoaming>();
        _characterController = _player.GetComponent<CharacterController>();

        _autoRoaming.Init(this);

#if !UNITY_EDITOR || UNITY_WEBGL
        this._rotateSpeed *= 0.2f;
#endif

#if UNITY_EDITOR || !UNITY_WEBGL
        this._rotateSpeed *= 1.5f;
#endif
    }

    private void RegistInputAction()
    {
        _inputService.RegisterCameraControl(InputActionType.Move, GameScene.Roaming, MoveInput);
        _inputService.RegisterCameraControl(InputActionType.Rotate, GameScene.Roaming, RotateInput);
    }

    public void EnterRoamingMode()
    {
        OnEnableRoaming();
        StopAutoRoaming();
    }

    private void OnEnableRoaming()
    {
        _cameraService.SetCameraFollowTarget(_cameraFollowPoint);
    }

    public void ExitRoamingMode()
    {
        //决定是否要重置角度
        _autoRoaming.StopAutoRoaming();
    }

    private void MoveInput(Vector2 input)
    {
        _moveInput = input.normalized;
    }

    private void RotateInput(Vector2 input)
    {
        //Debug.Log(input);
        this.Rotate(_deltaTime, input);
    }

    private void Update()
    {
        this._deltaTime = Time.deltaTime;
        this.Move(_deltaTime, _moveInput);
    }

    //移动
    private void Move(float deltaTime, Vector2 moveDir)
    {
        if (!AutoRoaming.IsAutoRoaming && moveDir.magnitude > 0.1f)
        {
            var moveDirection = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            var inputForward = moveDirection * moveDir.y + _mainCamera.transform.right * moveDir.x;
            var currentPosition = _player.transform.position;
            _moveDestination = currentPosition + inputForward * _moveSpeed;
            _moveDestination = Vector3.Lerp(currentPosition, _moveDestination, 1.5f * deltaTime);
            _autoRoaming.Move(_moveDestination - currentPosition);
        }
    }

    private void Rotate(float deltaTime, Vector2 input)
    {
        if (!AutoRoaming.IsAutoRoaming && input.magnitude >= 0.02f)
        {
            //水平转player
            _player.Rotate(Vector3.up, input.x * _rotateSpeed.x * deltaTime);
            _verticalRotate += -_rotateSpeed.y * input.y * deltaTime;
            _verticalRotate = Mathf.Clamp(_verticalRotate, _rotateVerticalLimit.x, _rotateVerticalLimit.y);

            _cameraFollowPoint.eulerAngles = new Vector3(_verticalRotate, _cameraFollowPoint.eulerAngles.y, 0);
        }
    }

    public void TryContimueAutoAroming()
    {
        this._autoRoaming.TryContimueAutoAroming();
    }

    public void TryPauseAutoAroming()
    {
        this._autoRoaming.WaitUntilDoorOpen();
    }

    public void SetAutoRoamingTarget(Vector3 guildPoiont, Vector3 doorPosition)
    {
        this._autoRoaming.SetDirection(guildPoiont, doorPosition);
    }

    public Vector3 GetAutoRoamingDirection()
    {
        return this._autoRoaming.GetAutoRoamingDirection();
    }

    public void OnRoomNotAllowVisit()
    {
        this._autoRoaming.OnRoomNotAllowVisit();
    }

    public void StopAutoRoaming()
    {
        if (_autoRoaming != null)
        {
            _autoRoaming.StopAutoRoaming();
        }
    }
    public void LookAtTarget(Vector3 position, Action callBack = null)
    {
        this._player.DOLookAt(position, 0.8f, AxisConstraint.Y).OnComplete(() =>
        {
            callBack?.Invoke();
        });
    }

}
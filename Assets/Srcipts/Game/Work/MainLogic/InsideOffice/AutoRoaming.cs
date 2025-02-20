using System.Collections;
using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;
using UnityEngine.AI;

public class AutoRoaming : MonoBehaviour
{
    public static bool IsAutoRoaming = false;
    private NavMeshPath _path;
    private IEffectService _effectService;
    private IViewService _viewService;
    private INavigationService _navigationService;
    private Roaming _roaming;
    private CharacterController _characterController;

    [SerializeField]
    private bool _isNavAgentControl = false;
    private int _nextPointIndex = 0;
    private bool _isWaitingDoorOpen = false;

    private float _calTime = 0;
    private Vector3 _currentPosition;
    private Vector3 _lastPosition;
    private Vector3 _currentGuildRoomDoorPositoin;

    public void Init(Roaming roaming)
    {
        _roaming = roaming;
    }

    private void Awake()
    {
        _path = new NavMeshPath();
        _characterController = this.GetComponent<CharacterController>();
    }

    private void Start()
    {
        _effectService = Context.GetApplicationContext().GetService<IEffectService>();
        _viewService = Context.GetApplicationContext().GetService<IViewService>();
        _navigationService = Context.GetApplicationContext().GetService<INavigationService>();
    }

    public void SetDirection(Vector3 guildPoiont, Vector3 doorPosition)
    {
        SetRoamingDirection(guildPoiont, doorPosition);
    }

    private void SetRoamingDirection(Vector3 guildPoiont, Vector3 doorPosition)
    {
        //等待两帧 保证导航障碍物刷新
        IsAutoRoaming = true;
        _nextPointIndex = 0;
        //设置终点的门的位置
        _currentGuildRoomDoorPositoin = doorPosition;
        //引导线
        var path = _navigationService.GeneratePathOnly(guildPoiont);
        _path = path;
        _effectService.ShowGuildLine(_path);
        _isNavAgentControl = true;
        _isWaitingDoorOpen = false;
    }

    public Vector3 GetAutoRoamingDirection()
    {
        return (_currentPosition - _lastPosition).normalized;
    }

    private void DoAutoRoaming(float deltaTime)
    {
        if (_isWaitingDoorOpen)
        {
            return;
        }
        Vector3 destination = _path.corners[_nextPointIndex];
        Vector3 currentPosition = this.transform.position;
        currentPosition.y = destination.y;
        if (Vector3.Distance(destination, currentPosition) > .1f)
        {
            Vector3 direction = (destination - currentPosition).normalized;
            //当前位置+归一化方向向量*速度 1.5f
            Vector3 moveDestination = this.transform.position + direction * 1.5f;
            moveDestination = Vector3.Lerp(this.transform.position, moveDestination, 1.5f * deltaTime);
            this.Move(moveDestination - this.transform.position);
        }
        else
        {
            //切换下一点
            _nextPointIndex++;
            //判断是否到达终点]
            if (_nextPointIndex >= _path.corners.Length)
            {
                StopAutoRoaming();
                IsAutoRoaming = true;
                //旋转摄像机 看向房门
                //判段到达的条件存在0.1f的误差 计算到终点的向量，在旋转看向门时，需要减去这个误差
                Vector3 lastPoint = _path.corners[_nextPointIndex - 1];//最后一个点
                lastPoint.y = this.transform.position.y;
                Vector3 playerToTargetOffset = lastPoint - this.transform.position;
                _roaming.LookAtTarget(_currentGuildRoomDoorPositoin - playerToTargetOffset, () =>
                {
                    //到达目的地    显示消息
                    var guildService = Context.GetApplicationContext().GetService<IGuildTourService>();
                    guildService.OnArriveRoamingDesitnation();

                    IsAutoRoaming = false;
                });
                return;
            }
        }
        //没走到最终点前修正朝向
        if (_nextPointIndex < _path.corners.Length)
        {
            Vector3 target = _path.corners[_nextPointIndex];
            target.y = this.transform.position.y;
            Vector3 playerToTarget = target - this.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
            targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
            transform.Rotate(Vector3.up, targetRotation.eulerAngles.y - transform.eulerAngles.y);
        }

        return;
    }
    private void FixedUpdate()
    {
        if (_isNavAgentControl)
        {
            DoAutoRoaming(Time.deltaTime);
        }

        _calTime += Time.deltaTime;
        if (_calTime > 3f)
        {
            _calTime = 0;
            _lastPosition = _currentPosition;
            _currentPosition = this.transform.position;
        }
    }

    public void StopAutoRoaming()
    {
        if (!_isNavAgentControl)
        {
            return;
        }
        this.Stop();
        _isNavAgentControl = false;
        IsAutoRoaming = false;
        //关闭导航线
        _effectService.HideGuildLine();

        //发送消息告诉前端结束自动漫游
        var webService = Context.GetApplicationContext().GetService<IWebService>();
        webService.UnityCallWeb(Constant.UNITY_SEND_SWITCH_AUTOROAMING, "false");
        XLog.I("发送停止自动漫游消息");
    }

    public void Move(Vector3 offset)
    {
        _characterController.Move(offset);
    }

    public void Stop()
    {
        if (this._isNavAgentControl)
        {
            _isNavAgentControl = false;
        }
    }

    public void WaitUntilDoorOpen()
    {
        if (IsAutoRoaming)
        {
            _isWaitingDoorOpen = true;
        }
    }
    public void TryContimueAutoAroming()
    {
        if (IsAutoRoaming)
        {
            _isNavAgentControl = true;
            _isWaitingDoorOpen = false;
        }
    }

    public void OnRoomNotAllowVisit()
    {
        StopAutoRoaming();
    }
}
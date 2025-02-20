using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Loxodon.Framework.Contexts;
using LP.Framework;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour, IInteractive
{
    [Serializable]
    public enum DoorType
    {
        SwingDoors = 0, //平开门
        SlidingDoor = 1, //推拉门
    }

    [SerializeField] protected DoorType _doorType;
    [SerializeField] protected string _roomId; //门开启房间内的ID
    protected string _doorName;
    protected bool _requirePermission;
    [SerializeField] protected int _doorID;

    protected float _hypotenuseLength;

    //protected NavMeshModifier _modifier;
    protected Vector3 _doorForward;
    [SerializeField] protected Vector3 _vetorInsideRoom;

    protected IInteractCollider _interactCollider;

#if UNITY_EDITOR
    public DoorCollider doorCollider;
#endif

    protected bool _isOpen = false;
    protected Animator _animator;
    protected Vector3 _defaultRotate;
    protected Camera _mainCamera;
    protected DoorController _controller;
    protected BoxCollider _doorCollider; //阻碍移动的Colider

    protected float _animLength;

    //特殊开门加载工位事件
    protected Func<bool> _doorEnterCallBack;

    //开门完成回调
    protected Action _doorOpenCompleteCallBack;
    protected Action _doorCloseCompleteCallBack;
    protected Action _enterDoorCallBack;
    protected Action _onDoorPlayOpenCallBack;
    protected Action _onExitDoorAreaCallBack;

    public void Init(DoorController controller, int index)
    {
        this._doorID = index;
        _controller = controller;
        //_modifier = this.GetComponent<NavMeshModifier>();
    }

    protected virtual void Start()
    {
        _mainCamera = Camera.main;
        _defaultRotate = this.transform.eulerAngles;
        if (!this.TryGetComponent(out _doorCollider))
        {
            _doorCollider = this.transform.GetChild(0).GetComponent<BoxCollider>();
        }
    }

    public virtual void SetData(DoorController.DoorColliderBind doorBindData,
        DoorCollider triggerCollider) //NavMeshModifier modifier)
    {
        this._doorType = doorBindData.doorType;
        this._doorForward = new Vector3(doorBindData.forwardX, doorBindData.forwardY, doorBindData.forwardZ);
        this._vetorInsideRoom = new Vector3(doorBindData.vetorInsideRoomX,
            doorBindData.vetorInsideRoomY, doorBindData.vetorInsideRoomZ);
        this._roomId = doorBindData.roomId;
        this._requirePermission = doorBindData.requirePermission;
        _interactCollider = triggerCollider as IInteractCollider;
        //this._modifier = modifier;
        this._doorName = doorBindData.doorName;

#if UNITY_EDITOR
        this.doorCollider = triggerCollider;
#endif
        _interactCollider.SetOwner(this);
    }

    //开门触发事件（用在了加载电脑上）
    public void AddDoorEnterCallBack(Func<bool> callBack)
    {
        _doorEnterCallBack += callBack;
    }

    public void AddDoorOpenCompleteCallBack(Action callBack)
    {
        this._doorOpenCompleteCallBack += callBack;
    }

    public void AddEnterDootCallBack(Action callBack)
    {
        this._enterDoorCallBack += callBack;
    }

    public void AddDoorPlayOpenAnim(Action callBack)
    {
        this._onDoorPlayOpenCallBack += callBack;
    }

    public void AddExitDoorCallBack(Action callBack)
    {
        this._onExitDoorAreaCallBack += callBack;
    }

    public virtual void Interactive(InterActiveArgs data)
    {
        if (data.doorTriggerType == DoorTriggerType.Exit)
        {
            TriggerExit(data.vector3);
            return;
        }
        else if (data.doorTriggerType == DoorTriggerType.Enter)
        {
            //设置碰撞体最大斜边半径
            this._hypotenuseLength = this._doorType == DoorType.SwingDoors ? data.floatValue : 2.0f;
            TriggerEnter(data.gameObject);
            return;
        }
        else if (data.doorTriggerType == DoorTriggerType.Stay)
        {
            //Stay
            TriggerStay(data.gameObject);
            return;
        }
    }

    protected virtual void TriggerEnter(GameObject collider)
    {
        if (_isOpen)
        {
            //重置计时
            this.StopAllCoroutines();
            this.transform.DOPause();
            this._isOpen = true;
            return;
        }

        _enterDoorCallBack?.Invoke();
        if (!AutoRoaming.IsAutoRoaming)
        {
            //不在自动漫游时
            //判断是否可以开门
            if (!_controller.CanOpenDoor(collider, this._doorForward, 0.85f))
            {
                return;
            }
        }
        else
        {
            //判断当前这扇门是否需要判断开关
            int roomIndex;
            if (string.IsNullOrEmpty(this._roomId))
            {
                roomIndex = -2;
            }
            else
            {
                roomIndex = Convert.ToInt32(this._roomId);
            }

            if (!_controller.CanOpenDoor(roomIndex))
            {
                //自动漫游过程中 碰到门 门不可开
                return;
            }
        }

        if (_doorEnterCallBack != null && !_doorEnterCallBack.Invoke())
        {
            XLog.E("开门回调执行失败");
            return;
        }

        //计算是否可以触发
        DoCheckPermission();
    }

    protected virtual void TriggerStay(GameObject collider)
    {
        if (_isOpen)
        {
            return;
        }

        //Stay过程中
        if (!AutoRoaming.IsAutoRoaming)
        {
            //不在自动漫游时
            //判断是否可以开门
            if (!_controller.CanOpenDoor(collider, this._doorForward, 0.9f))
            {
                return;
            }
        }
        else
        {
            //判断当前这扇门是否需要判断开关
            int roomIndex;
            if (string.IsNullOrEmpty(this._roomId))
            {
                roomIndex = -2;
            }
            else
            {
                roomIndex = Convert.ToInt32(this._roomId);
            }

            if (!_controller.CanOpenDoor(roomIndex))
            {
                //自动漫游过程中 碰到门 门不可开
                return;
            }
        }

        if (_doorEnterCallBack != null && !_doorEnterCallBack.Invoke())
        {
            XLog.E("开门回调执行失败");
            return;
        }

        _enterDoorCallBack?.Invoke();

        //计算是否可以触发
        if (_controller.CanSendOpenDoorMessage(this.gameObject) || AutoRoaming.IsAutoRoaming)
            //输入部分按下w 且摄像机方向和门方向同向 或者自动漫游时
        {
            DoCheckPermission();
        }
    }

    protected virtual void TriggerExit(Vector3 colliderPosition)
    {
        //离开碰撞体时 检测移动方向 以移动方向判断是否离开房间
        _controller.OnTriggerExitADoor(this._vetorInsideRoom, this._roomId, colliderPosition,
            this._doorName == "MeetingRoomDoor");
        CloseDoor();
        _onExitDoorAreaCallBack?.Invoke();
    }

    public void OpenDoor()
    {
        if (_isOpen)
        {
            return;
        }

        this.DoOpen();
    }

    public void CloseDoor()
    {
        if (!_isOpen)
        {
            return;
        }

        this.DoClose();
    }

    //检查权限
    protected void DoCheckPermission()
    {
        if (this._doorID == 7 || this._doorID == 8)
        {
            //7号8号门特殊，只作为装饰
            this.OpenDoor();
            return;
        }

        //找Controller判断权限
        this._controller.OnTriggerEnterADoor(this._doorID, this._roomId);
    }

    protected void GetDoorPermission()
    {
        //if (this._modifier == null)
        //{
        //    _modifier = GetComponent<NavMeshModifier>();
        //}
        //
        //_modifier.ignoreFromBuild = true;
        //_modifier.overrideArea = false;
        //_modifier.area = 0;
    }

    protected void DoOpen()
    {
        if (_isOpen)
        {
            return;
        }

        _onDoorPlayOpenCallBack?.Invoke();
        switch (_doorType)
        {
            case DoorType.SwingDoors:
                PlaySwingDoorOpenAnim();
                break;
            case DoorType.SlidingDoor:
                StartCoroutine(PlaySlidingDoorOpenAnim());
                break;
        }

        this._isOpen = true;
    }

    protected void DoClose()
    {
        switch (_doorType)
        {
            case DoorType.SwingDoors:
                PlaySwingDoorCloseAnim();
                break;
            case DoorType.SlidingDoor:
                StartCoroutine("PlaySlidingDoorCloseAnim");
                break;
        }
    }

    protected virtual void PlaySwingDoorOpenAnim()
    {
        Vector3 collideDir = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up);
        //碰撞方向和forward同向 旋转-100°
        float angle = Vector3.Angle(this._doorForward, collideDir);
        if (angle < 45f || (AutoRoaming.IsAutoRoaming && angle < 90))
        {
            //同向
            this.transform.DORotate(new Vector3(0, -90f, 0), 0.35f).OnComplete(() =>
            {
                this.GetDoorPermission();

                if (!AutoRoaming.IsAutoRoaming)
                {
                    var inputService = Context.GetApplicationContext().GetService<IInputService>();
                    inputService.EnableMoveInput();
                }

                _doorOpenCompleteCallBack?.Invoke();
                _doorCollider.enabled = false;
            });
        }
        else if (angle > 135f || (AutoRoaming.IsAutoRoaming && angle > 90))
        {
            //反向
            this.transform.DORotate(new Vector3(0, 90f, 0), 0.35f).OnComplete(() =>
            {
                this.GetDoorPermission();

                if (!AutoRoaming.IsAutoRoaming)
                {
                    var inputService = Context.GetApplicationContext().GetService<IInputService>();
                    inputService.EnableMoveInput();
                }

                _doorOpenCompleteCallBack?.Invoke();
                _doorCollider.enabled = false;
            });
        }
    }

    protected void PlaySwingDoorCloseAnim()
    {
        float timer = 0;
        DOTween.To(() => timer, x => timer = x, 1f, 2.0f).OnComplete(() =>
            this.transform.DORotate(_defaultRotate, 0.35f).OnComplete(() =>
            {
                this._isOpen = false;
                _doorCollider.enabled = true;
            }));
    }

    protected IEnumerator PlaySlidingDoorOpenAnim()
    {
        _animator.SetTrigger("OpenDoor");

        yield return new WaitForSecondsRealtime(_animLength);

        this.GetDoorPermission();
        _doorOpenCompleteCallBack?.Invoke();

        //关华滑动门
        PlaySlidingDoorCloseAnim();
    }

    protected IEnumerator PlaySlidingDoorCloseAnim()
    {
        yield return new WaitForSeconds(2.0f);
        _animator.SetTrigger("CloseDoor");

        yield return new WaitForSeconds(_animLength);

        this._isOpen = false;
    }

    public string GetRoomId()
    {
        return _roomId;
    }
#if UNITY_EDITOR
    public void SetDataEditorMode(DoorController.DoorColliderBind doorBindData, DoorCollider doorCollider)
    {
        this._doorType = doorBindData.doorType;
        this._doorForward = new Vector3(doorBindData.forwardX, doorBindData.forwardY, doorBindData.forwardZ);
        this._roomId = doorBindData.roomId;
        this._requirePermission = doorBindData.requirePermission;
        _interactCollider = doorCollider;
        this.doorCollider = doorCollider;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, _doorForward);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, _vetorInsideRoom);
    }

    public bool GetRequirePassword()
    {
        return this._requirePermission;
    }

    public NavMeshModifier GetModifier()
    {
        //return this._modifier;
        return null;
    }

    public DoorType GetDoorType()
    {
        return this._doorType;
    }

    public Vector3 GetDoorForward()
    {
        return this._doorForward;
    }
#endif
}
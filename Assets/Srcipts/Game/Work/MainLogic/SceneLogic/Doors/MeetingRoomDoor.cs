using System;
using Loxodon.Framework.Contexts;
using LP.Framework;
using Unity.AI.Navigation;
using UnityEngine;

public class MeetingRoomDoor : Door
{
    public static bool IsInLinkedMeetingRoom = false;
    private BoxCollider _meetingRoomCollider; //判断是否在meetingRoom的Collider 在门的判断Coolider上
    private BoxCollider _meetingRoomDoorCollider; //手动漫游时阻挡palyer
    private Vector3 _defaultColliderCenter;
    private Vector3 _defaultColliderSize;
    private Vector3 _colliderCenter = new Vector3(-0.9435039f, 0f, -0.6971083f);
    private Vector3 _colliderSize = new Vector3(18.65255f, 2.3f, 10.51354f);
    private bool _isMeetingRoomLinked = false; //会议室是否正在链接状态

    protected override void Start()
    {
        //base.Start();
        this.transform.TryGetComponent(out _animator);
        _meetingRoomDoorCollider = this.transform.GetChild(this.transform.childCount - 1).GetComponent<BoxCollider>();
        foreach (var anim in _animator.runtimeAnimatorController.animationClips)
        {
            if (anim.name.Equals("BigMeetingRoom") || anim.name.Equals("SlidingDoorOpen"))
            {
                this._animLength = anim.length;
                break;
            }
        }

        //会议室隔断门打开完毕后触发
        this.AddDoorOpenCompleteCallBack(() =>
        {
            this._isMeetingRoomLinked = true;
            IsInLinkedMeetingRoom = true;
            //开门后Collider变为扩散至两个会议室大小
            _meetingRoomCollider.center = _colliderCenter;
            _meetingRoomCollider.size = _colliderSize;
            _meetingRoomDoorCollider.gameObject.SetActive(false);
        });
    }

    public override void SetData(DoorController.DoorColliderBind doorBindData,
        DoorCollider triggerCollider) //,NavMeshModifier modifier)
    {
        base.SetData(doorBindData, triggerCollider); //, modifier);
        this._meetingRoomCollider = triggerCollider.GetComponent<BoxCollider>();
        //配置默认collider大小
        this._defaultColliderCenter = _meetingRoomCollider.center;
        this._defaultColliderSize = _meetingRoomCollider.size;
    }

    public override void Interactive(InterActiveArgs data)
    {
        //是否由外部的Collider控制开合
        if (data.doorTriggerType == DoorTriggerType.Exit)
        {
            TriggerExit(data.vector3);
            return;
        }
        //自动漫游下 会议室的门不需要Enter和Stay互动检查
        else if (data.doorTriggerType == DoorTriggerType.Enter && !AutoRoaming.IsAutoRoaming)
        {
            //设置碰撞体最大斜边半径
            this._hypotenuseLength = this._doorType == DoorType.SwingDoors ? data.floatValue : 2.0f;
            TriggerEnter(data.gameObject);
            return;
        }
        else if (data.doorTriggerType == DoorTriggerType.Stay && !AutoRoaming.IsAutoRoaming)
        {
            //Stay
            TriggerStay(data.gameObject);
            return;
        }
    }

    protected override void TriggerEnter(GameObject collider)
    {
        base.TriggerEnter(collider);
    }

    protected override void TriggerStay(GameObject collider)
    {
        //会议室隔断门Stay过程中 doorForward不能用于判断是否可以开门
        if (_isOpen)
        {
            return;
        }
        //Stay过程中

        //自动漫游时不会触发Stay
        //根据player位置与Door的连线向量 和doorForward 确定是否要反转doorForward
        Vector3 doorToPlayer = (collider.transform.position - this.transform.position).normalized;
        //Debug.Log(collider.gameObject.name);
        doorToPlayer.y = 0;
        if (Vector3.Dot(doorToPlayer, this._doorForward) < 0)
        {
            //Debug.Log(123);
            //player在门doorForward指向的一侧
            if (!_controller.CanOpenDoor(collider, this._doorForward, 0.9f))
            {
                return;
            }
        }
        else
        {
            //Debug.Log(321);
            //player在另一侧
            //判断是否可以开门  
            if (!_controller.CanOpenDoor(collider, -this._doorForward, 0.9f))
            {
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

    protected override void TriggerExit(Vector3 vector3)
    {
        base.TriggerExit(vector3);
        if (_isMeetingRoomLinked)
        {
            //离开会议室门范围后
            this._isMeetingRoomLinked = false;
            IsInLinkedMeetingRoom = false;
            //离开会议室后 collider变回默认大小
            _meetingRoomCollider.center = _defaultColliderCenter;
            _meetingRoomCollider.size = _defaultColliderSize;
            _meetingRoomDoorCollider.gameObject.SetActive(true);
        }
    }
}
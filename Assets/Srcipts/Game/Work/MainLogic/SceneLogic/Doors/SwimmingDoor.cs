using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public class SwimmingDoor : Door
{
    private IIPService _iPService;
    private IInputService _inputService;
    private BoxCollider _elevatorRoomCollider;//判断是否在电梯间内的Collider 在Door的模型上
    private Vector3 _colliderCenter = new Vector3(5.9f, 0.10221f, -0.08f);
    private Vector3 _colliderSize = new Vector3(11.59881f, 3.162384f, 5.106248f);
    private bool _isInElevatorRoom = true;//是否在电梯间内

    protected override void Start()
    {
        //base.Start();
        _defaultRotate = this.transform.eulerAngles;
        this.transform.TryGetComponent(out _animator);
        foreach (var anim in _animator.runtimeAnimatorController.animationClips)
        {
            if (anim.name.Equals("BigMeetingRoom") || anim.name.Equals("SlidingDoorOpen"))
            {
                this._animLength = anim.length;
                break;
            }
        }
        
        _iPService = Context.GetApplicationContext().GetService<IIPService>();
        _inputService = Context.GetApplicationContext().GetService<IInputService>();

        _elevatorRoomCollider = this.gameObject.AddComponent<BoxCollider>();
        _elevatorRoomCollider.center = _colliderCenter;
        _elevatorRoomCollider.size = _colliderSize;
        _elevatorRoomCollider.isTrigger = true;
        var rig = this.gameObject.AddComponent<Rigidbody>();
        rig.useGravity = false;
        rig.isKinematic = true;


        //电梯大门打开完毕后启动IP
        this.AddDoorOpenCompleteCallBack(() =>
        {
            if (this._isInElevatorRoom)
            {
                //关闭移动输入
                _inputService.DisableMoveInput();
                //在电梯间时才会执行                
                _iPService.ShowIPAnim(() =>
                {
                    this.OnIPSwitchComplte();
                });
            }
        });
        this.AddDoorPlayOpenAnim(() =>
        {
            if (this._isInElevatorRoom)
            {
                //在电梯间内的时候触发
                //关闭移动输入
                _inputService.DisableMoveInput();
            }
        });
    }

    public void OnIPSwitchComplte()
    {
        //IP切换完成，打开控制
        _inputService.EnableMoveInput();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_isInElevatorRoom)
        {
            _isInElevatorRoom = true;
            //进入空间关闭IP
            _iPService.PlayIPTurnOffAnim();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(_isInElevatorRoom);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isInElevatorRoom)
        {

            _isInElevatorRoom = false;
            //   Debug.Log(1919810);
        }
    }

}

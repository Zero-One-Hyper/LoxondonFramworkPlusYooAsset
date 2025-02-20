using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Loxodon.Framework.Contexts;
using LP.Framework;
using Unity.AI.Navigation;
using UnityEngine;
using YooAsset;

public interface IDoorController
{
    void Init();
    void OpenDoor(bool openDoor);
    void ClearCheckDoorPermission();
    List<Door> GetDoorByRoomID(int roomID);
}

public class DoorController : IDoorController
{
    [Serializable]
    public class DoorColliderData
    {
        public List<DoorColliderBind> allDoorBingData = new List<DoorColliderBind>();
    }

    [Serializable]
    public class DoorColliderBind
    {
        public string doorName;
        public string colliderPath;
        public string navMeshModifierPath;
        public Door.DoorType doorType;
        public string roomId;
        public bool requirePermission;
        public float forwardX;
        public float forwardY;
        public float forwardZ;
        public float vetorInsideRoomX;
        public float vetorInsideRoomY;
        public float vetorInsideRoomZ;
    }

    private ApplicationContext _context;
    private IAssetLoadUtil _assetLoad;
    private ISceneGameObjectService _sceneGameObjectService;
    private INavigationService _navigationService;
    private IPermissionsManagement _permissionsManagement;
    private ISceneLogicService _sceneLogicService;
    private IInputService _inputcontroller;
    private IViewService _viewService;
    private IRoomService _roomService;
    private DoorColliderData _doorColliderBindData;
    private List<Door> _allDoors = new List<Door>();
    private List<int> _doorNeedChickPermissions = new List<int>();
    private Dictionary<int, List<Door>> _allRoomDoor; //所有房间对应的门

    private Camera _mainCamera;
    private GameObject _doorGameObject;
    private GameObject _doorColliderGameObject;
    private Vector2 _moveInputDir;

    public async void Init()
    {
        _context = Context.GetApplicationContext();

        _navigationService = _context.GetService<INavigationService>();
        _permissionsManagement = _context.GetService<IPermissionsManagement>();
        _sceneLogicService = _context.GetService<ISceneLogicService>();
        _inputcontroller = _context.GetService<IInputService>();
        _viewService = _context.GetService<IViewService>();
        _roomService = _context.GetService<IRoomService>();

        _inputcontroller.RegisterCameraControl(InputActionType.Move, GameScene.Roaming, (Vector2 dir) =>
        {
            _moveInputDir = dir;
        });

        _assetLoad = _context.GetService<IAssetLoadUtil>();
        _sceneGameObjectService = _context.GetService<ISceneGameObjectService>();

        //加载门Collider绑定数据
        var textAsset = await _assetLoad.ResourceLoadAsync<TextAsset>("DoorColliderBingConfig");
        _doorColliderBindData = JsonUtility.FromJson<DoorColliderData>(textAsset.text);
        //拿到门模型
        _doorGameObject = await _sceneGameObjectService.TryGetSceneGameObject("Doors");
        //拿到门Collider
        _doorColliderGameObject = await _sceneGameObjectService.TryGetSceneGameObject("DoorColliders");

        //绑定Collider
        Dictionary<string, DoorCollider> colliders = new Dictionary<string, DoorCollider>();
        for (int i = 0; i < _doorColliderGameObject.transform.childCount; i++)
        {
            var collider = _doorColliderGameObject.transform.GetChild(i).gameObject;
            colliders.Add(collider.name, collider.AddComponent<DoorCollider>());
        }

        for (int i = 0; i < _doorColliderBindData.allDoorBingData.Count; i++)
        {
            Door door;
            if (i == 0)
            {
                door = _doorGameObject.transform.GetChild(i).gameObject.AddComponent<MeetingRoomDoor>();
            }
            else if (i == 1)
            {
                door = _doorGameObject.transform.GetChild(i).gameObject.AddComponent<SwimmingDoor>();
            }
            else
            {
                door = _doorGameObject.transform.GetChild(i).gameObject.AddComponent<Door>();
            }

            DoorColliderBind doorBindData = _doorColliderBindData.allDoorBingData[i];
            DoorCollider collider = colliders[doorBindData.colliderPath];

            door.Init(this, i);
            if (i == 0)
            {
                MeetingRoomDoor meetingRoomDoor = door as MeetingRoomDoor;
                meetingRoomDoor.SetData(doorBindData, collider);
            }
            else
            {
                door.SetData(doorBindData, collider);
            }

            _allDoors.Add(door);
            if (i == 1)
            {
                SwimmingDoor swimmingDoor = door as SwimmingDoor;
                //1号门是向两边开的门，尝试在这扇门第一次打开时加载工位
                swimmingDoor.AddDoorEnterCallBack(() =>
                {
                    _context.GetService<IEmployeeService>().InitEmployeeCubicleData();
                    return true;
                });
                swimmingDoor.AddDoorEnterCallBack(() =>
                {
                    return _sceneLogicService.InitAds();
                });
            }

            //所有门添加自动漫游开门后再继续移动的回调
            door.AddDoorOpenCompleteCallBack(() =>
            {
                _sceneLogicService.TryContimueAutoAroming();
            });
            door.AddEnterDootCallBack(() =>
            {
                _sceneLogicService.TryPauseAutoAroming();
            });
        }

        _mainCamera = Camera.main;
    }



    public void OnTriggerEnterADoor(int doorIndex, string roomId)
    {
        //检查权限 
        CheckPermission(doorIndex, Convert.ToInt32(roomId));
    }

    public void OpenDoor(bool openDoor)
    {
        foreach (int doorIndex in _doorNeedChickPermissions)
        {
            //Debug.Log(doorIndex);
            if (openDoor)
            {
                _allDoors[doorIndex].OpenDoor();
            }
            else
            {
                _allDoors[doorIndex].CloseDoor();
            }
        }

        this._doorNeedChickPermissions.Clear();
    }

    public void ReBakeNavMeshSurface()
    {
        //_navigationService.UpdateNavSurface();
    }

    //用于自动漫游
    public bool CanOpenDoor(int roomIndex)
    {
        return _roomService.IsInRoamingPath(roomIndex);
    }

    public bool CanOpenDoor(GameObject collider, Vector3 doorForward, float angleLimit)
    {
        if (Vector3.Dot(_mainCamera.transform.forward, doorForward) < 0)
        {
            //确保平面向量与摄像机forward同向
            doorForward = -doorForward;
        }

        if (!AutoRoaming.IsAutoRoaming)
        {
            // 后退不开门 或不在自动漫游
            if (_moveInputDir.y <= 0)
            {
                return false;
            }
            //计算移动方向和门平面的法向量夹角
            //经过collider位置地 且法向量为doorForward的平面
            //Plane plane = new Plane(doorForward, collider.transform.position);

            //计算移动方向
            var direction = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
            var inputForward = direction * _moveInputDir.y + _mainCamera.transform.right * _moveInputDir.x;

            float angle = Vector3.Dot(doorForward.normalized, inputForward.normalized);
            //XLog.I(angle);
            return angle > angleLimit;
        }
        else
        {
            Vector3 autoMoveDirection = _sceneLogicService.GetAutoRoamingDirection();
            float angle = Vector3.Dot(doorForward.normalized, autoMoveDirection);
            //XLog.I($"自动漫游角度{angle}");
            return angle > angleLimit;
        }
    }

    public bool CanSendOpenDoorMessage(GameObject doorGameObject)
    {
        //计算从门到palyer的向量 判断是指向摄像机方向的还是背对摄像机方向
        if (_moveInputDir.y > 0.1f)
        {
            Vector3 playerToDoor = doorGameObject.transform.position - _mainCamera.transform.position;
            if (Vector3.Dot(playerToDoor.normalized, _mainCamera.transform.forward) < 0)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public void OnTriggerExitADoor(Vector3 vectorInsideRoom, string roomId, Vector3 doorPosition, bool isMeetingRoom)
    {
        int roomIndex;
        if (string.IsNullOrEmpty(roomId))
        {
            roomIndex = -1;
        }
        else
        {
            roomIndex = Convert.ToInt32(roomId);
        }
        //离开Collider后关闭UI
        //计算移动方向
        //var direction = Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized;
        //var inputForward = direction * _moveInputDir.y + _mainCamera.transform.right * _moveInputDir.x;
        //计算移动方向是否朝向房间内部
        //float angle = Vector3.Dot(vectorInsideRoom.normalized, inputForward.normalized);

        //判断此时物体在door平面的那一边
        Vector3 doorToPlayer = Vector3.ProjectOnPlane(_mainCamera.transform.position - doorPosition, Vector3.up)
            .normalized;
        float angle = Vector3.Dot(vectorInsideRoom.normalized, doorToPlayer);
        //会议室的方向指向11号房间 反方向时10号房间
        if (angle >= 0f)
        {
            //大于0.3(大约72°)认为向房间内移动 (与房间内向量同向)
            if (isMeetingRoom)
            {
                //会议室的方向指向11号房间
                _roomService.EnterRoom(11);
            }
            else
            {
                _roomService.EnterRoom(roomIndex);
            }
        }
        else //if (angle < -0.3f)//小于-0.3(大约108°)认为
        {
            if (isMeetingRoom)
            {
                //会议室 反方向时指向10号房间
                _roomService.EnterRoom(10);
            }
            else
            {
                _roomService.ExitRoom(roomIndex);
            }
        }

        ClearCheckDoorPermission();
        _permissionsManagement.CloseUIDoorPermission();
        _permissionsManagement.RemoveCheckPermission(roomIndex);
    }

    public void ClearCheckDoorPermission()
    {
        _doorNeedChickPermissions.Clear();
    }

    public List<Door> GetDoorByRoomID(int roomID)
    {
        if (_allRoomDoor == null)
        {
            this.FileRoomDoorData();
        }

        if (_allRoomDoor.TryGetValue(roomID, out List<Door> doors))
        {
            return doors;
        }

        return null;
    }

    private void FileRoomDoorData()
    {
        _allRoomDoor = new Dictionary<int, List<Door>>();
        foreach (var door in _allDoors)
        {
            string doorsRoomID = door.GetRoomId();
            if (string.IsNullOrEmpty(doorsRoomID))
            {
                doorsRoomID = "-1";
            }

            int roomID = Convert.ToInt32(doorsRoomID);
            if (_allRoomDoor.ContainsKey(roomID))
            {
                _allRoomDoor[roomID].Add(door);
            }
            else
            {
                _allRoomDoor[roomID] = new List<Door> { door };
            }
        }
    }

    //发送消息检查房间权限
    private void CheckPermission(int doorIndex, int roomId)
    {
        if (!_doorNeedChickPermissions.Contains(doorIndex))
        {
            _doorNeedChickPermissions.Add(doorIndex);
        }

        _permissionsManagement.SendChecktUserPermission(roomId, Time.realtimeSinceStartup);
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;


public interface IGuildTourService : IService
{
    void OpenMinMap();
    void OpenGuidlMap(string shouldOpen);
    void CloseeGuildMap();
    void GuildToRoom(int index);
    void OnArriveRoamingDesitnation();
}

public class GuildTourService : IGuildTourService
{
    private ApplicationContext _context;
    private IViewService _viewService;
    private ISceneLogicService _sceneLogicService;
    private IPermissionsManagement _permissionsManagement;
    private IWebService _webService;
    private IRoomService _roomService;
    private IDoorplateService _doorplateService;
    private IInputService _inputService;
    private List<RoomGuildPoint> _roomGuildPoint = new List<RoomGuildPoint>();
    public static Vector2 _mapSize = new Vector2(57.67f, 35.66f);
    private int _currentGuildRoomIndex;
    private bool _isToParentRoom = false;

    public void Init()
    {
        //打开小地图
        _context = Context.GetApplicationContext();
        _webService = _context.GetService<IWebService>();
        _roomService = _context.GetService<IRoomService>();
        _viewService = _context.GetService<IViewService>();
        _inputService = _context.GetService<IInputService>();

        var eventHandleService = _context.GetService<IEventHandleService>();
        eventHandleService.AddListener(SceneEvent.AutoRoaming, OnWebCallAutoRoaming);
        OpenMinMap();
        LoadGuildPointData();
    }

    public void OpenMinMap()
    {
        //拿到Player当前位置
        Func<Vector3> getPlayerCurrentPosition = GetPlayerCurrentPosition;
        Func<float> getPlayerEularAngleY = GetPlayerEularAngle;
        _viewService.OpenView<UIMinMap>(getPlayerCurrentPosition, getPlayerEularAngleY);
    }

    public void OpenGuidlMap(string shouldOpen)
    {
        if (shouldOpen == "true")
        {
            //停止自动寻路
            _sceneLogicService.StopAutoRoaming();
            //XLog.W("打开大地图");
            Func<Vector3> getPlayerCurrentPosition = GetPlayerCurrentPosition;
            Func<float> getPlayerEularAngleY = GetPlayerEularAngle;
            _viewService.OpenView<UIGuildTourWindow>(getPlayerCurrentPosition, getPlayerEularAngleY);
            _viewService.CloseView<UIEmployeeDataChart>();
            //关闭可能出现的门UI
            if (this._permissionsManagement == null)
            {
                _permissionsManagement = _context.GetService<IPermissionsManagement>();
            }

            _permissionsManagement.CloseUIDoorPermission();
            //关闭小地图
            _viewService.CloseView<UIMinMap>();
            XLog.I("发送打开导览图消息");
            _webService.UnityCallWeb(Constant.UNITY_CALL_NAVIGATION_MAP, "true");
        }
        else if (shouldOpen == "false")
        {
            CloseeGuildMap();
            _viewService.OpenView<UITipLeftDown>();
        }
    }


    private Vector3 GetPlayerCurrentPosition()
    {
        if (this._sceneLogicService == null)
        {
            _sceneLogicService = _context.GetService<ISceneLogicService>();
        }

        if (_sceneLogicService == null)
        {
            return Vector3.zero;
        }

        return _sceneLogicService.GetPlayerPosition();
    }

    private float GetPlayerEularAngle()
    {
        if (this._sceneLogicService == null)
        {
            _sceneLogicService = _context.GetService<ISceneLogicService>();
        }

        if (_sceneLogicService == null)
        {
            return -90;
        }

        return _sceneLogicService.GetPlayerForward();
    }


    public void CloseeGuildMap()
    {
        _viewService.CloseView<UIGuildTourWindow>();
        //打开小地图
        OpenMinMap();
        _webService.UnityCallWeb(Constant.UNITY_CALL_NAVIGATION_MAP, "false");
        XLog.I("发送关闭导览图消息");
    }

    public void GuildToRoom(int roomId)
    {
        if (this._roomGuildPoint == null || this._roomGuildPoint.Count == 0)
        {
            this.LoadGuildPointData();
            XLog.I("加载引导点数据");
        }

        //XLog.I("引导到房间" + roomId);

        //请求是否可以进入房间
        _currentGuildRoomIndex = roomId;

        if (this._sceneLogicService == null)
        {
            _sceneLogicService = _context.GetService<ISceneLogicService>();
        }

        if (this._doorplateService == null)
        {
            this._doorplateService = _context.GetService<IDoorplateService>();
        }

        string roomName = _doorplateService.GetSingleDoorplateData(roomId);
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "目的地";
        }

        StringBuilder sb = new StringBuilder("正在前往" + roomName);
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

        //生成途径的房间列表
        _roomService.GenerateElapseRoomList(_currentGuildRoomIndex);
        //正式开始导航
        DoGuildToRoom();
    }

    public void OnArriveRoamingDesitnation()
    {
        _viewService.OpenView<UITipLeftDown>();

        StringBuilder sb = new StringBuilder(_isToParentRoom ? "已到达目的地，请参观" : "已到达目的地，请开门参观");
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
    }

    private void DoGuildToRoom()
    {
        //开始导航时，判断角色现在在那个房间
        if (_roomService.IsInRoom(_currentGuildRoomIndex))
        {
            //当前选中的房间时指定的漫游目标房间
            StringBuilder sb = new StringBuilder("已位于目标房间");
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

            _viewService.OpenView<UITipLeftDown>();
            _inputService.EnableMoveInput();
            return;
        }

        var iPService = Context.GetApplicationContext().GetService<IIPService>();
        if (_roomService.IsInParentRoom(_currentGuildRoomIndex))
        {
            //当前选中的房间是当前房间的父房间  
            var room = _roomService.GetCurrentRoom();

            //移动向当前房间的房门 并看向当前房间的反方向
            Vector3 dir = _roomGuildPoint[room.roomIndex - 1].GetGuildPositoin() -
                          _roomGuildPoint[room.roomIndex - 1].GetDoorPosition();
            //看向的方向 路径点 + 方向
            Vector3 lookAtPositoin = dir.normalized + _roomGuildPoint[room.roomIndex - 1].GetGuildPositoin();
            RoomGuildPoint point = new RoomGuildPoint()
            {
                x = _roomGuildPoint[room.roomIndex - 1].x,
                y = _roomGuildPoint[room.roomIndex - 1].y,
                z = _roomGuildPoint[room.roomIndex - 1].z,
                doorPositionX = lookAtPositoin.x,
                doorPositionY = lookAtPositoin.y,
                doorPositionZ = lookAtPositoin.z,
            };
            //房间号从1开始 列表中从0开始
            _sceneLogicService.BeginAutoRoaming(point);
            ///打开IP
            iPService.SetRoamingIPModeActive(true);
            //关闭自由漫游提示
            _viewService.CloseView<UITipLeftDown>();
            //发送开始漫游消息
            _webService.UnityCallWeb(Constant.UNITY_SEND_SWITCH_AUTOROAMING, "true");
            XLog.I("发送开始漫游消息");
            //漫游至当前房间的父房间
            _isToParentRoom = true;
            return;
        }

        _isToParentRoom = false;
        //房间号从1开始 列表中从0开始
        if (_currentGuildRoomIndex == -1)
        {
            _sceneLogicService.BeginAutoRoaming(_roomGuildPoint[36]);
        }
        else
        {
            _sceneLogicService.BeginAutoRoaming(_roomGuildPoint[_currentGuildRoomIndex - 1]);
        }

        //打开IP
        iPService.SetRoamingIPModeActive(true);
        //关闭自由漫游提示
        _viewService.CloseView<UITipLeftDown>();
        //发送开始漫游消息
        _webService.UnityCallWeb(Constant.UNITY_SEND_SWITCH_AUTOROAMING, "true");
        XLog.I("发送开始漫游消息");
    }

    private async void LoadGuildPointData()
    {
        var assetLoad = Context.GetApplicationContext().GetService<IAssetLoadUtil>();
        //加载工位数据
        var textAsset = await assetLoad.ResourceLoadAsync<TextAsset>("RoomGuildPoints");

        if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
        {
            XLog.E("房间导航数据加载失败");
            return;
        }

        RoomGuildPoints allData = JsonUtility.FromJson<RoomGuildPoints>(textAsset.text);
        if (allData == null || allData.roomGuildPoints == null)
        {
            XLog.E("房间导航数据json反序列化失败");
            return;
        }

        List<RoomGuildPoint> datas = allData.roomGuildPoints;
        if (datas == null || datas.Count <= 0)
        {
            XLog.E("房间导航数据为空");
            return;
        }
        Debug.Log(123123);
        _roomGuildPoint = datas;
    }

    private void OnWebCallAutoRoaming(EventArgs args)
    {
        SceneEventArgs data = args as SceneEventArgs;
        if (data == null || string.IsNullOrEmpty(data.Data))
        {
            XLog.E("固定漫游切换参数为空");
        }

        //判断是否在自动漫游
        if (AutoRoaming.IsAutoRoaming)
        {
            //在固定路线漫游过程中点击 停止固定漫游
            _sceneLogicService.StopAutoRoaming();
            //提示漫游
            StringBuilder sb = new StringBuilder("已切换为自由漫游");
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

            //_viewService.OpenView<UITip>(sb);
            _viewService.OpenView<UITipLeftDown>();
            return;
        }

        OpenGuidlMap(data.Data);
    }
}

[Serializable]
public class RoomGuildPoints
{
    public List<RoomGuildPoint> roomGuildPoints;

    public List<Vector3> GetAllData()
    {
        List<Vector3> result = new List<Vector3>();
        foreach (var item in roomGuildPoints)
        {
            result.Add(new Vector3(item.x, item.y, item.z));
        }

        return result;
    }
}

[Serializable]
public class RoomGuildPoint
{
    public RoomGuildPoint() { }

    public RoomGuildPoint(Vector3 point, Vector3 doorPosition)
    {
        this.x = point.x;
        this.y = point.y;
        this.z = point.z;
        this.doorPositionX = doorPosition.x;
        this.doorPositionY = doorPosition.y;
        this.doorPositionZ = doorPosition.z;
    }

    public float x;
    public float y;
    public float z;

    public float doorPositionX;
    public float doorPositionY;
    public float doorPositionZ;

    public Vector3 GetGuildPositoin()
    {
        return new Vector3(x, y, z);
    }

    public Vector3 GetDoorPosition()
    {
        return new Vector3(doorPositionX, doorPositionY, doorPositionZ);
    }
}
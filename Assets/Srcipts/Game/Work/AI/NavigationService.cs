using System.Collections.Generic;
using System.Linq;
using Loxodon.Framework.Contexts;
using LP.Framework;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public interface INavigationService : IService
{
    NavMeshPath GeneratePathOnly(Vector3 destination);
    public Vector3 GetRoomPointPosition(string roomName);

    public Vector3 GetRoomPointEulerAngle(string roomName);
}

public class NavigationService : INavigationService
{
    private NavMeshAgent _agent;
    private NavMeshPath _path;
    private Transform _player;
    private ApplicationContext _context;
    private IRoomService _roomService;

    private Dictionary<string, RoomService.RoomPointData> _roomPointDatas =
        new Dictionary<string, RoomService.RoomPointData>();


    public async void Init()
    {
        var _context = Context.GetApplicationContext();
        //var roomDataService = _context.GetService<IRoomService>();
        //var datas = roomDataService.GetAllRoomPointData();
        //FillRoomData(datas);

        var sceneGameObjectService = _context.GetService<ISceneGameObjectService>();
        this._roomService = _context.GetService<IRoomService>();

        //sceneGameObjectService.TryGetSceneGameObject("PlayerRoot", GetPlayerRootCallBack);
        GameObject playerRoot = await sceneGameObjectService.TryGetSceneGameObject("PlayerRoot");
        _player = playerRoot.transform.GetChild(0);
        _agent = playerRoot.transform.GetChild(1).GetComponent<NavMeshAgent>();
    }

    public NavMeshPath GeneratePathOnly(Vector3 destination)
    {
        List<Vector3> res = new List<Vector3>();
        if (_path == null)
        {
            _path = new NavMeshPath();
        }

        _agent.enabled = false;
        _agent.transform.position = _player.position;

        _agent.enabled = true;
        //只计算寻路路径 
        _agent.CalculatePath(destination, _path);
        res = _path.corners.ToList();
        return _path;
    }

    private void FillRoomData()
    {
        var roomDataService = _context.GetService<IRoomService>();
        var datas = roomDataService.GetAllRoomPointData();
        foreach (var room in datas)
        {
            _roomPointDatas.TryAdd(room.roomPointName, room);
        }
    }

    public Vector3 GetRoomPointPosition(string roomName)
    {
        if (_roomPointDatas.Count <= 0)
        {
            FillRoomData();
        }

        if (_roomPointDatas.TryGetValue(roomName, out RoomService.RoomPointData res))
        {
            return res.GetRoomPointPosition();
        }

        XLog.E($"不存在点{roomName}");
        return Vector3.zero;
    }

    public Vector3 GetRoomPointEulerAngle(string roomName)
    {
        if (_roomPointDatas.Count <= 0)
        {
            FillRoomData();
        }

        if (_roomPointDatas.TryGetValue(roomName, out RoomService.RoomPointData res))
        {
            return res.GetRoomPointEulerAngle();
        }

        XLog.E($"不存在点{roomName}");
        return Vector3.zero;
    }
}
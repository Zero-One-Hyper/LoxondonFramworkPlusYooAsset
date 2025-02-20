using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public interface IRoomService : IService
{
    List<RoomService.RoomPointData> GetAllRoomPointData();
    void EnterRoom(int roomIndex);
    void ExitRoom(int roomIndex);
    Room GetCurrentRoom();
    void GenerateElapseRoomList(int destinationRoomIndex);
    bool IsInRoamingPath(int roomIndex);
    bool IsInRoom(int currentGuildRoomIndex);
    bool IsInParentRoom(int currentGuildRoomIndex);
    List<int> GetGuildPathElapseRoom(); //返回需要经过的所有房间
}

[Serializable]
public class AllRoomConfigs
{
    public List<Room> allRoomConfigs;
}

[Serializable]
public class Room
{
    public int roomIndex;
    public int parentRoomIndex;
}

public class RoomService : IRoomService
{
    [Serializable]
    public class RoomPointData
    {
        public string roomPointName;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float eulerAngleX;
        public float eulerAngleY;
        public float eulerAngleZ;

        public void SetRoomPointData(Transform point)
        {
            this.positionX = point.position.x;
            this.positionY = point.position.y;
            this.positionZ = point.position.z;
            this.eulerAngleX = point.eulerAngles.x;
            this.eulerAngleY = point.eulerAngles.y;
            this.eulerAngleZ = point.eulerAngles.z;
        }

        public Vector3 GetRoomPointPosition()
        {
            return new Vector3(positionX, positionY, positionZ);
        }

        public Vector3 GetRoomPointEulerAngle()
        {
            return new Vector3(eulerAngleX, eulerAngleY, eulerAngleZ);
        }
    }

    [Serializable]
    public class AllRoomPointData
    {
        public List<RoomPointData> allPointData;
    }

    private ApplicationContext _context;
    private AllRoomPointData _allRoomPointData;

    private Room _currentRoom = null;
    private Dictionary<int, Room> _allRoom = new Dictionary<int, Room>();
    private List<int> _guildPathElapseRoom = new List<int>(); //固定漫游过程中可能途径的房间

    public async void Init()
    {
        _context = Context.GetApplicationContext();

        //加载门对应的Collider数据
        var assetLoad = _context.GetService<IAssetLoadUtil>();
        TextAsset textAsset = await assetLoad.ResourceLoadAsync<TextAsset>("RoomConfigs");
        AllRoomConfigs data = JsonUtility.FromJson<AllRoomConfigs>(textAsset.text);
        foreach (var room in data.allRoomConfigs)
        {
            _allRoom.Add(room.roomIndex, room);
        }
        /*
        assetLoad.LoadAssetAsync("RoomConfigs", handle =>
        {
            if (handle != null)
            {
                TextAsset textAsset = handle.AssetObject as TextAsset;
                AllRoomConfigs data = JsonUtility.FromJson<AllRoomConfigs>(textAsset.text);
                foreach (var room in data.allRoomConfigs)
                {
                    _allRoom.Add(room.roomIndex, room);
                }
            }
        });
        */
        LoadLightingData();
    }

    //加载存储的光照数据
    private async void LoadLightingData()
    {
        var resService = _context.GetService<IAssetLoadUtil>();
        TextAsset roomPositionData = await resService.ResourceLoadAsync<TextAsset>("RoomPositionConfig");

        _allRoomPointData = JsonUtility.FromJson<AllRoomPointData>(roomPositionData.text);
        EnterRoom(-1);
    }

    public List<RoomPointData> GetAllRoomPointData()
    {
        if (this._allRoomPointData == null)
        {
            Debug.Log(218931);
            return null;
        }

        return this._allRoomPointData.allPointData;
    }

    public Room GetCurrentRoom()
    {
        if (_currentRoom == null)
        {
            return null;
        }
        else
        {
            return _currentRoom;
        }
    }

    public void GenerateElapseRoomList(int destinationRoomIndex)
    {
        _guildPathElapseRoom.Clear();

        if (_allRoom.TryGetValue(destinationRoomIndex, out Room destinationRoom))
        {
            //组合当前房间的ID
            if (_currentRoom != null) //若现在在某个房间中
            {
                if (_currentRoom.roomIndex != destinationRoom.parentRoomIndex)
                {
                    //如果当前房间不是目标房间的父房间

                    _guildPathElapseRoom.Add(_currentRoom.roomIndex);
                }

                if (_currentRoom.parentRoomIndex != -2)
                {
                    //当前房间有父房间
                    _guildPathElapseRoom.Add(_currentRoom.parentRoomIndex);
                }
            }

            //组合目的地的房间ID
            if (destinationRoom.parentRoomIndex != -2) //判断目标房间是否存在父房间
            {
                if (_currentRoom != null)
                {
                    if (_currentRoom.roomIndex != destinationRoom.parentRoomIndex)
                    {
                        //当前房间不是目标房间的父房间
                        _guildPathElapseRoom.Add(destinationRoom.parentRoomIndex);
                    }
                }
                else //若当前没有在任何一个房间 
                {
                    //若目的地房间存在父房间
                    _guildPathElapseRoom.Add(destinationRoom.parentRoomIndex);
                }
            }

            //最后添加终点站
            _guildPathElapseRoom.Add(destinationRoom.roomIndex);
        }
    }

    public bool IsInRoamingPath(int roomIndex)
    {
        if (_guildPathElapseRoom.Count > 0)
        {
            //门的index在路径门中 ，且不是最后一个（终点） 或者 没有所属房间（-2）
            return (_guildPathElapseRoom.Contains(roomIndex) &&
                    _guildPathElapseRoom[_guildPathElapseRoom.Count - 1] != roomIndex) ||
                   roomIndex == -2 ||
                   (MeetingRoomDoor.IsInLinkedMeetingRoom && (roomIndex == 11 || roomIndex == 10) &&
                    (_guildPathElapseRoom.Contains(10) || _guildPathElapseRoom.Contains(11)));
        }

        return false;
    }

    public void EnterRoom(int roomIndex)
    {
        if (_allRoom.TryGetValue(roomIndex, out Room room))
        {
            _currentRoom = room;
            //Debug.Log($"进入房间{roomIndex}");
        }
    }

    public void ExitRoom(int roomIndex)
    {
        if (_currentRoom == null)
        {
            return;
        }

        if (_currentRoom.roomIndex == roomIndex)
        {
            //Debug.Log($"离开房间{roomIndex}");

            if (_currentRoom.parentRoomIndex != -2) //36是电梯间 -2是其他空白地方
            {
                EnterRoom(_currentRoom.parentRoomIndex);
            }
            else
            {
                //父房间index为-2时
                _currentRoom = null;
            }
        }
    }

    public bool IsInRoom(int currentGuildRoomIndex)
    {
        if (_currentRoom != null)
        {
            //有当前房间
            if (_currentRoom.roomIndex == currentGuildRoomIndex)
            {
                //在房间内
                return true;
            }

            //追加判断
            if (MeetingRoomDoor.IsInLinkedMeetingRoom)
            {
                //如果两个会议室链接起来了
                if (currentGuildRoomIndex == 10 || currentGuildRoomIndex == 11)
                {
                    //并位于连接状态的会议室中
                    return true;
                }
            }
        }

        //当前没有房间 返回null
        return false;
    }

    public bool IsInParentRoom(int currentGuildRoomIndex)
    {
        if (_currentRoom != null)
        {
            //有房间
            if (_currentRoom.parentRoomIndex != -2 && _currentRoom.parentRoomIndex == currentGuildRoomIndex)
            {
                //当前房间的父房间不是-2 在父房间中
                return true;
            }
        }

        return false;
    }

    public List<int> GetGuildPathElapseRoom()
    {
        return this._guildPathElapseRoom;
    }
}
using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;

public interface IPermissionsManagement : IService
{
    //权限检测
    void SendChecktUserPermission(int roomId, float time);

    //检验密码
    bool CheckPassword(string passwordText);
    void CloseUIDoorPermission();
    void RemoveCheckPermission(int roomID);
}

[Serializable]
public class AllPermissionData
{
    public List<PermissionData> datas;
}

[Serializable]
public class PermissionData
{
    public string roomIndex;
    public string roomName;
    public string allowVisit;
    public string passwordRequire;
    public string password; //密码
}

public class PermissionsManagement : IPermissionsManagement
{
    private IViewService _viewService;
    private IWebService _webService;

    private IInputService _inputService;

    //存储查询某个门的许可情况消息发送的时间
    private Dictionary<int, float> _checkDoorPermissionTime = new Dictionary<int, float>();
    private bool _isShowingPermissionUI = false;
    private string _password; //密码

    public void Init()
    {
        var context = Context.GetApplicationContext();
        _webService = context.GetService<IWebService>();
        _viewService = context.GetService<IViewService>();
        _inputService = context.GetService<IInputService>();
        var eventHandleService = context.GetService<IEventHandleService>();
        eventHandleService.AddListener(PermissionEvent.CheckRoomPermission, OnGetRoomPermissionData);
    }

    public void SendChecktUserPermission(int roomId, float time)
    {
        if (_isShowingPermissionUI)
        {
            //避免多次发送消息
            return;
        }

        if (_checkDoorPermissionTime.ContainsKey(roomId))
        {
            //已经发送过消息了 
            if (time - _checkDoorPermissionTime[roomId] < 4f)
            {
                //限制4s后再发送消息
                return;
            }

            _checkDoorPermissionTime[roomId] = time;
        }
        else
        {
            //Debug.Log($"添加请求{roomId}");
            _checkDoorPermissionTime.Add(roomId, time);
        }

        string data = roomId.ToString().Length < 2 ? "0" + roomId.ToString() : roomId.ToString();
        _webService.UnityCallWeb(Constant.UNITY_CALL_USER_PERMISSION, data);
        XLog.I($"发送验证进入房间权限请求:{data}");

#if UNITY_EDITOR
        currentRoomId = roomId;
        PermissionData permissionData = new PermissionData()
        {
            roomIndex = roomId.ToString(),
            roomName = "测试名称",
            allowVisit = "true", //roomId == "03" ? "false" : "true",
            passwordRequire = roomId == 22 ? "true" : "false",
            password = "114514",
        };
        AllPermissionData allPermissionData = new AllPermissionData()
        {
            datas = new List<PermissionData>() { permissionData },
        };

        string json = JsonUtility.ToJson(allPermissionData);
        //Debug.Log(json);
        PermissionEventDispath.DispatchSingleRoomPermission(json);
        UIEventDispatch.DispatchSwitchUIMask("false");
#endif
    }

    public bool CheckPassword(string passwordText)
    {
        return passwordText == this._password;
    }

    public void RemoveCheckPermission(int roomID)
    {
        //Debug.Log($"清除请求{roomID}");
        if (this._checkDoorPermissionTime.ContainsKey(roomID))
        {
            _checkDoorPermissionTime.Remove(roomID);
        }
    }

    private void OnGetRoomPermissionData(EventArgs args)
    {
        PermissionEventArgs data = args as PermissionEventArgs;
        if (data == null)
        {
            XLog.E("查询房间许可数据为空");
            return;
        }

        if (string.IsNullOrEmpty(data.Json))
        {
            XLog.E("房间许可字符串为空");
            return;
        }

        //解析数据
        AllPermissionData allPermissionDatas = JsonUtility.FromJson<AllPermissionData>(data.Json);
        if (allPermissionDatas == null)
        {
            XLog.E("解析房间许可信息数据错误");
            return;
        }

        if (allPermissionDatas.datas == null || allPermissionDatas.datas.Count <= 0)
        {
            XLog.E("房间许可信息为空");
            return;
        }

        XLog.I("接收到开门权限信息");
        PermissionData permissionData = allPermissionDatas.datas[0];
        this._password = permissionData.password;
        ShowOpenDoorUI(permissionData);
        int roomIndex = -2;
        if (!string.IsNullOrEmpty(permissionData.roomIndex))
        {
            roomIndex = Convert.ToInt32(permissionData.roomIndex);
        }

        RemoveCheckPermission(roomIndex);
    }

    private void ShowOpenDoorUI(PermissionData permissionData)
    {
        if (AutoRoaming.IsAutoRoaming && permissionData.passwordRequire == "false")
        {
            if (permissionData.allowVisit == "false")
            {
                //房间不允许访问              
                _viewService.OpenView<UIDoorPermission>(permissionData);
                _isShowingPermissionUI = true;
                return;
            }

            //自动漫游时 门不需要密码 直接继续走
            var sceneLogicService = Context.GetApplicationContext().GetService<ISceneLogicService>();
            sceneLogicService.ControlDoor(true);
            return;
        }

        if (!_isShowingPermissionUI)
        {
            //XLog.I("打开开门的UI");
            _viewService.OpenView<UIDoorPermission>(permissionData);
            _isShowingPermissionUI = true;
        }
    }

    public void CloseUIDoorPermission()
    {
        if (!AutoRoaming.IsAutoRoaming)
        {
            if (_inputService == null)
            {
                _inputService = Context.GetApplicationContext().GetService<IInputService>();
            }

            _inputService.EnableMoveInput();
        }

        if (_isShowingPermissionUI)
        {
            _isShowingPermissionUI = false;
            _viewService.CloseView<UIDoorPermission>();
            //关闭UI 清空所有等待检查权限的门
            _checkDoorPermissionTime.Clear();
        }
    }

    private static int currentRoomId;
#if UNITY_EDITOR
    public static void SendDoorPermissionCheckData()
    {
        PermissionData permissionData = new PermissionData()
        {
            roomIndex = currentRoomId.ToString(),
            roomName = "测试名称",
            allowVisit = "true", //roomId == "03" ? "false" : "true",
            passwordRequire = "false",
            password = "114514",
        };
        AllPermissionData allPermissionData = new AllPermissionData()
        {
            datas = new List<PermissionData>() { permissionData },
        };

        string json = JsonUtility.ToJson(allPermissionData);
        Debug.Log($"接收{json}");
        PermissionEventDispath.DispatchSingleRoomPermission(json);
    }
#endif
}
using UnityEngine;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using System.Text;
using System.Collections.Generic;

public interface ISceneLogicService : IService
{
    void BeginAutoRoaming(RoomGuildPoint roomGuildPoint);
    void StopAutoRoaming();
    void ControlDoor(bool openDoor);
    Vector3 GetPlayerPosition();
    float GetPlayerForward();
    bool InitAds();
    void TryContimueAutoAroming();
    void TryPauseAutoAroming();
    Vector3 GetAutoRoamingDirection();
    void OnRoomNotAllowVisit();
    void OnCancleOpenDoor();
    List<Door> GetDoorByRoomID(int roomID);
}

public class SceneLogic : ISceneLogicService
{
    private ApplicationContext _context;
    private ICameraService _cameraController;
    private ISceneGameObjectService _sceneGameObjectService;
    private Roaming _roamingLogic;

    private IDoorController _doorController;
    private IInputService _inputService;

    private IProvinceService _provinceController;
    private IReceptionGuildManager _receptionGuildManager;
    private IAddService _addService;
    private IWebService _webService;
    private IViewService _viewService;

    public async void Init()
    {
        _context = Context.GetApplicationContext();
        this._webService = _context.GetService<IWebService>();
        this._viewService = _context.GetService<IViewService>();

        GameObject playerRoot = await _context.GetService<ISceneGameObjectService>().TryGetSceneGameObject("PlayerRoot");
        _roamingLogic = playerRoot.AddComponent<Roaming>();
        _roamingLogic.Init();

        _cameraController = _context.GetService<ICameraService>();

        _inputService = _context.GetService<IInputService>();
        _inputService.RegisterShortCutKeyStopAutoRoaming(() =>
        {
            if (AutoRoaming.IsAutoRoaming)
            {
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

                _viewService.OpenView<UITipLeftDown>();
                StopAutoRoaming();
            }

            _inputService.EnableMoveInput();
        });

        var service = Context.GetApplicationContext().GetService<IEventHandleService>();
        service.AddListener(SceneEvent.SceneType, OnWebCallSwitchScene);

        _doorController = new DoorController();
        _doorController.Init();

        //电梯间导览台
        _receptionGuildManager = new ReceptionGuildManager();
        _receptionGuildManager.Init();

        _sceneGameObjectService = _context.GetService<ISceneGameObjectService>();
        SwitchToRoamingMode();
    }


    //在开门后初始化广告牌
    public bool InitAds()
    {
        if (this._addService == null)
        {
            _addService = new AdsManager();
            _addService.Init();
        }

        return true;
    }

    private void OnWebCallSwitchScene(EventArgs obj)
    {
        SceneEventArgs args = obj as SceneEventArgs;
        if (args == null)
        {
            XLog.E("场景切换消息为空");
            return;
        }

        string data = args.Data;

        if (string.IsNullOrEmpty(data))
        {
            XLog.E("场景切换消息为空");
            return;
        }

        XLog.I($"接收到切换场景的消息{data}");
        switch (data)
        {
            case "Roaming":
                SwitchToRoamingMode();
                break;
            case "Panorama":
                SwitchToPanoramaMode();
                break;
        }
    }

    //切换漫游模式
    private void SwitchToRoamingMode()
    {
        //打开小地图
        var viewManager = Context.GetApplicationContext().GetService<IViewService>();
        viewManager.EnterRoamingModeSetUI();
        viewManager.OpenView<UITipLeftDown>();

        _inputService.ChangeInputType(GameScene.Roaming);
        if (_cameraController != null)
        {
            _cameraController.SwitchCamera(GameScene.Roaming);
        }

        if (_roamingLogic != null)
        {
            _roamingLogic.EnterRoamingMode();
        }

        if (_sceneGameObjectService != null)
        {
            //清除省图模型
            _sceneGameObjectService.UnLoadProvincialMap();
        }
    }

    //切换省图模式
    private void SwitchToPanoramaMode()
    {
        //关闭无关UI
        var viewManager = Context.GetApplicationContext().GetService<IViewService>();
        viewManager.EnterPanoramaModeSetUI();

        //生成省图模型
        _sceneGameObjectService.LoadProvincialMap();
        LoadProvincialInteractive();

        _inputService.ChangeInputType(GameScene.Panorama);
        _cameraController.SwitchCamera(GameScene.Panorama);
        _roamingLogic.ExitRoamingMode();
    }

    public void ControlDoor(bool openDoor)
    {
        _doorController.OpenDoor(openDoor);

        //if (openDoor)
        //{
        //    //Debug.Log("考虑放在Door的开门事件中");
        //    ContinuePathfinding();
        //}
    }

    public Vector3 GetPlayerPosition()
    {
        if (_roamingLogic == null || _roamingLogic.transform.GetChild(0) == null)
        {
            return Vector3.zero;
        }
        return _roamingLogic.transform.GetChild(0).position;
    }

    public float GetPlayerForward()
    {
        if (_roamingLogic == null || _roamingLogic.transform.GetChild(0) == null)
        {
            return 0;
        }
        return _roamingLogic.transform.GetChild(0).eulerAngles.y;
    }

    public void OnCancleOpenDoor()
    {
        _doorController.ClearCheckDoorPermission();
    }

    private async void LoadProvincialInteractive()
    {
        GameObject provinceScene = await _sceneGameObjectService.TryGetSceneGameObject("ProvinceScene");
        if (this._provinceController == null)
        {
            _provinceController = new ProvinceCreator();
        }

        _provinceController.Init(provinceScene);
    }


    public void TryContimueAutoAroming()
    {
        _roamingLogic.TryContimueAutoAroming();
    }

    public void TryPauseAutoAroming()
    {
        _roamingLogic.TryPauseAutoAroming();
    }

    public void BeginAutoRoaming(RoomGuildPoint roomGuildPoint)
    {
        _roamingLogic.SetAutoRoamingTarget(roomGuildPoint.GetGuildPositoin(), roomGuildPoint.GetDoorPosition());
    }

    public void StopAutoRoaming()
    {
        _roamingLogic.StopAutoRoaming();
    }

    public Vector3 GetAutoRoamingDirection()
    {
        return _roamingLogic.GetAutoRoamingDirection();
    }

    public void OnRoomNotAllowVisit()
    {
        _roamingLogic.OnRoomNotAllowVisit();
    }

    public List<Door> GetDoorByRoomID(int roomID)
    {
        return _doorController.GetDoorByRoomID(roomID);
    }
}
using System;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnitData.Const;
using UnityEngine;

//导航台点击
public interface IReceptionGuildManager
{
    void Init();
}

public class ReceptionGuildManager : IReceptionGuildManager
{
    private IGuildTourService _guildTourService;
    private ReceptionGuild _receptionGuildDesk;
    private Camera _mainCamera;

    public async void Init()
    {
        var sceneGameObjectService = Context.GetApplicationContext().GetService<ISceneGameObjectService>();
        var receptionGuildGameObject = await sceneGameObjectService.TryGetSceneGameObject("ReceptionGuild");
        _receptionGuildDesk = receptionGuildGameObject.AddComponent<ReceptionGuild>();
        _receptionGuildDesk.Init(this);

        _guildTourService = Context.GetApplicationContext().GetService<IGuildTourService>();
        _mainCamera = Camera.main;
    }

    public void OnClickReceptionGuild()
    {
        //点击导览台弹窗
        IWebService webService = Context.GetApplicationContext().GetService<IWebService>();
        string id = "53";
        XLog.I($"发送点击导览台广告位{id}事件");
        webService.UnityCallWeb(Constant.UNITY_CALL_CLICK_ADS, id);
    }
}
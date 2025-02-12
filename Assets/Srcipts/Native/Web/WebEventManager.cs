/// <summary>
/// Web 注册事件管理器
/// </summary>
public class WebEventManager : IWebEvent
{
    public void UnitySceneChange(string name)
    {
        SceneEventDispath.DispatchScene(name);
    }

    public void UnitySwitchingFrame(string name)
    {
    }

    public void UnitySwitchingTrainModel(string name)
    {
    }

    public void UnitySetAutoRounding(string name)
    {
    }

    public void OnCallSwitchScene(string obj)
    {
        SceneEventDispath.DispatchSwitchScene(obj);
    }

    public void OnWebCallSwitchUIMask(string obj)
    {
        UIEventDispatch.DispatchSwitchUIMask(obj);
    }

    public void OnWebSendEmployeeInfomation(string obj)
    {
        EmployeeDataDispath.DispatchSingleEmployeeInfomation(obj);
    }
    //web发送单个房间许可
    public void OnWebSendRoomPermission(string obj)
    {
        PermissionEventDispath.DispatchSingleRoomPermission(obj);
    }
    //web发送点击自动漫游
    public void OnWebCallAutoRounding(string obj)
    {
        SceneEventDispath.DispatchAutoRounding(obj);
    }

    //web发送开关原点IP
    public void OnWebCallSwitchIP(string obj)
    {
        SceneEventDispath.DispatchSwitchIP(obj);
    }
    //web发送所有房间名数据
    public void OnWebSendAllDoorplateData(string obj)
    {
        SceneEventDispath.DispatchAllDoorplateData(obj);
    }
    //web发送单个房间名数据
    public void OnWebSendSingleDoorplateData(string obj)
    {
        SceneEventDispath.DispatchSingleDoorplateData(obj);
    }
}
/// <summary>
/// Web 注册事件
/// </summary>
public interface IWebEvent
{
    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="name"></param>
    void UnitySceneChange(string name);

    /// <summary>
    /// 切换线框
    /// </summary>
    /// <param name="name"></param>
    void UnitySwitchingFrame(string name);

    /// <summary>
    /// 切换列车模型
    /// </summary>
    /// <param name="name"></param>
    void UnitySwitchingTrainModel(string name);

    /// <summary>
    /// 自动旋转
    /// </summary>
    /// <param name="name"></param>
    void UnitySetAutoRounding(string name);

    /// <summary>
    /// web切换unity场景方法
    /// </summary>
    /// <param name="obj"></param>
    void OnCallSwitchScene(string obj);

    void OnWebCallSwitchUIMask(string obj);
    void OnWebSendEmployeeInfomation(string obj);
    void OnWebSendRoomPermission(string obj);
    void OnWebCallAutoRounding(string obj);
    void OnWebCallSwitchIP(string obj);
    void OnWebSendSingleDoorplateData(string obj);
    void OnWebSendAllDoorplateData(string obj);
}
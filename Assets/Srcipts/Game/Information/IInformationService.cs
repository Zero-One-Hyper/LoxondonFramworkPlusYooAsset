/// <summary>
/// 数据信息服务接口
/// </summary>
public interface IInformationService
{
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="name"></param>
    void GetInformation(string name);

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="name"></param>
    void SetInformation(string name);
}

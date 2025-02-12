/// <summary>
/// 场景加载服务接口
/// </summary>
public interface ISceneService
{
    string SceneName { get; set; }
    void LoadScene(string scene);
}

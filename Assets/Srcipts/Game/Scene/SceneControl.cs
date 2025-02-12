using UnityEngine.SceneManagement;

/// <summary>
/// Scene控制器
/// </summary>
public class SceneControl : ISceneService
{
    public string SceneName { get; set; }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
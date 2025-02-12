using System.Threading.Tasks;

namespace LP.Framework
{
    /// <summary>
    /// 启动加载资产接口
    /// </summary>
    public interface IRunLoad : ILoad
    {
        Task Load();
    }
}

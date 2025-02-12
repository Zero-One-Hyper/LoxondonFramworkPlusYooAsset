using System.Collections.Generic;

namespace LP.Framework
{
    /// <summary>
    /// 应用启动接口
    /// </summary>
    public interface IAppRun
    {
        List<IRunLoad> AppLoadList { get; set; }
        void StartUp();

        void Register(IRunLoad load);
        void Stop();
    }
}


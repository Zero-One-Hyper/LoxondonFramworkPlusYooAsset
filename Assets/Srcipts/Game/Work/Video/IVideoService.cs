using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 视频接口服务
    /// </summary>
    public interface IVideoService : IService
    {
        /// <summary>
        /// 播放
        /// </summary>
        void Play();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
    }
}

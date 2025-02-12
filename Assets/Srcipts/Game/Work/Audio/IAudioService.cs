using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 音频服务接口
    /// </summary>
    public interface IAudioService : IService
    {
        /// <summary>
        /// 播放
        /// </summary>
        void Play();
        void Play(AudioClip clip);
    
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="source"></param>
        void Reset(AudioSource source);
    }
}


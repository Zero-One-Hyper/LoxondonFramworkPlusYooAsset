using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// Icon 服务接口
    /// </summary>
    public interface IIconService : IService
    {

        void Init();
    
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="_CameraDis"></param>
        /// <param name="_CameraY"></param>
        void ScaleingAndFading(float _CameraDis, float _CameraY);
    }
}

using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 动画服务接口
    /// </summary>
    public interface IAnimationService:IService
    {
        Animator Animator { get; set; }

        /// <summary>
        /// 开始扫描
        /// </summary>
        void StartScanning();

        /// <summary>
        /// 动画结束回调
        /// </summary>
        void AnimaEndCallBack();

        /// <summary>
        /// 动画结束调用
        /// </summary>
        void AnimaEnd();

        /// <summary>
        /// 点击碰撞体
        /// </summary>
        /// <param name="name"></param>
        void OnClkBox(string name);

        /// <summary>
        /// 自动环绕旋转
        /// </summary>
        void AutoRounding();

        /// <summary>
        /// 设置透明
        /// </summary>
        void SetFraming();

    }
}


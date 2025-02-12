using Loxodon.Framework.Contexts;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// 动画控制器
    /// </summary>
    public class AnimationControl: IAnimationService
    {
        public Animator Animator { get; set; }
    
        public void Init()
        {
            var update = Context.GetApplicationContext().GetService<IBuiltInFuncService>();
            update.OnUpdateEvent+=Update;
        }
    
        public void StartScanning()
        {
        }

        public void AnimaEndCallBack()
        {
        }

        public void AnimaEnd()
        {
        }

        public void OnClkBox(string name)
        {
        }

        public void AutoRounding()
        {
        }

        public void SetFraming()
        {
        }

        void Update()
        {
        }
    }
}


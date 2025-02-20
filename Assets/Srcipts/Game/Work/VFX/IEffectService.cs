namespace LP.Framework
{
    /// <summary>
    /// 特效服务接口
    /// </summary>
    public interface IEffectService : IService
    {
        void ShowGuildLine(UnityEngine.AI.NavMeshPath path);

        void HideGuildLine();

        /// <summary>
        /// 设置提示
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="scale"></param>
        //void SetNoiseCube(Vector3 pos, Vector3 scale);

        /// <summary>
        /// 设置模型
        /// </summary>
        /// <param name="isOn"></param>
        //void SetModelShow(bool isOn);

        /// <summary>
        /// 设置Name
        /// </summary>
        /// <param name="value"></param>
        //void SetNameText(string value);
    }
}
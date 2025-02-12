using UnityEngine;

public class CameraEventArgs : EventArgs
{
    /// <summary>
    /// V3值
    /// </summary>
    public Vector3 V3 { get; set; }

    public float Angle { get; set; }

    /// <summary>
    /// 相机事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sender"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public CameraEventArgs Register(CameraEvent type, GameObject sender, Vector3 v3)
    {
        base.Register(type,sender);
        this.V3 = v3;
        return this;
    }
}

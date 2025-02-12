using System;
using LP.Framework;
using UnityEngine;

public class UIMap : ViewBase
{
    //接收每帧player相对地图位置的回调
    protected Func<Vector3> _playerMoveCallBack;
    protected Func<float> _playerForwardCallBack;

    public override void InitUI(params object[] args)
    {
        base.InitUI();
        //处理参数
        if (args != null && args.Length > 1)
        {
            this._playerMoveCallBack = args[0] as Func<Vector3>;
            this._playerForwardCallBack = args[1] as Func<float>;
        }
    }

    protected virtual void SetMinmapPosition(Vector3 palyerRelativePosition)
    {

    }

    protected virtual void SetMinMapRotation(float playerAngleY)
    {

    }
}

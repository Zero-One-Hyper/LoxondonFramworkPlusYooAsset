using System;
using UnityEngine;

public class WalkThroughCamera : BaseCamera
{
    private Transform _followTarget;

    public void Init(Camera mainCamera)
    {
        base.Init(mainCamera, this.transform.GetChild(0));
    }

    public override void SetFollowTarget(Transform followTarget)
    {
        this._followTarget = followTarget;
    }

    private void Update()
    {
        //漫游相机跟随场景角色移动
        if (this.CameraFollowTransform != null && _followTarget != null)
        {
            this.CameraFollowTransform.SetPositionAndRotation(_followTarget.position, _followTarget.rotation);
        }
    }
}
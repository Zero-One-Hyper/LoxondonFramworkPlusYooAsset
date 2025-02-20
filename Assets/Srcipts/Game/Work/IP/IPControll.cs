#if UNITY_EDITOR
using System;
using DG.Tweening;
using UnityEngine;

//此类旨只在Eidtor模式下使用
public class IPControll : MonoBehaviour
{
    public AnimationCurve Curve;
    public Ease Ease;
    private Camera _mainCamera;
    public void Init(Camera mainCamera)
    {
        this._mainCamera = mainCamera;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 ip2Camera = _mainCamera.transform.position - this.transform.position;
        //Vector3 ipForward = Vector3.ProjectOnPlane(ip2Camera, Vector3.up);
        //this.transform.forward = ipForward.normalized;
    }
}
#endif
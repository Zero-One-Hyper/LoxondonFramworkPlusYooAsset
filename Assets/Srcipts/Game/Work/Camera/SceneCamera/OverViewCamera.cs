using System;
using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class OverViewCamera : BaseCamera
{
    private bool _isFouceing = false;
    private Vector3 _defaultPosition = Vector3.zero;
    public void Init(Camera mainCamera)
    {
        MoveSpeed = 1f;
        MoveLimit = new Vector4(1f, -1f, 0.5f, -0.5f); //x最大 x最小 z最大 z最小
        RotateSpeed = new Vector2(30f, 20f); //水平 竖直
        RotateVerticalLimit = new Vector2(-25f, 0f); //仰角 俯角
        ScaleSpeed = 1;
        _cinemachineFollow = _cinemachineCamera.transform.GetComponent<CinemachineFollow>();

        _defaultFollowOffset = new Vector3(0f, 1.58f, 0f);
        base.Init(mainCamera, this.transform.GetChild(0));
    }

    public void SetFocus(Transform target, Action callBack)
    {
        LookAt(target.transform.position, callBack);
    }

    public void UnFocus(Action callBack)
    {
        StopAllCoroutines();
        if (_isFouceing)
        {
            CameraFollowTransform.DOMove(_defaultPosition, 0.5f);
            StartCoroutine(DoScale(_defaultFollowOffset, 0.35f, () =>
           {
               this._isFouceing = false;
               callBack?.Invoke();
           }));
        }
        else
        {
            CameraFollowTransform.DOMove(_defaultPosition, 0.5f).OnComplete(() =>
            {
                callBack?.Invoke();
            });
        }
    }
    private void LookAt(Vector3 position, Action callBack)
    {
        StopAllCoroutines();
        if (!_isFouceing)
        {
            CameraFollowTransform.DOMove(position, 0.5f);
            //CameraFollowTransform.DORotate(new Vector3(-20, 0, 0), 0.5f);
            StartCoroutine(DoScale(_defaultFollowOffset + (Vector3.up * -0.79f), 0.35f, () =>
            {
                this._isFouceing = true;
                callBack?.Invoke();
            }));
        }
        else
        {
            //CameraFollowTransform.DORotate(new Vector3(-20, 0, 0), 0.5f);
            CameraFollowTransform.DOMove(position, 0.5f).OnComplete(() =>
            {
                callBack?.Invoke();
            });
        }

    }

    public override void Move(float deltaTime, Vector2 moveDir)
    {
        return;
    }

    public override void Rotate(float deltaTime, Vector2 input)
    {
        return;
    }

    public override void Scale()
    {

    }

    private bool LimitMovement(Vector3 destination)
    {
        if (destination.x < MoveLimit.x && destination.x > MoveLimit.y &&
            destination.z < MoveLimit.z && destination.z > MoveLimit.w)
        {
            return true;
        }

        return false;
    }
}
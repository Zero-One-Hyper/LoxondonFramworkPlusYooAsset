using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseCamera : MonoBehaviour
{
    protected Transform CameraFollowTransform;
    protected Camera MainCamera;

    //数值在子类中填充
    protected bool CanMove;
    protected float MoveSpeed;
    protected Vector4 MoveLimit;
    protected Vector2 RotateSpeed;
    protected Vector2 RotateVerticalLimit;
    protected float ScaleSpeed;
    protected Vector2 ScaleLimit;

    protected Vector3 MoveDirection
    {
        get
        {
            var dir = Vector3.ProjectOnPlane(MainCamera.transform.forward, Vector3.up).normalized;
            if (dir.magnitude <= 0.9f)
            {
                return Vector3.forward;
            }
            return dir;
        }
    }

    protected Vector3 MoveDestination;
    protected Vector3 CurrentPosition;

    protected Vector3 _defaultFollowOffset;
    protected float VerticalRotate;

    protected CinemachineCamera _cinemachineCamera;
    protected CinemachineFollow _cinemachineFollow;

    public virtual void Init(Camera mainCamera, Transform followTransform)
    {
        this.MainCamera = mainCamera;
        this.CameraFollowTransform = followTransform;
    }

    public void EnableCamera()
    {
        this.gameObject.SetActive(true);
        _cinemachineCamera.Priority = 100;
        VerticalRotate = CameraFollowTransform.eulerAngles.x;
    }

    public void DisableCamera()
    {
        _cinemachineCamera.Priority = -1;
        this.gameObject.SetActive(false);
    }

    private void Awake()
    {
        CameraFollowTransform = this.transform.GetChild(0);
        _cinemachineCamera = CameraFollowTransform.GetChild(0).GetComponent<CinemachineCamera>();
    }
    protected IEnumerator DoScale(Vector3 targetFollowOffset, float time, Action callBack = null)
    {
        Vector3 currentFollowOffset = _cinemachineFollow.FollowOffset;
        for (int i = 0; i < 100; i++)
        {
            Vector3 temp = Vector3.Lerp(currentFollowOffset, targetFollowOffset, i * 0.01f);
            _cinemachineFollow.FollowOffset = temp;
            yield return new WaitForSecondsRealtime(time / 100f);
        }
        callBack?.Invoke();
    }

    public virtual void Move(float deltaTime, Vector2 moveDir)
    {
        this.CurrentPosition = CameraFollowTransform.position;
    }

    public virtual void Rotate(float deltaTime, Vector2 input)
    {
    }

    public virtual void Scale()
    {
    }

    public virtual void SetFollowTarget(Transform followTransform)
    {
    }
}
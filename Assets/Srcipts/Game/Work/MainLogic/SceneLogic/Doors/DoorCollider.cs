using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[DisallowMultipleComponent]
public class DoorCollider : MonoBehaviour, IInteractCollider
{
    private List<IInteractive> _doors = new List<IInteractive>();
    [SerializeField]
    private List<Door> door = new List<Door>();
    private BoxCollider _boxCollider;
    //collider三角形斜边
    private float _hypotenuseLength;

    public IInteractive interactor { get; set; }

    private void Start()
    {
        this._boxCollider = this.GetComponent<BoxCollider>();
        //计算斜边长度
        this._hypotenuseLength = Mathf.Sqrt(Mathf.Pow(_boxCollider.size.x, 2) +
            Mathf.Pow(_boxCollider.size.z, 2)) * 0.5f;
    }

    public void ColliderInteractive(InteractiveColliderType interactiveColliderType)
    {
        //用于外部调用
    }

    public void SetOwner(IInteractive owner)
    {
        this._doors.Add(owner);
        door.Add(owner as Door);
    }

    private void OnTriggerEnter(Collider other)
    {
        //拿到角色当前的移动方向
        //Debug.Log(other.gameObject.name);
        TriggerEnter(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        //在Collider中时判断
        TriggerStay(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExit();
    }

    private void TriggerEnter(GameObject go)
    {
        foreach (var door in _doors)
        {
            InterActiveArgs args = new InterActiveArgs()
            { doorTriggerType = DoorTriggerType.Enter, gameObject = go, floatValue = _hypotenuseLength };
            door.Interactive(args);
        }
        //Debug.Log(go.name);
    }

    private void TriggerStay(GameObject go)
    {
        foreach (var door in _doors)
        {
            InterActiveArgs args = new InterActiveArgs()
            {
                doorTriggerType = DoorTriggerType.Stay,
                gameObject = go
            };
            door.Interactive(args);
        }
    }

    private void TriggerExit()
    {
        foreach (var door in _doors)
        {
            door.Interactive(new InterActiveArgs()
            {
                doorTriggerType = DoorTriggerType.Exit,
                vector3 = this.transform.position
            });
        }
    }

}
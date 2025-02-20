using System;
using Loxodon.Framework.Contexts;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomCollider : MonoBehaviour
{
    //private int _roomID;
    //private IRoomService _roomService;
    //private void Init()
    //{
    //    _roomService = Context.GetApplicationContext().GetService<IRoomService>();
    //}
    //
    //private void TriggerEnter(GameObject go)
    //{
    //    TriggerEnter();
    //}
    //private void OnTriggerStay(Collider other)
    //{
    //    //在Collider中时判断
    //    TriggerStay();
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    TriggerExit();
    //}
    //
    //private void TriggerEnter()
    //{
    //    XLog.I($"进入房间{_roomID}");
    //    _roomService.EnterRoom(_roomID);
    //}
    //
    //private void TriggerStay()
    //{
    //    //XLog.I($"呆在房间{_roomID}");
    //}
    //
    //private void TriggerExit()
    //{
    //    XLog.I($"离开房间{_roomID}");
    //    _roomService.ExitRoom(_roomID);
    //}
}

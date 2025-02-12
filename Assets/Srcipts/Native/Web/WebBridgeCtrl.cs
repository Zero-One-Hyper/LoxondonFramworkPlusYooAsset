using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnitData.Const;
using UnityEngine;

namespace LP.Framework
{
    /// <summary>
    /// Web交互接口控制器，此需为场景物体挂载
    /// </summary>
    public class WebBridgeCtrl : MonoBehaviour, IWebService
    {
        public string ID { get; set; }
        public Dictionary<string, List<Action<string>>> ListenEventList { get; set; }
        public Dictionary<string, TaskCompletionSource<string>> CallBacks { get; set; }

        [DllImport("__Internal")]
        private static extern void EmitWeb(string id, string type, string json);

        private IWebEvent _webEventMgr;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void Init()
        {
            XLog.I("WebBridgeCtrl Init");
            _webEventMgr = new WebEventManager();
            ListenEventList = new Dictionary<string, List<Action<string>>>();
            CallBacks = new Dictionary<string, TaskCompletionSource<string>>();
            RegisterEventUtil();
        }

        /// <summary>
        /// Unity调用Web
        /// </summary>
        /// <param name="type"></param>
        /// <param name="json"></param>
        public void UnityCallWeb(string type, string json)
        {
            ID = Guid.NewGuid().ToString("N") + Time.frameCount;
#if !UNITY_EDITOR
        DistributeEmitWeb(ID,type,json);
#endif
        }

        public System.Collections.IEnumerator UnityCallWebDelay(float time, string type, string json)
        {
            yield return new WaitForSeconds(time);
            UnityCallWeb(type, json);
        }

        /// <summary>
        /// Web调用Unity
        /// </summary>
        /// <param name="events"></param>
        public void EmitUnity(string events)
        {
            try
            {
                Regex regex = new Regex("(.+)__(.+)__(.*)");
                Match match = regex.Match(events);
                if (!match.Success)
                    return;

                string id = match.Groups[1].Value;
                string type = match.Groups[2].Value;
                string json = match.Groups[3].Value;

                XLog.I($"EmitUnity id:{id} type:{type} json:{json}");

                if (CallBacks.TryGetValue(id, out var task))
                {
                    task.SetResult(json);
                    CallBacks.Remove(id);
                    return;
                }

                if (ListenEventList.TryGetValue(type, out var list))
                {
                    list.ToList().ForEach(func => func(json));
                }
            }
            catch (Exception e)
            {
                XLog.I("EmitUnity: " + e.Message);
            }
        }

        public void RegisterEvent(string type, Action<string> action)
        {
            ListenEventList.TryGetValue(type, out var list);
            if (list == null)
            {
                list = new List<Action<string>>();
            }

            list.Add(action);
            ListenEventList.Add(type, list);
        }

        public void RemoveEvent(string type, Action<string> action)
        {
            if (ListenEventList.TryGetValue(type, out var list))
            {
                list.Remove(action);
            }
        }

        public Task<string> CallWebWithBack(string type, string json)
        {
            ID = Guid.NewGuid().ToString("N") + Time.frameCount;
            var task = new TaskCompletionSource<string>();
            CallBacks.Add(ID, task);
            DistributeEmitWeb(ID, type, json);
            return task.Task;
        }

        /// <summary>
        /// 辅助注册事件
        /// </summary>
        void RegisterEventUtil()
        {
            RegisterEvent("UnitySceneChange", _webEventMgr.UnitySceneChange);
            RegisterEvent("UnitySwitchingFrame", _webEventMgr.UnitySwitchingFrame);
            RegisterEvent("UnitySwitchingTrainModel", _webEventMgr.UnitySwitchingTrainModel);
            RegisterEvent("UnitySetAutoRounding", _webEventMgr.UnitySetAutoRounding);

            //切换场景
            RegisterEvent(Constant.WEB_CALL_SWITCH_SCENE, _webEventMgr.OnCallSwitchScene);
            //切换UIMask
            RegisterEvent(Constant.WEB_CALL_SWITCH_UI_MASK, _webEventMgr.OnWebCallSwitchUIMask);
            //web发送单个工位信息
            RegisterEvent(Constant.WEB_SEND_EMPLOYEE_INFOMATION, _webEventMgr.OnWebSendEmployeeInfomation);
            //web发送房间许可信息
            RegisterEvent(Constant.WEB_SEND_ROOM_PERMISSION, _webEventMgr.OnWebSendRoomPermission);
            //web发送点击自动漫游
            RegisterEvent(Constant.WEB_CALL_AUTO_ROUNDING, _webEventMgr.OnWebCallAutoRounding);
            //web发送开关原点IP
            RegisterEvent(Constant.WEB_CALL_SWITCH_IP, _webEventMgr.OnWebCallSwitchIP);
            //web发送所有房间名数据
            RegisterEvent(Constant.WEB_SEND_ALL_ROOM_NAME, _webEventMgr.OnWebSendAllDoorplateData);
            //web发送单个房间名
            RegisterEvent(Constant.WEB_SEND_SINGLE_ROOM_NAME, _webEventMgr.OnWebSendSingleDoorplateData);
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="json"></param>
        void DistributeEmitWeb(string id, string type, string json)
        {
            try
            {
                EmitWeb(id, type, json);
            }
            catch (Exception e)
            {
                XLog.I("DistributeEmitWeb" + e.Message);
            }
        }
    }
}
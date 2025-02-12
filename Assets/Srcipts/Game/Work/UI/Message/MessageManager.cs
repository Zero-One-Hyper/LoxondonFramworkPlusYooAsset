using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using UnityEngine;

namespace LP.Framework
{
    public class MessageManager : IMessageService
    {
        public void Init()
        {
            
        }

        public void SendMessage(XMessage msg)
        {
            var _viewList = Context.GetApplicationContext().GetService<IViewBase>().GetViewsList();
            for (int i = 0; i < _viewList.Count; i++)
            {
                var v = _viewList[i];
                if (v != null)
                {
                    v.ReceiveMessage(msg);
                }
                else
                {
                    Debug.LogError("没有找到这个View");
                }
            }
        }

        public void SendMessage(byte type, int cmd, object content)
        {
            var msg = new XMessage(type, cmd, content);
            SendMessage(msg);
        }
    }
}


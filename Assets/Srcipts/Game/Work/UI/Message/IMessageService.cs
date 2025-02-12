using System.Collections;
using System.Collections.Generic;
using LP.Framework;
using UnityEngine;

namespace LP.Framework
{
    public interface IMessageService : IService
    {
        void SendMessage(XMessage msg);

        void SendMessage(byte type, int cmd, object content);
    }
}


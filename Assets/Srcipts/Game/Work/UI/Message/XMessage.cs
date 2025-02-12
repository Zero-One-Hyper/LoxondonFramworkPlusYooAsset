using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework
{
    public class XMessage
    {
        public byte Type;
        public int Id;
        public object Content;

        public XMessage() { }

        public XMessage(byte type, int id, object content)
        {
            Type = type;
            Id = id;
            Content = content;
        }
    }

    public class XMessageType
    {
        public static byte Type_UI = 2;
    }

    public class XMessageCmd
    {
        public static int UI_Refresh = 10001;
    }
}


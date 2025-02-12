using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHttpHandle
{
    void RegisterMsg(Dictionary<EOPERATION, Action<TokenMsg>> handlers);
}

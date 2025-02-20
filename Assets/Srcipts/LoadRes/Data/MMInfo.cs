using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MMInfo
{
    public MMType type;
    public string key;
    public List<string> values;
}

[Serializable]
public enum MMType
{
    Mat,Mesh
}

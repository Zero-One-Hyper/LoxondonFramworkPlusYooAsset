using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EmployeeDataArgs : EventArgs
{
    /// <summary>
    /// web传入数据
    /// </summary>
    public string Json { get; set; }

    /// <summary>
    /// 列车相关事件注册
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sender"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public EmployeeDataArgs Register(EmployeeDataEvent type, string json)
    {
        base.Register(type);
        this.Json = json;
        return this;
    }
}
public enum EmployeeDataEvent
{
    SingleEmployeeInfomation,
}
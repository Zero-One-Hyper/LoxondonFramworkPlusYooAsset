using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;

public static class EmployeeDataDispath
{
    public static void DispatchSingleEmployeeInfomation(string json)
    {
        Context.GetApplicationContext().GetService<IEventHandleService>().Allocate<EmployeeDataArgs>()
            .Register(EmployeeDataEvent.SingleEmployeeInfomation, json)
            .Invoke();
    }
}

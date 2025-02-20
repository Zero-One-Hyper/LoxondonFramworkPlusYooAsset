using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using UnityEngine;

public class Test : MonoBehaviour
{
#if UNITY_EDITOR
    private string _testDoorplateData =
        "{\n    \"datas\": [\n        {\n            \"roomIndex\": \"1\",\n            \"name\": \"会议室\"\n        },\n        {\n            \"roomIndex\": \"2\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"3\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"4\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"5\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"6\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"7\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"8\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"9\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"10\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"11\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"12\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"13\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"14\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"15\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"16\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"17\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"18\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"19\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"20\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"21\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"22\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"23\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"24\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"25\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"26\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"27\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"28\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"29\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"30\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"31\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"32\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"33\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"34\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"35\",\n            \"name\": \"一二三四五六七八九十\"\n        },\n        {\n            \"roomIndex\": \"36\",\n            \"name\": \"一二三四五六七八九十\"\n        }\n    ]\n}";

    //测试代码
    private void Update()
    {
        //切换相机 漫游
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneEventDispath.DispatchSwitchScene("Roaming");
        }

        //切换相机 俯视图 省图模式
        if (Input.GetKeyDown(KeyCode.X))
        {
            SceneEventDispath.DispatchSwitchScene("Panorama");
        }

        //开启 关闭 Mask
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIEventDispatch.DispatchSwitchUIMask("true");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            UIEventDispatch.DispatchSwitchUIMask("false");
        }

        //测试点击工位电脑显示屏
        if (Input.GetKeyDown(KeyCode.B))
        {
            EmployeeData data = new EmployeeData()
            {
                name = "名字",
                department = "部门",
                posts = "岗位",
                state = "工作中",
                introduction = "个人简介\n asdasdasdasd 阿达吊袜带阿德飒飒的\n大王大大挖的啊实打实大苏打伟大撒旦哇打撒德瓦达打完大粪娃饿挖的挖的撒达瓦达瓦低洼地达瓦大屋顶",
                //introduction = "文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字文字文文字",
            };
            List<EmployeeData> dataList = new List<EmployeeData>();
            dataList.Add(data);
            AllEmployeeDatas alldata = new AllEmployeeDatas();
            alldata.datas = dataList;
            string json = JsonUtility.ToJson(alldata);
            Debug.Log(json);
            EmployeeDataDispath.DispatchSingleEmployeeInfomation(json);
        }

        //测试点击工位电脑显示屏
        if (Input.GetKeyDown(KeyCode.H))
        {
            EmployeeData data = new EmployeeData()
            {
                name = "",
                department = "部门",
                posts = "岗位",
                state = "工作中",
                introduction = "个人简介 \n",
            };
            List<EmployeeData> dataList = new List<EmployeeData>();
            dataList.Add(data);
            AllEmployeeDatas alldata = new AllEmployeeDatas();
            alldata.datas = dataList;
            string json = JsonUtility.ToJson(alldata);
            Debug.Log(json);
            EmployeeDataDispath.DispatchSingleEmployeeInfomation(json);
        }

        //测试开关门权限 
        if (Input.GetKeyDown(KeyCode.N))
        {
            PermissionData permissionData = new PermissionData()
            {
                allowVisit = "true",
                passwordRequire = "true",
                password = "114514",
            };
            AllPermissionData allPermissionData = new AllPermissionData()
            {
                datas = new List<PermissionData>() { permissionData },
            };

            string json = JsonUtility.ToJson(allPermissionData);
            Debug.Log(json);
            PermissionEventDispath.DispatchSingleRoomPermission(json);
        }

        #region IP测试

        //开关IP
        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneEventDispath.DispatchSwitchIP("true");
        }

        if (Input.GetKeyDown(KeyCode.Comma)) //逗号，键
        {
            SceneEventDispath.DispatchSwitchIP("false");
        }

        #endregion
        if (Input.GetKeyDown(KeyCode.Period))
        {
            SceneEventDispath.DispatchAllDoorplateData(_testDoorplateData);
        }

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            string data = "{\"datas\": [{\"roomIndex\": \"02\",\"name\": \"测试长字符串123s\"}]}";
            SceneEventDispath.DispatchSingleDoorplateData(data);
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            //测试自动漫游过程中 打断
            SceneEventDispath.DispatchAutoRounding("false");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneEventDispath.DispatchAutoRounding("true");
        }
        //允许进门消息
        if (Input.GetKeyDown(KeyCode.K))
        {
            PermissionsManagement.SendDoorPermissionCheckData();
        }
    }

#endif
}
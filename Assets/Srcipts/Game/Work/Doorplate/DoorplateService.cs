using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using TMPro;
using UnityEngine;


public interface IDoorplateService : IService
{
    Dictionary<int, string> GetAllDoorplateData();
    string GetSingleDoorplateData(int roomIndex);
}

public class DoorplateService : IDoorplateService
{
    private GameObject _doorplateRoot;
    private ApplicationContext _context;
    private Dictionary<int, string> _allDoorplateDataDic = new Dictionary<int, string>();

    public async void Init()
    {
        _context = Context.GetApplicationContext();
        //注册接收所有门的数据

        var eventHandleService = _context.GetService<IEventHandleService>();
        eventHandleService.AddListener(SceneEvent.AllDoorplateData, OnGetAllDoorplateData);
        eventHandleService.AddListener(SceneEvent.SingleDoorplateData, OnGetSingleDoorplateData);

        var sceneGameObjectService = _context.GetService<ISceneGameObjectService>();
        _doorplateRoot = await sceneGameObjectService.TryGetSceneGameObject("Doorplate");
    }

    public Dictionary<int, string> GetAllDoorplateData()
    {
        return this._allDoorplateDataDic;
    }

    public string GetSingleDoorplateData(int roomIndex)
    {
        string res = "";
        if (this._allDoorplateDataDic.TryGetValue(roomIndex, out res))
        {
            return res;
        }

        return res;
    }

    private void OnGetAllDoorplateData(EventArgs args)
    {
        SceneEventArgs data = args as SceneEventArgs;
        if (data == null)
        {
            XLog.E("所有房间名数据为空");
            return;
        }

        if (string.IsNullOrEmpty(data.Data))
        {
            XLog.E("所有房间名json数据为空");
            return;
        }

        //解析数据
        AllDoorplateData allDoorplateDatas = JsonUtility.FromJson<AllDoorplateData>(data.Data);
        if (allDoorplateDatas == null)
        {
            XLog.E("解析所有房间名数据数据错误");
            return;
        }

        if (allDoorplateDatas.datas == null || allDoorplateDatas.datas.Count <= 0)
        {
            XLog.E("所有房间名数据为空");
            return;
        }

        XLog.I($"接收到所有房间名数据,数据长度：{allDoorplateDatas.datas.Count}");
        InitAllDoorplateData(allDoorplateDatas);
        UpdateDoorplateGameObject();
    }


    private void OnGetSingleDoorplateData(EventArgs args)
    {
        SceneEventArgs data = args as SceneEventArgs;
        if (data == null)
        {
            XLog.E("单个房间名数据为空");
            return;
        }

        if (string.IsNullOrEmpty(data.Data))
        {
            XLog.E("单个房间名json数据为空");
            return;
        }

        //解析数据
        AllDoorplateData allDoorplateDatas = JsonUtility.FromJson<AllDoorplateData>(data.Data);
        if (allDoorplateDatas == null)
        {
            XLog.E("解析单个房间名数据数据错误");
            return;
        }

        if (allDoorplateDatas.datas == null || allDoorplateDatas.datas.Count <= 0)
        {
            XLog.E("单个房间名数据为空");
            return;
        }

        XLog.I($"接收到单个房间名数据：{allDoorplateDatas.datas[0].roomIndex} {allDoorplateDatas.datas[0].name}");
        UpdateDoorplateData(allDoorplateDatas.datas[0]);
        UpdateDoorplateGameObject();
    }


    private void InitAllDoorplateData(AllDoorplateData allDoorplateDatas)
    {
        foreach (var data in allDoorplateDatas.datas)
        {
            string roomIndex = data.roomIndex;
            if (string.IsNullOrEmpty(roomIndex))
            {
                XLog.E($"房间：{data.name}没有有效的编号");
                continue;
            }

            int index = Convert.ToInt32(roomIndex);

            _allDoorplateDataDic[index] = data.name;
        }
    }

    private void UpdateDoorplateData(DoorplateData data)
    {
        string roomIndex = data.roomIndex;
        if (string.IsNullOrEmpty(roomIndex))
        {
            XLog.E($"房间：{data.name}没有有效的编号");
            return;
        }

        int index = Convert.ToInt32(roomIndex);
        _allDoorplateDataDic[index] = data.name;

        var viewService = _context.GetService<IViewService>();
        var uiGuildTourWindow = viewService.GetView<UIGuildTourWindow>();
        if (uiGuildTourWindow != null)
        {
            uiGuildTourWindow.RefreshUI();
        }
    }


    private void UpdateDoorplateGameObject()
    {
        for (int i = 0; i < _doorplateRoot.transform.childCount; i++)
        {
            Transform doorPlate = _doorplateRoot.transform.GetChild(i);
            TMP_Text tMP_Text = doorPlate.GetComponent<TMP_Text>();
            string roomName = "";
            if (!doorPlate.name.Contains('m'))
            {
                XLog.E($"门牌{doorPlate.name} 没有有效的名名");
                continue;
            }

            string roomIndex = doorPlate.name.Split('m')[1].ToString();
            int index = Convert.ToInt32(roomIndex);

            _allDoorplateDataDic.TryGetValue(index, out roomName);
            if (string.IsNullOrEmpty(roomName))
            {
                XLog.I($"编号{roomIndex}房间没有数据");
                tMP_Text.text = "";
                continue;
            }

            tMP_Text.text = roomName;
        }

        var viewService = _context.GetService<IViewService>();
        var uiGuildTourWindow = viewService.GetView<UIGuildTourWindow>();
        if (uiGuildTourWindow != null)
        {
            uiGuildTourWindow.RefreshUI();
        }
    }
}

[Serializable]
public class AllDoorplateData
{
    public List<DoorplateData> datas;
}

[Serializable]
public class DoorplateData
{
    public string roomIndex;
    public string name;
}
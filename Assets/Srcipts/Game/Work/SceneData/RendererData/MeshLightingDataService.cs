using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using LP.Framework;
using UnityEngine;


public interface IMeshLightingDataService : IService
{
    List<MeshLightingDataLoader.CustomLightMapData> GetLightData();
}

public class MeshLightingDataService : IMeshLightingDataService
{
    [Serializable]
    public class AllLightData
    {
        public List<MeshLightingDataLoader.CustomLightMapData> lightDataList;
    }

    private ApplicationContext _context;
    private AllLightData _lightData;

    public void Init()
    {
        _context = Context.GetApplicationContext();
        LoadLightingData();
    }

    //加载存储办公室的光照数据
    private void LoadLightingData()
    {
        var resService = _context.GetService<IAssetLoadUtil>();
        TextAsset lightingData = resService.ResourcesLoad<TextAsset>("Configs/" + "LightingData.json");
        _lightData = JsonUtility.FromJson<AllLightData>(lightingData.text);
    }

    public List<MeshLightingDataLoader.CustomLightMapData> GetLightData()
    {
        return _lightData.lightDataList;
    }
}
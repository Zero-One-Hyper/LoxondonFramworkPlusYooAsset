using System;
using System.Collections.Generic;
using Loxodon.Framework.Contexts;
using UnityEngine;

//这个类不需要继承MonoBehaviour
public class MeshLightingDataLoader
{
    [Serializable]
    public class CustomLightMapData
    {
        public string hierarchyLevel;
        public int lightmapIndexOffset;
        public int lightmapIndex;
        public float lightmapScaleOffsetX;
        public float lightmapScaleOffsetY;
        public float lightmapScaleOffsetZ;
        public float lightmapScaleOffsetW;

        public Vector4 GetLightmapScaleOffset()
        {
            return new Vector4(lightmapScaleOffsetX, lightmapScaleOffsetY,
                lightmapScaleOffsetZ, lightmapScaleOffsetW);
        }

        public void SetLightmapScaleOffset(Vector4 value)
        {
            lightmapScaleOffsetX = value.x;
            lightmapScaleOffsetY = value.y;
            lightmapScaleOffsetZ = value.z;
            lightmapScaleOffsetW = value.w;
        }
    }

    private GameObject _meshRoot;
    private List<CustomLightMapData> _lightData;

    public void Init(GameObject meshRoot)
    {
        _meshRoot = meshRoot;
        var context = Context.GetApplicationContext();
        IMeshLightingDataService meshLightingDataService = context.GetService<IMeshLightingDataService>();
        //收集光照贴图数据
        _lightData = meshLightingDataService.GetLightData();
        SetLightingData();
    }


    //填充光照数据
    private void SetLightingData()
    {
        for (int i = 0; i < _lightData.Count; i++)
        {
            var data = _lightData[i];
            Transform go = _meshRoot.transform.Find(data.hierarchyLevel);
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            renderer.lightmapIndex = data.lightmapIndex + data.lightmapIndexOffset;
            renderer.lightmapScaleOffset = data.GetLightmapScaleOffset();
        }
    }
}
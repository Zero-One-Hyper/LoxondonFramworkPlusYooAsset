#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// 收集场景中的Mesh的光照贴图数据
/// </summary>
public class LightingMapTool : EditorWindow
{
    //[MenuItem("Tools/光照贴图/收集添加光照贴图数据")]
    private static void Init()
    {
        GetWindow<LightingMapTool>("收集、添加光照贴图数据");
    }

    public Transform parentRoot;
    public Transform reflectionProbRoot;

    private string _path = "Config/LightingData.json";
    private MeshLightingDataService.AllLightData _allLightData = new MeshLightingDataService.AllLightData();

    private void OnGUI()
    {
        parentRoot = EditorGUILayout.ObjectField(new GUIContent("场景根物体"),
            parentRoot, typeof(Transform), true) as Transform;
        reflectionProbRoot = EditorGUILayout.ObjectField(new GUIContent("反射探针根物体"),
            reflectionProbRoot, typeof(Transform), true) as Transform;
        if (GUILayout.Button("收集光照贴图数据"))
        {
            if (parentRoot != null)
            {
                _allLightData.lightDataList = new List<MeshLightingDataLoader.CustomLightMapData>();
                CollectLightDataInAllChildren(parentRoot, parentRoot.name);
                Debug.Log("Done");
            }
        }

        if (GUILayout.Button("设置反射探针数据"))
        {
            CollectReflectionProbeDataInAllChildren();
        }

        if (GUILayout.Button("存储光照信息"))
        {
            SaveLightDataConfig();
        }
    }

    //遍历子物体寻找使用光照贴图的物体
    private bool IsMeshUseLightMap(MeshRenderer meshRenderer)
    {
        if (meshRenderer.receiveGI == ReceiveGI.Lightmaps &&
            meshRenderer.gameObject.isStatic)
        {
            return true;
        }

        return false;
    }

    private void FillLightMapData(MeshRenderer meshRenderer, string hierarchyLevel)
    {
        MeshLightingDataLoader.CustomLightMapData lightmapData = new MeshLightingDataLoader.CustomLightMapData();
        lightmapData.hierarchyLevel = hierarchyLevel;
        lightmapData.lightmapIndex = meshRenderer.lightmapIndex;
        lightmapData.SetLightmapScaleOffset(meshRenderer.lightmapScaleOffset);

        _allLightData.lightDataList.Add(lightmapData);
    }

    private void CollectLightDataInAllChildren(Transform root, string parentLevel)
    {
        string hierarchyLevel = parentLevel;
        if (root.TryGetComponent(out MeshRenderer meshRenderer))
        {
            if (IsMeshUseLightMap(meshRenderer))
            {
                FillLightMapData(meshRenderer, hierarchyLevel);
            }
        }

        int childCount = root.transform.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                var child = root.transform.GetChild(i);
                string level = hierarchyLevel + "/" + child.name;
                CollectLightDataInAllChildren(child, level);
            }
        }
    }

    private void SaveLightDataConfig()
    {
        if (_allLightData == null || _allLightData.lightDataList == null ||
            _allLightData.lightDataList.Count < 0)
        {
            return;
        }

        string js = JsonUtility.ToJson(_allLightData, true);
        string jsonPath = Application.streamingAssetsPath + "/" + _path;
        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
        }


        File.WriteAllText(jsonPath, js);
        Debug.Log("Done!!!");
        AssetDatabase.Refresh();
    }

    private void CollectReflectionProbeDataInAllChildren()
    {
        for (int i = 0; i < reflectionProbRoot.childCount; i++)
        {
            var child = reflectionProbRoot.GetChild(i);
            var reflectionProb = child.GetComponent<ReflectionProbe>();
            var tex = reflectionProb.bakedTexture;
            reflectionProb.mode = ReflectionProbeMode.Custom;
            reflectionProb.customBakedTexture = tex;
            //Debug.Log(114514);
        }
    }
}
#endif
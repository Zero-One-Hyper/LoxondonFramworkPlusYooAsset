#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class AdsDataCollecter : EditorWindow
{

    [MenuItem("Tools/场景数据/收集广告告示牌数据")]
    private static void Init()
    {
        GetWindow<AdsDataCollecter>("收集广告告示牌数据");
    }

    public Transform parentRoot;

    private string _path = "Configs/AdsDataConfig.json";

    private void OnGUI()
    {
        parentRoot = EditorGUILayout.ObjectField(new GUIContent("广告告示父物体"),
            parentRoot, typeof(Transform), true) as Transform;

        if (GUILayout.Button("收集广告告示牌数据"))
        {
            if (parentRoot != null)
            {
                this.CollectAdsData(parentRoot);
            }
        }
    }
    private void CollectAdsData(Transform root)
    {
        List<string> datas = new List<string>();
        for (int i = 0; i < root.childCount; i++)
        {
            var adsItem = root.GetChild(i).gameObject;
            Ads ads = adsItem.GetComponent<Ads>();
            datas.Add(ads.GetAdsData());
        }

        if (datas == null && datas.Count < 0)
        {
            return;
        }

        AdsDataConfig adsDataConfig = new AdsDataConfig();
        adsDataConfig.adsData = datas;
        string js = JsonConvert.SerializeObject(adsDataConfig);
        string jsonPath = Application.streamingAssetsPath + "/" + _path;
        if (File.Exists(jsonPath))
        {
            File.Delete(jsonPath);
        }

        JsonSerializer serializer = new JsonSerializer();
        TextReader tr = new StringReader(js);
        JsonTextReader jtr = new JsonTextReader(tr);
        object obj = serializer.Deserialize(jtr);

        StringWriter textWriter = new StringWriter();
        JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
        {
            Formatting = Formatting.Indented,
            Indentation = 4,
            IndentChar = ' '
        };
        serializer.Serialize(jsonWriter, obj);

        File.WriteAllText(jsonPath, textWriter.ToString());
        Debug.Log("Done!!!");
        AssetDatabase.Refresh();
    }
}
#endif
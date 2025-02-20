#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class RoomPositionCollect : EditorWindow
{
    [MenuItem("Tools/房间位置收集/收集房间位置")]
    private static void Init()
    {
        GetWindow<RoomPositionCollect>("收集存储房间位置");
    }

    private Transform _parentRoot;

    private string _path = "Config/RoomPositionConfig.json";
    private RoomService.AllRoomPointData _allRoomPointData = new RoomService.AllRoomPointData();

    private void OnGUI()
    {
        _parentRoot = EditorGUILayout.ObjectField(new GUIContent("父物体"),
            _parentRoot, typeof(Transform), true) as Transform;
        if (GUILayout.Button("收集办公室点位数据"))
        {
            if (_parentRoot != null)
            {
                if (_allRoomPointData == null)
                {
                    _allRoomPointData = new RoomService.AllRoomPointData();
                    _allRoomPointData.allPointData = new List<RoomService.RoomPointData>();
                }

                _allRoomPointData.allPointData.Clear();
                _allRoomPointData.allPointData = new List<RoomService.RoomPointData>();
                CollectAllRoomPosition(_parentRoot);
                SaveRoomDataConfig();
                Debug.Log("All Done");
            }
        }
    }

    private void CollectAllRoomPosition(Transform parentRoot)
    {
        for (int i = 0; i < parentRoot.childCount; i++)
        {
            var roomPoint = parentRoot.GetChild(i);
            RoomService.RoomPointData data = new RoomService.RoomPointData();
            data.SetRoomPointData(roomPoint);
            data.roomPointName = roomPoint.name;
            _allRoomPointData.allPointData.Add(data);
        }
    }

    private void SaveRoomDataConfig()
    {
        if (_allRoomPointData == null || _allRoomPointData.allPointData == null ||
            _allRoomPointData.allPointData.Count < 0)
        {
            return;
        }

        string js = JsonConvert.SerializeObject(_allRoomPointData);
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
            Formatting = Formatting.Indented, Indentation = 4, IndentChar = ' '
        };
        serializer.Serialize(jsonWriter, obj);

        File.WriteAllText(jsonPath, textWriter.ToString());
        Debug.Log("Done!!!");
        AssetDatabase.Refresh();
    }
}
#endif
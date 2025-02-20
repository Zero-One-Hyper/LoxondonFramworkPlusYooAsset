#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class RoomGuildPointTool : EditorWindow
{
    [MenuItem("Tools/场景数据/收集房间导航点数据")]
    private static void Init()
    {
        GetWindow<RoomGuildPointTool>("收集房间导航点数据");
    }

    public Transform roomGuildPointRoot;

    RoomGuildPoints roomGuildPoints = new RoomGuildPoints();
    private string _path = "Configs/RoomGuildPoints.json";


    private void OnGUI()
    {
        roomGuildPointRoot = EditorGUILayout.ObjectField(new GUIContent("根物体"),
            roomGuildPointRoot, typeof(Transform), true) as Transform;

        if (GUILayout.Button("收集房间导航点数据"))
        {
            if (roomGuildPointRoot != null)
            {

                CollectRoomGuildPointData(roomGuildPointRoot);
                Debug.Log("Done");
                SaveRoomGuildPointDataConfig();
            }
        }
    }

    private void CollectRoomGuildPointData(Transform parentRoot)
    {
        roomGuildPoints = new RoomGuildPoints();
        roomGuildPoints.roomGuildPoints = new List<RoomGuildPoint>();
        for (int i = 0; i < parentRoot.childCount; i++)
        {
            Transform roomGuildPoint = parentRoot.GetChild(i);
            Vector3 roomDoorPositoin = roomGuildPoint.GetChild(0).position;
            RoomGuildPoint point = new RoomGuildPoint(roomGuildPoint.position, roomDoorPositoin);
            roomGuildPoints.roomGuildPoints.Add(point);
        }
        Debug.Log(roomGuildPoints.roomGuildPoints.Count);
    }

    private void SaveRoomGuildPointDataConfig()
    {
        if (roomGuildPoints == null || roomGuildPoints.roomGuildPoints == null ||
            roomGuildPoints.roomGuildPoints.Count < 0)
        {
            return;
        }

        string js = JsonConvert.SerializeObject(roomGuildPoints);
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
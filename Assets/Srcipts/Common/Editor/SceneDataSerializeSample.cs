#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SceneDataSerializeSample : EditorWindow
{
    //[MenuItem("Tools/场景数据/示例收集场景中的诗句")]
    private static void Init()
    {
        GetWindow<SceneDataSerializeSample>("收集数据");
    }

    public Transform parentRoot;
    public Transform doorColliders;


    private string _path = "Configs/路径.json";
    //private DoorController.DoorColliderData allDoorColliderData; //类

    private void OnGUI()
    {
        parentRoot = EditorGUILayout.ObjectField(new GUIContent("父物体"),
            parentRoot, typeof(Transform), true) as Transform;

        if (GUILayout.Button("收集数据"))
        {
            if (parentRoot != null)
            {
                //allDoorColliderData = new DoorController.DoorColliderData();
                //allDoorColliderData.allDoorBingData = new List<DoorController.DoorColliderBind>();
                //CollectDoorBindData(parentRoot);
                //Debug.Log("Done");
            }
        }

        if (GUILayout.Button("填充数据"))
        {
            FillData(parentRoot, doorColliders);
        }
    }

    private void CollectDoorBindData(Transform root)
    {
        //收集数据
    }

    private void FillData(Transform root, Transform colliderRoot)
    {
        //加载门对应的Collider数据
        //var textAsset = Resources.Load<TextAsset>("Configs/DoorColliderBingConfig.json");
        string text = File.ReadAllText(Application.streamingAssetsPath + "/Configs/DoorColliderBingConfig.json");

        /*
        DoorController.DoorColliderData data = JsonUtility.FromJson<DoorController.DoorColliderData>(text);

        //绑定
        Dictionary<string, DoorCollider> colliders = new Dictionary<string, DoorCollider>();
        for (int i = 0; i < colliderRoot.childCount; i++)
        {
            var collider = colliderRoot.GetChild(i).gameObject;
            if (collider.TryGetComponent(out DoorCollider doorCollider))
            {
                colliders.Add(collider.name, doorCollider);
            }
            else
            {
                colliders.Add(collider.name, collider.AddComponent<DoorCollider>());
            }
        }

        for (int i = 0; i < data.allDoorBingData.Count; i++)
        {
            if (!root.GetChild(i).TryGetComponent(out Door door))
            {
                door = root.GetChild(i).gameObject.AddComponent<Door>();
            }

            DoorController.DoorColliderBind doorBindData = data.allDoorBingData[i];
            DoorCollider collider = colliders[doorBindData.colliderPath];

            door.SetDataEditorMode(doorBindData, collider);
            Debug.Log($"Done {door.gameObject.name}");
        }
        */
    }
    /*
    private void SaveLightDataConfig()
    {
        if (allDoorColliderData == null || allDoorColliderData.allDoorBingData == null ||
            allDoorColliderData.allDoorBingData.Count < 0)
        {
            return;
        }

        string js = JsonConvert.SerializeObject(allDoorColliderData);
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
    */
}
#endif
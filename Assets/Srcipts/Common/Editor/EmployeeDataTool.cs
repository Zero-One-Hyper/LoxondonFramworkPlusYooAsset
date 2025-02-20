#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class EmployeeDataTool : EditorWindow
{
    [MenuItem("Tools/工位数据/收集工位数据")]
    private static void Init()
    {
        GetWindow<EmployeeDataTool>("收集工位数据");
    }

    public Transform parentRoot;
    public Transform doorColliders;


    private string _path = "Configs/EmployeeCubicleData.json";

    private void OnGUI()
    {
        parentRoot = EditorGUILayout.ObjectField(new GUIContent("根物体"),
            parentRoot, typeof(Transform), true) as Transform;

        if (GUILayout.Button("转换Transform的Scale为Collider Size"))
        {
            if (parentRoot != null)
            {
                for (int i = 0; i < parentRoot.childCount; i++)
                {
                    Transform employeePlace = parentRoot.GetChild(i);
                    BoxCollider boxCollider = employeePlace.GetComponent<BoxCollider>();
                    boxCollider.size = employeePlace.localScale;
                    employeePlace.localScale = Vector3.one;
                }
            }
        }
        if (GUILayout.Button("收集工位Index信息"))
        {
            SaveEmployeeCubicleDataConfig();
        }

    }
    private void SaveEmployeeCubicleDataConfig()
    {
        List<EmployeeCubicleData> employeeCubicleDataList = new List<EmployeeCubicleData>();
        AllEmployeeCubicleDataes allEmployeeCubicleDataes = new AllEmployeeCubicleDataes();

        for (int i = 0; i < parentRoot.childCount; i++)
        {
            Transform employeePlace = parentRoot.GetChild(i);
            EmployeeCubicle employeeCubicle = employeePlace.GetComponent<EmployeeCubicle>();
            employeeCubicleDataList.Add(new EmployeeCubicleData() { cubicleIndex = employeeCubicle.GetCubicleId() });
        }
        allEmployeeCubicleDataes.employeeCubicleDataList = employeeCubicleDataList;

        string js = JsonConvert.SerializeObject(allEmployeeCubicleDataes);
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
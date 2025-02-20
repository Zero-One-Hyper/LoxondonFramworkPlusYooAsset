using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Loxodon.Framework.Contexts;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
#if UNITY_EDITOR
public class LineBuilder : EditorWindow
{
    [MenuItem("Tools/车辆路线/车辆线路编辑")]
    private static void Init()
    {
        GetWindow<LineBuilder>("车辆线路编辑");
    }

    public GameObject StationModel;
    public Transform Line;

    public Mesh _linemesh;
    public CinemachinePath _linepath;

    private void OnGUI()
    {
        _linemesh =
            EditorGUILayout.ObjectField(new GUIContent("Mesh-资源中的模型里的mesh"), _linemesh, typeof(Mesh), true) as Mesh;
        _linepath = EditorGUILayout.ObjectField(new GUIContent("cinemachinepath"), _linepath, typeof(CinemachinePath),
            true) as CinemachinePath;
        if (GUILayout.Button("生成Linerenderer"))
        {
            var _waypoint = new CinemachinePath.Waypoint[_linemesh.vertexCount];
            _linepath.m_Waypoints = _waypoint;
            Vector3 tempPos;
            for (var i = 0; i < _linemesh.vertexCount; i++)
            {
                tempPos = _linemesh.vertices[i];
                _linepath.m_Waypoints[i].position = tempPos;
            }
        }

        Line = EditorGUILayout.ObjectField(new GUIContent("站点父项"), Line, typeof(Transform), true) as Transform;
        StationModel =
            EditorGUILayout.ObjectField(new GUIContent("站点模型"), StationModel, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("生成线路"))
        {
            foreach (Transform _line in Line)
            {
                GameObject instance = PrefabUtility.InstantiatePrefab(StationModel) as GameObject;
                instance.transform.parent = _line;
            }
        }

        if (GUILayout.Button("line排序"))
        {
            var Alist = new List<CinemachinePath.Waypoint>();
            var Blist = new List<CinemachinePath.Waypoint>();
            //顺时针
            var _lineLenght = _linepath.m_Waypoints.Length;
            for (var i = 1; i < _lineLenght; i += 2)
            {
                Alist.Add(_linepath.m_Waypoints[i]);
                Blist.Add(_linepath.m_Waypoints[_lineLenght - (i + 1)]);
            }

            Alist.AddRange(Blist);
            _linepath.m_Waypoints = Alist.ToArray();
            //可根据斜率删除点
            //_Line5Path.m_Waypoints[1].tangent


        }

        if (GUILayout.Button("line斜率机选"))
        {
            var _lineLenght = _linepath.m_Waypoints.Length;
            Vector3 _Temp;
            for (int i = 1; i < _lineLenght; i++)
            {
                _Temp = _linepath.m_Waypoints[i].position - _linepath.m_Waypoints[i - 1].position;
                _Temp.y = 0;
                _Temp *= 0.25f;
                _linepath.m_Waypoints[i].tangent = _Temp;
                _linepath.m_Resolution = 1;
            }
        }
    }
}
#endif

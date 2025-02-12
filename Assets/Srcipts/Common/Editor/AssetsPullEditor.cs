#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnitConfig.Editor
{
    /// <summary>
    /// 模型资产拆分
    /// </summary>
    public class AssetsPullEditor : EditorWindow
    {
        private static EditorWindow window;
        private static GameObject model;
        private static bool isSelMesh = false;
        private static bool isSelMat = false;
        private static bool isSelPrefab = false;
        private static string assetName;

        private static string assetPath;

        /// <summary>
        /// 获取模型mesh,mat
        /// </summary>
        [MenuItem("Tools/一键提取模型(mesh|mat|prefab)")]
        public static void GetModelMat()
        {
            window = EditorWindow.GetWindow(typeof(AssetsPullEditor));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Model Split", EditorStyles.label);
            GUILayout.Space(10);
            assetName = EditorGUILayout.TextField("Asset Name", assetName);
            GUILayout.Space(10);
            model = (GameObject)EditorGUILayout.ObjectField("Model", model, typeof(GameObject));
            GUILayout.Space(10);
            isSelMesh = GUILayout.Toggle(isSelMesh, "Mesh");
            isSelMat = GUILayout.Toggle(isSelMat, "Mat");
            isSelPrefab = GUILayout.Toggle(isSelPrefab, "Prefab");
            if (GUILayout.Button("Create"))
            {
                assetPath = Application.dataPath + "/TractionSystem/Res";
                if (model == null)
                {
                    XLog.I("选择需要操作的模型");
                }

                if (isSelMesh)
                {
                    CreateMesh();
                }

                if (isSelMat)
                {
                    CreateMat();
                }

                if (isSelPrefab)
                {
                    CreateModelPrefab();
                }

                window.Close();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 一键提取mesh
        /// </summary>
        /// <param name="obj"></param>
        private void CreateMesh()
        {
            XLog.I("Create Mesh");
            var path = CreateDirectory("mesh");
            path = path.Substring(path.LastIndexOf("A"));
            GetAsset(model, path,false);
        }

        /// <summary>
        /// 一键提取材质
        /// </summary>
        /// <param name="obj"></param>
        private void CreateMat()
        {
            XLog.I("Create Mat");
            var path = CreateDirectory("mat");
            path = path.Substring(path.LastIndexOf("A"));
            GetAsset(model, path,true);
        }

        /// <summary>
        /// 递归循环
        /// </summary>
        /// <param name="go"></param>
        /// <param name="path"></param>
        void GetAsset(GameObject go,string path,bool isMat)
        {
            if (go.transform.childCount > 0)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var o = go.transform.GetChild(i).gameObject;
                    GetAsset(o,path,isMat);
                }
            }
            else
            {
                if(isMat)
                    GetMatUtil(go, path);
                else
                {
                    GetMeshUtil(go, path);
                }
            }
        }
        
        void GetMeshUtil(GameObject go,string path)
        {
            Mesh mesh = null;
            if (go.GetComponent<MeshFilter>())
            {
                mesh = Instantiate(go.GetComponent<MeshFilter>().sharedMesh);
            }
            else if (go.GetComponent<SkinnedMeshRenderer>())
            {
                mesh = Instantiate(go.GetComponent<SkinnedMeshRenderer>().sharedMesh);
            }
            else
            {
                XLog.I("no mesh");
            }

            //保存为资产
            if (mesh)
                AssetDatabase.CreateAsset(mesh,
                    path + "/" + assetName + "_" + mesh.name.Replace("(Clone)", "") + ".mesh");
        }


        void GetMatUtil(GameObject go,string path)
        {
            Material[] mats = new Material[]{};
            if (go.GetComponent<MeshRenderer>())
            {
                mats = go.GetComponent<MeshRenderer>().sharedMaterials;
            }
            else if (go.GetComponent<SkinnedMeshRenderer>())
            {
                mats = go.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
            }
            else
            {
                XLog.I("no mat");
            }

            if (mats.Length > 0)
            {
                for (int j = 0; j < mats.Length; j++)
                {
                    var mat = Instantiate(mats[j]);
                    if (mat)
                        AssetDatabase.CreateAsset(mat,
                            path + "/" + assetName + "_" + mat.name.Replace("(Clone)", "") + ".mat");
                }
            }
        }

        /// <summary>
        /// 一键生成空模型数据载体
        /// </summary>
        private void CreateModelPrefab()
        {
            XLog.I("Create prefab");
            var path = CreateDirectory("prefab");
            path = path.Substring(path.LastIndexOf("A"));
            var o = Instantiate(model);
            for (int i = 0; i < o.transform.childCount; i++)
            {
                var oc = o.transform.GetChild(i);
                if (oc.GetComponent<MeshFilter>())
                {
                    DestroyImmediate(oc.GetComponent<MeshFilter>());
                }
                if (oc.GetComponent<MeshRenderer>())
                {
                    DestroyImmediate(oc.GetComponent<MeshRenderer>());
                }

                if (oc.GetComponent<SkinnedMeshRenderer>())
                {
                    DestroyImmediate(oc.GetComponent<SkinnedMeshRenderer>());
                    var n = oc.name;
                    oc.name = n + "_SMR";
                }
            }
            //保存为预制体
            PrefabUtility.SaveAsPrefabAsset(o, path + "/" + assetName + "_" + o.name.Replace("(Clone)", "") + ".prefab");
            DestroyImmediate(o);
        }

        /// <summary>
        /// 创建文件目录并删除旧资产
        /// </summary>
        private string CreateDirectory(string type)
        {
            var dicPath = assetPath + "/" + type + "/" + assetName.ToLower();
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }

            DeleteAllFile(dicPath);
            return dicPath;
        }

        /// <summary>
        /// 删除文件目录下的所有文件
        /// </summary>
        bool DeleteAllFile(string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta"))
                    {
                        continue;
                    }

                    string FilePath = fullPath + "/" + files[i].Name;
                    File.Delete(FilePath);
                }

                return true;
            }

            return false;
        }
    }
}
#endif

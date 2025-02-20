
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LP.Framework.Editor
{
#if UNITY_EDITOR
    public class AssetBundleExporter
    {
        [MenuItem("Assets/AB Name/标记AB名字[标记文件夹])")]
        public static void GenAssetNameDir()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                return;
            }
            string workPath = EditorUtils.AssetsPath2ABSPath(selectPath); 
            string assetBundleName =
                EditorUtils.AssetPath2ReltivePath(selectPath)
                    .ToLower();
            string asset_path = workPath.Replace (Application.dataPath, "Assets");
            AssetImporter ai = AssetImporter.GetAtPath(asset_path);
            ai.assetBundleName = assetBundleName;
        }


        [MenuItem("Assets/AB Name/标记AB名字")]
        public static void GenAssetNameFile()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                return;
            }
            string assetBundleName =
                EditorUtils.AssetPath2ReltivePath(selectPath)
                    .ToLower();
            Object obj = Selection.activeObject;
            string path = AssetDatabase.GetAssetPath(obj);
            var abName = assetBundleName.Replace(@"assets\", "");
            AssetImporter.GetAtPath(path).assetBundleName =Path.Combine(abName,obj.name.ToLower());
        }
        
        //自动设置选中目录下的AssetBundle Name
        //[MenuItem("Assets/AB Name/标记AB名字[文件夹名])")]
        public static void GenAssetNameAsFolderName()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                return;
            }

            AutoGenAssetNameInFolder(selectPath, true);
        }

        //自动设置选中目录下的AssetBundle Name
        //[MenuItem("Assets/AB Name/标记AB名字[文件名])")]
        public static void GenAssetNameAsFileName()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                return;
            }

            AutoGenAssetNameInFolder(selectPath, false);

            AssetDatabase.SaveAssets();
        }
        [MenuItem("Assets/AB Name/清楚无用ABName)")]
        public static void RemoveABName()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        /// <summary>
        // 递归处理文件夹下所有Asset 文件
        /// </summary>
        /// <param name="folderPath">Asset目录下文件夹</param>
        private static void AutoGenAssetNameInFolder(string folderPath, bool useFolderName)
        {
            if (folderPath == null)
            {
                return;
            }

            string workPath = EditorUtils.AssetsPath2ABSPath(folderPath); //EditUtils.GetFullPath4AssetsPath(folderPath);
            string assetBundleName =
                EditorUtils.AssetPath2ReltivePath(folderPath)
                    .ToLower(); //EditUtils.GetReltivePath4AssetPath(folderPath).ToLower();
            assetBundleName = assetBundleName.Replace("resources/", "");
            //处理文件
            var filePaths = Directory.GetFiles(workPath);
            for (int i = 0; i < filePaths.Length; ++i)
            {
                if (!AssetFileFilter.IsAsset(filePaths[i]))
                {
                    continue;
                }

                string fileName = Path.GetFileName(filePaths[i]);

                string fullFileName = string.Format("{0}/{1}", folderPath, fileName);

                AssetImporter ai = AssetImporter.GetAtPath(fullFileName);
                if (ai == null)
                {
                    continue;
                }
                else
                {
                    if (useFolderName)
                    {
                        //ai.assetBundleName = assetBundleName + ".bundle";//带后缀
                        ai.assetBundleName = assetBundleName; //不带后缀
                    }
                    else
                    {
                        //带.bundle后缀
                        // ai.assetBundleName = string.Format("{0}/{1}.bundle", assetBundleName,
                        //     PathHelper.FileNameWithoutSuffix(fileName));
                        
                        //不带.bundle后缀
                        Debug.Log(string.Format("{0}/{1}", assetBundleName,
                            PathHelper.FileNameWithoutSuffix(fileName)));
                        var name = string.Format("{0}/{1}", assetBundleName,
                            PathHelper.FileNameWithoutSuffix(fileName));
                        var bName = name.Replace(@"assets\", "");
                        ai.assetBundleName = bName;
                    }
                }

                //ai.SaveAndReimport();
                //Log.i("Success Process Asset:" + fileName);
            }

            //递归处理文件夹
            var dirs = Directory.GetDirectories(workPath);
            for (int i = 0; i < dirs.Length; ++i)
            {
                string fileName = Path.GetFileName(dirs[i]);

                fileName = string.Format("{0}/{1}", folderPath, fileName);
                AutoGenAssetNameInFolder(fileName, useFolderName);
            }
        }
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LP.Framework.Editor
{
#if UNITY_EDITOR
    public class EditorFileUtils
    {
        public static List<string> GetDirSubFilePathList(string dirABSPath, bool isRecursive = true, string suffix = "")
        {
            List<string> pathList = new List<string>();
            DirectoryInfo di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            FileInfo[] files = di.GetFiles();
            foreach (FileInfo fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }
                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                DirectoryInfo[] dirs = di.GetDirectories();
                foreach (DirectoryInfo d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        public static List<string> GetDirSubDirNameList(string dirABSPath)
        {
            List<string> nameList = new List<string>();
            DirectoryInfo di = new DirectoryInfo(dirABSPath);

            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo d in dirs)
            {
                nameList.Add(d.Name);
            }

            return nameList;
        }

        public static string GetFileName(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");

            if (lastIndex >= 0)
            {
                return name.Substring(lastIndex + 1);
            }
            else
            {
                return name;
            }
        }

        public static string GetFileNameWithoutExtend(string absOrAssetsPath)
        {
            string fileName = GetFileName(absOrAssetsPath);
            int lastIndex = fileName.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return fileName.Substring(0, lastIndex);
            }
            else
            {
                return fileName;
            }
        }

        public static string RemoveFileExtend(string fileName)
        {
            int lastIndex = fileName.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return fileName.Substring(0, lastIndex);
            }
            else
            {
                return fileName;
            }
        }

        public static string GetFileExtendName(string absOrAssetsPath)
        {
            int lastIndex = absOrAssetsPath.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return absOrAssetsPath.Substring(lastIndex);
            }

            return string.Empty;
        }

        public static string GetDirPath(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            int lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }
    }
#endif
}


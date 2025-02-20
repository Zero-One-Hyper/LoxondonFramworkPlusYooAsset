using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP.Framework.Editor
{
    public class AssetFileFilter
    {
        public static bool IsAsset(string fileName)
        {
            if (fileName.EndsWith(".meta") || fileName.EndsWith(".gaf") || fileName.EndsWith(".DS_Store"))
            {
                return false;
            }
            return true;
        }

        public static bool IsAssetBundle(string fileName)
        {
            if (fileName.EndsWith(".bundle"))
            {
                return true;
            }
            return false;
        }

        public static bool IsConfigTable(string fileName)
        {
            if (fileName.EndsWith(".txt"))
            {
                return true;
            }
            return false;
        }
    }
}


// Copyright 2021 Ikina Games
// Author : Seung Ha Kim (Syadeu)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Point.Collections.Editor;
using Point.Collections.ResourceControl;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Point.Collections.ResourceControl.Editor
{
    public static class ResourceAddressesEditorUtils
    {
        private static AssetAddressesSetting Setting
        {
            get
            {
                return PointProjectSettings.Instance.GetSetting<AssetAddressesSetting>();
            }
        }

        #region AssetID

        //private static FieldInfo s_AssetIDsField;
        //private static FieldInfo AssetIDsField
        //{
        //    get
        //    {
        //        if (s_AssetIDsField == null)
        //        {
        //            s_AssetIDsField
        //                = TypeHelper.TypeOf<AssetAddressesSetting>.GetFieldInfo("m_AssetIDs");
        //        }

        //        return s_AssetIDsField;
        //    }
        //}
        //private static AssetID[] AssetIDs
        //{
        //    get
        //    {
        //        return (AssetID[])AssetIDsField.GetValue(Setting);
        //    }
        //    set
        //    {
        //        AssetIDsField.SetValue(Setting, value);
        //        EditorUtility.SetDirty(PointProjectSettings.Instance);
        //        AssetDatabase.SaveAssetIfDirty(PointProjectSettings.Instance);
        //    }
        //}
        //private static Dictionary<AssetID, int> s_RegisteredAssets;
        //private static Dictionary<AssetID, int> RegisteredAssets
        //{
        //    get
        //    {
        //        if (s_RegisteredAssets == null)
        //        {
        //            s_RegisteredAssets = new Dictionary<AssetID, int>();
        //            AssetID[] ids = AssetIDs;
        //            for (int i = 0; i < ids.Length; i++)
        //            {
        //                s_RegisteredAssets.Add(ids[i], i);
        //            }
        //        }

        //        return s_RegisteredAssets;
        //    }
        //}

        public static bool IsTrackedAsset(this UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return Setting.IsTrackedAsset(in path);
        }
        public static bool IsTrackedAsset(this PointProjectSettings _, in string assetPath)
        {
            AssetID id = new AssetID(new Hash(assetPath));
            return Setting.IsTrackedAsset(in id);
        }
        public static AssetID UpdateAsset(this PointProjectSettings _, in string prevAssetPath, in string targetAssetPath)
        {
            return Setting.UpdateAsset(in prevAssetPath, in targetAssetPath);
        }
        public static AssetID RegisterAsset(this UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return Setting.RegisterAsset(in path);
        }
        public static AssetID RegisterAsset(this PointProjectSettings _, in string assetPath)
        {
            return Setting.RegisterAsset(in assetPath);
        }
        //public static void RemoveAssets(this PointProjectSettings _, in string[] assetPaths)
        //{
        //    AssetID[] ids = new AssetID[assetPaths.Length];
        //    int[] indics = new int[assetPaths.Length];
        //    for (int i = 0; i < assetPaths.Length; i++)
        //    {
        //        ids[i] = new AssetID(new Hash(assetPaths[i]));
        //        if (!RegisteredAssets.ContainsKey(ids[i]))
        //        {
        //            throw new System.Exception();
        //        }

        //        indics[i] = RegisteredAssets[ids[i]];
        //        RegisteredAssets.Remove(ids[i]);
        //    }

        //    List<AssetID> temp = AssetIDs.ToList();
        //    for (int i = 0; i < indics.Length; i++)
        //    {
        //        temp.RemoveAt(indics[i]);
        //    }
        //    AssetIDs = temp.ToArray();

        //    Point.Log(Point.LogChannel.Editor,
        //        $"Multiple assets({ids[0].Hash}, and {ids.Length - 1} more assets) has been removed and no longer tracked by resource manager.");
        //}
        public static void RemoveAsset(this UnityEngine.Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            Setting.RemoveAsset(in path);
        }
        public static void RemoveAsset(this PointProjectSettings _, in string assetPath)
        {
            Setting.RemoveAsset(in assetPath);
        }

        public static UnityEngine.Object GetAsset(this in AssetID other)
        {
            string path = other.Hash.Key;
            return AssetDatabase.LoadAssetAtPath(path, TypeHelper.TypeOf<UnityEngine.Object>.Type);
        }

        public static bool HasAssetBundle(this in AssetID other)
        {
            // https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImplicitAssetBundleName.html
            string bundleName = AssetDatabase.GetImplicitAssetBundleName(other.Hash.Key);
            return !string.IsNullOrEmpty(bundleName);
        }
        //public static UnityEngine.AssetBundle GetAssetBundle(this in AssetID other)
        //{
        //    string name = AssetDatabase.GetImplicitAssetBundleName(other.Hash.Key);
        //    return 
        //}

        #endregion

        public static void UpdateAssetBundleID(this ResourceAddresses other, params string[] assetBundleNames)
        {
            FieldInfo field = TypeHelper.TypeOf<ResourceAddresses>.GetFieldInfo("m_TrackedAssetBundles");

            AssetID[] assetIDs = new AssetID[assetBundleNames.Length];
            for (int i = 0; i < assetIDs.Length; i++)
            {
                assetIDs[i] = new AssetID(new Hash(assetBundleNames[i]));
            }

            field.SetValue(ResourceAddresses.Instance, assetIDs);
            EditorUtility.SetDirty(ResourceAddresses.Instance);
        }
    }
}

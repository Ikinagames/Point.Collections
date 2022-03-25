// Copyright 2022 Ikina Games
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

#if UNITY_2020
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Editor;
using Point.Collections.ResourceControl;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using UnityEditor;

namespace Point.Collections.ResourceControl.Editor
{
    public static class ResourceAddressesEditorUtils
    {
        //private static AssetAddressesSetting Setting
        //{
        //    get
        //    {
        //        return PointProjectSettings.Instance.GetSetting<AssetAddressesSetting>();
        //    }
        //}

        //#region AssetID

        //public static bool IsTrackedAsset(this UnityEngine.Object obj)
        //{
        //    string path = AssetDatabase.GetAssetPath(obj);
        //    return Setting.IsTrackedAsset(in path);
        //}
        //public static bool IsTrackedAsset(this PointProjectSettings _, in string assetPath)
        //{
        //    return Setting.IsTrackedAsset(in assetPath);
        //}
        //public static AssetID UpdateAsset(this PointProjectSettings _, in string prevAssetPath, in string targetAssetPath)
        //{
        //    return Setting.UpdateAsset(in prevAssetPath, in targetAssetPath);
        //}
        //public static AssetID RegisterAsset(this UnityEngine.Object obj)
        //{
        //    string path = AssetDatabase.GetAssetPath(obj);
        //    return Setting.RegisterAsset(in path);
        //}
        //public static AssetID RegisterAsset(this PointProjectSettings _, in string assetPath)
        //{
        //    return Setting.RegisterAsset(in assetPath);
        //}
        ////public static void RemoveAssets(this PointProjectSettings _, in string[] assetPaths)
        ////{
        ////    AssetID[] ids = new AssetID[assetPaths.Length];
        ////    int[] indics = new int[assetPaths.Length];
        ////    for (int i = 0; i < assetPaths.Length; i++)
        ////    {
        ////        ids[i] = new AssetID(new Hash(assetPaths[i]));
        ////        if (!RegisteredAssets.ContainsKey(ids[i]))
        ////        {
        ////            throw new System.Exception();
        ////        }

        ////        indics[i] = RegisteredAssets[ids[i]];
        ////        RegisteredAssets.Remove(ids[i]);
        ////    }

        ////    List<AssetID> temp = AssetIDs.ToList();
        ////    for (int i = 0; i < indics.Length; i++)
        ////    {
        ////        temp.RemoveAt(indics[i]);
        ////    }
        ////    AssetIDs = temp.ToArray();

        ////    Point.Log(Point.LogChannel.Editor,
        ////        $"Multiple assets({ids[0].Hash}, and {ids.Length - 1} more assets) has been removed and no longer tracked by resource manager.");
        ////}
        //public static void RemoveAsset(this UnityEngine.Object obj)
        //{
        //    string path = AssetDatabase.GetAssetPath(obj);
        //    Setting.RemoveAsset(in path);
        //}
        //public static void RemoveAsset(this PointProjectSettings _, in string assetPath)
        //{
        //    Setting.RemoveAsset(in assetPath);
        //}

        //public static UnityEngine.Object GetAsset(this in AssetID other)
        //{
        //    string path = other.Hash.Key;
        //    return AssetDatabase.LoadAssetAtPath(path, TypeHelper.TypeOf<UnityEngine.Object>.Type);
        //}

        //public static bool HasAssetBundle(this in AssetID other)
        //{
        //    // https://docs.unity3d.com/ScriptReference/AssetDatabase.GetImplicitAssetBundleName.html
        //    string bundleName = AssetDatabase.GetImplicitAssetBundleName(other.Hash.Key);
        //    return !string.IsNullOrEmpty(bundleName);
        //}
        ////public static UnityEngine.AssetBundle GetAssetBundle(this in AssetID other)
        ////{
        ////    string name = AssetDatabase.GetImplicitAssetBundleName(other.Hash.Key);
        ////    return 
        ////}

        //#endregion

        //public static void UpdateAssetBundleID(this ResourceAddresses other, params string[] assetBundleNames)
        //{
        //    FieldInfo field = TypeHelper.TypeOf<ResourceAddresses>.GetFieldInfo("m_TrackedAssetBundles");

        //    TrackedBundle[] bundles = new TrackedBundle[assetBundleNames.Length];
        //    for (int i = 0; i < bundles.Length; i++)
        //    {
        //        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleNames[i]);
        //        bundles[i] = new TrackedBundle(assetBundleNames[i], assetPaths);
        //    }

        //    field.SetValue(other, bundles);
        //    EditorUtility.SetDirty(other);
        //}
    }
}

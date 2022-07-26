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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [InitializeOnLoad]
    public static class AssetHelper
    {
        public static T LoadEditorAsset<T>(string path)
            where T : UnityEngine.Object
        {
            T obj = EditorGUIUtility.Load(path) as T;

            return obj;
        }
        public static T LoadAsset<T>(string name, string label) where T : UnityEngine.Object
        {
            const string c_Format = "{0} l:{1} t:{2}";
            string context = string.Format(c_Format, name, label, TypeHelper.TypeOf<T>.Name);
            var assets = AssetDatabase.FindAssets(context);
            if (assets.Length == 0) return null;

            string guid = assets[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        public static T LoadAsset<T>(string name, params string[] labels) where T : UnityEngine.Object
        {
            const string
                c_Format = "{0} {1} t:{2}",
                c_LabelFormat = "l:{0}";
            string labelContext = string.Empty;
            for (int i = 0; i < labels.Length; i++)
            {
                if (!labelContext.IsNullOrEmpty())
                {
                    labelContext = labelContext.AddSpace();
                }
                labelContext += string.Format(c_LabelFormat, labels[i]);
            }

            string context = string.Format(c_Format, name,
                labelContext,
                TypeHelper.TypeOf<T>.Name);

            var assets = AssetDatabase.FindAssets(context)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>);
            if (!assets.Any()) return null;

            //string guid = assets[0];
            //string path = AssetDatabase.GUIDToAssetPath(guid);
            //return AssetDatabase.LoadAssetAtPath<T>(path);

            return assets.Where(t => t.name.Equals(name)).FirstOrDefault();
        }
        public static T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            var assets = AssetDatabase.FindAssets($"{name} t:{TypeHelper.TypeOf<T>.Name}");
            if (assets.Length == 0) return null;

            string guid = assets[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public static T AddSubAssetAt<T>(UnityEngine.Object target, string subAssetName)
            where T : ScriptableObject
        {
            if (AssetDatabase.GetAssetPath(target).IsNullOrEmpty()) return null;

            T t = ScriptableObject.CreateInstance<T>();
            t.name = subAssetName;
            AssetDatabase.AddObjectToAsset(t, target);

            EditorUtility.SetDirty(target);
            return t;
        }
        public static T AddSubAssetAt<T>(string targetAssetPath, string subAssetName)
            where T : ScriptableObject
        {
            if (targetAssetPath.IsNullOrEmpty()) return null;

            T t = ScriptableObject.CreateInstance<T>();
            t.name = subAssetName;
            AssetDatabase.AddObjectToAsset(t, targetAssetPath);

            //EditorUtility.SetDirty(target);
            return t;
        }

        public static string GetAssetPath(UnityEngine.Object obj)
        {
            string assetPath;
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                if (PrefabUtility.IsPartOfPrefabInstance(obj))
                {
                    assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                }
                else assetPath = AssetDatabase.GetAssetPath(obj);
            }
            else assetPath = AssetDatabase.GetAssetPath(obj);

            return assetPath;
        }
        public static GameObject GetPrefabAsset(UnityEngine.Object obj)
        {
            string assetPath;
            if (obj != null && PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                if (PrefabUtility.IsPartOfPrefabInstance(obj))
                {
                    assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                }
                else assetPath = AssetDatabase.GetAssetPath(obj);
            }
            else return null;

            return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        }
        public static string GetSelectionFolderPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path.IsNullOrEmpty())
            {
                path = "Assets/";
            }
            else if (!AssetDatabase.IsValidFolder(path))
            {
                path = path.Replace(Path.GetFileName(path), string.Empty);
            }

            return path;
        }

        public static TObject CreateAssetAtPath<TObject>(string path, string name, TObject templete = null)
            where TObject : ScriptableObject
        {
            if (templete == null)
            {
                templete = UnityEngine.ScriptableObject.CreateInstance<TObject>();
            }

            string targetPath = Path.Combine(path, $"{name}.asset");

            AssetDatabase.CreateAsset(templete, targetPath);
            AssetDatabase.ImportAsset(targetPath);
            AssetDatabase.Refresh();

            TObject result = AssetDatabase.LoadAssetAtPath<TObject>(targetPath);
            Selection.activeObject = result;

            return result;
        }
        public static GameObject CreatePrefabAtPath(string path, string name, GameObject templete, bool prefabVariant)
        {
            if (prefabVariant)
            {
                templete = (GameObject)PrefabUtility.InstantiatePrefab(templete);
            }

            string targetPath = Path.Combine(path, $"{name}.prefab");

            GameObject result = PrefabUtility.SaveAsPrefabAsset(templete, targetPath);
            AssetDatabase.ImportAsset(targetPath);
            AssetDatabase.Refresh();

            Selection.activeObject = result;

            return result;
        }

        public static bool HasLabel<TObject>(this TObject t, string label)
            where TObject : UnityEngine.Object
        {
            string[] labels = AssetDatabase.GetLabels(t);
            return label.Contains(label);
        }
        public static void AddLabel<TObject>(this TObject t, params string[] labels)
            where TObject : UnityEngine.Object
        {
            List<string> target = AssetDatabase.GetLabels(t).ToList();
            target.AddRange(labels);
            AssetDatabase.SetLabels(t, target.ToArray());
        }
        public static string[] GetLabels<TObject>(this TObject t)
            where TObject : UnityEngine.Object
        {
            return AssetDatabase.GetLabels(t);
        }
    }
}

#endif
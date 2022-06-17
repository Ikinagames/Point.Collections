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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    // https://github.com/ogxd/project-curator

    [InitializeOnLoad]
    public static class AssetHelper
    {
        private const string
            c_CachedDataPath = "ProjectSettings/AssetHelperCachedData.json";

        // https://docs.unity3d.com/ScriptReference/AssetModificationProcessor.html
        private sealed class AssetHelperAssetProcessor : AssetPostprocessor
        {
            public static void OnPostprocessAllAssets(
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                if (!UtilityMenu.EnableAssetInspector) return;

                foreach (string importedAsset in importedAssets)
                {
                    AddAssetDatabase(importedAsset);
                }
                foreach (string deletedAsset in deletedAssets)
                {
                    RemoveAssetDatabase(deletedAsset);
                }
            }
        }
        //[Serializable]
        //private sealed class CachedData
        //{
        //    [SerializeField]
        //    private AssetInfo[] m_AssetInfos;
        //}

        private static readonly Dictionary<string, AssetInfo> s_AssetDatabase = new Dictionary<string, AssetInfo>();
        private static Texture2D
            s_LinkBlack, s_LinkWhite, s_LinkBlue;
        private static GUIContent
            s_DisplayReferencesContent = new GUIContent(
                "Display References", "이 에셋을 참조하는 에셋입니다."),
            s_DisplayDependenciesContent = new GUIContent(
                "Display Dependencies", "이 에셋이 참조하는 에셋입니다.");

        private static bool 
            s_AssetDatabaseBuilded = false,
            s_DisplayAssetInspector = false,
            s_DisplayReferences, s_DisplayDependencies;

        static AssetHelper()
        {
            LoadResources();
            
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }
        private static void OnPostHeaderGUI(UnityEditor.Editor obj)
        {
            if (!UtilityMenu.EnableAssetInspector) return;

            bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(obj.target);

            if (obj.target is AssetImporter || obj.target is MonoScript)
            {
                return;
            }
            // 프리팹 오브젝트이지만, 루트가 아닌 자식 오브젝트일 경우 무시
            else if (isPrefab && !PrefabUtility.IsAnyPrefabInstanceRoot(obj.target as GameObject))
            {
                return;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                s_DisplayAssetInspector = 
                    CoreGUI.LabelToggle(s_DisplayAssetInspector, "Asset Inspector", 15, TextAnchor.MiddleCenter);

                using (new EditorGUI.DisabledGroupScope(s_AssetDatabaseBuilded))
                {
                    if (GUILayout.Button("Build", GUILayout.Width(45)))
                    {
                        RebuildAssetDatabase();
                    }
                }
            }

            if (!s_AssetDatabaseBuilded || !s_DisplayAssetInspector)
            {
                CoreGUI.Line();
                return;
            }

            string assetPath;
            if (isPrefab)
            {
                assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj.target);
            }
            else assetPath = AssetDatabase.GetAssetPath(obj.target);

            if (assetPath.IsNullOrEmpty() || 
                !s_AssetDatabase.TryGetValue(assetPath, out AssetInfo info))
            {
                return;
            }

            //EditorGUILayout.Space(2);

            //CoreGUI.Label("Asset Inspector", 20, TextAnchor.MiddleCenter);
            //EditorGUILayout.Space(5);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(AssetDatabase.GetCachedIcon(assetPath), GUILayout.Width(36), GUILayout.Height(36));
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Label(Path.GetFileName(assetPath));
                    // Display directory (without "Assets/" prefix)
                    GUILayout.Label(Regex.Match(Path.GetDirectoryName(assetPath), "(\\\\.*)$").Value);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    s_DisplayReferences = EditorGUILayout.ToggleLeft(s_DisplayReferencesContent, s_DisplayReferences);

                    if (s_DisplayReferences)
                    {
                        foreach (var referencer in info.References)
                        {
                            EditorGUILayout.ObjectField(
                                GUIContent.none,
                                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(referencer),
                                TypeHelper.TypeOf<UnityEngine.Object>.Type, true);
                        }
                    }
                }
                using (new EditorGUILayout.VerticalScope())
                {
                    s_DisplayDependencies = EditorGUILayout.ToggleLeft(s_DisplayDependenciesContent, s_DisplayDependencies);

                    if (s_DisplayDependencies)
                    {
                        foreach (var dependency in info.Dependencies)
                        {
                            EditorGUILayout.ObjectField(
                                GUIContent.none,
                                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(dependency),
                                TypeHelper.TypeOf<UnityEngine.Object>.Type, true);
                        }
                    }
                }
            }
            
            CoreGUI.Line();
        }
        private static void LoadResources()
        {
            s_LinkBlack = LoadAsset<Texture2D>("link_block", "PointEditor");
            s_LinkWhite = LoadAsset<Texture2D>("link_white", "PointEditor");
            s_LinkBlue = LoadAsset<Texture2D>("link_blue", "PointEditor");
        }
        private static void RebuildAssetDatabase()
        {
            const string c_Header = "Building Dependency Database";

            PointHelper.Log(Channel.Editor, c_Header);

            s_AssetDatabase.Clear();

#if UNITY_2020_1_OR_NEWER
            int id = Progress.Start(c_Header, "Gathering All Assets...", Progress.Options.None);
#endif
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < allAssetPaths.Length; i++)
            {
                AssetInfo info = new AssetInfo(allAssetPaths[i]);
                s_AssetDatabase.Add(allAssetPaths[i], info);

#if UNITY_2020_1_OR_NEWER
                Progress.Report(id, i / allAssetPaths.Length);
#endif
            }

#if UNITY_2020_1_OR_NEWER
            int subId = Progress.Start(c_Header, "Authoring All Assets...", parentId: id);
#endif

            for (int i = 0; i < allAssetPaths.Length; i++)
            {
                AssetInfo info = s_AssetDatabase[allAssetPaths[i]];
                info.BuildReferenceSet(s_AssetDatabase);

#if UNITY_2020_1_OR_NEWER
                Progress.Report(subId, i / allAssetPaths.Length);
#endif
                //bool cancel = EditorUtility.DisplayCancelableProgressBar(c_Header, "Authoring All Assets...", i / allAssetPaths.Length);
                //if (cancel) break;
            }
#if UNITY_2020_1_OR_NEWER
            Progress.Remove(subId);
            Progress.Remove(id);
#endif
            //EditorUtility.ClearProgressBar();
            s_AssetDatabaseBuilded = true;
        }
        private static void AddAssetDatabase(string path)
        {
            AssetInfo info = new AssetInfo(path);
            s_AssetDatabase[path] = info;

            info.BuildReferenceSet(s_AssetDatabase);
        }
        private static void RemoveAssetDatabase(string path)
        {
            if (!s_AssetDatabase.ContainsKey(path)) return;

            AssetInfo info = s_AssetDatabase[path];
            info.RemoveReferenceSet(s_AssetDatabase);

            s_AssetDatabase.Remove(path);
        }

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

            var assets = AssetDatabase.FindAssets(context);
            if (assets.Length == 0) return null;

            string guid = assets[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(path);
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
    }

    public enum BuildStatus
    {
        Unknown         =   0,

        Includable        =   0b0001,
        NotIncludable     =   0b0010,

        Referenced      =   0b00010000,
    }

    [Serializable]
    public sealed class AssetInfo : ISerializationCallbackReceiver
    {
        [NonSerialized]
        private HashSet<string>
            // 내가 참조하는 모든 에셋의 경로들
            m_ReferenceSet = new HashSet<string>(),
            // 나를 참조하는 모든 에셋의 경로들
            m_DependencySet = new HashSet<string>();
        [NonSerialized]
        private BuildStatus m_BuildStatus = BuildStatus.Unknown;

        [SerializeField]
        private AssetPathField m_Asset = new AssetPathField(string.Empty);
        [SerializeField]
        private string[]
            m_References = Array.Empty<string>(),
            m_Dependencies = Array.Empty<string>();

        public AssetPathField Asset => m_Asset;
        public HashSet<string> References => m_ReferenceSet;
        public HashSet<string> Dependencies => m_DependencySet;

        public BuildStatus BuildStatus
        {
            get
            {
                if (m_BuildStatus == BuildStatus.Unknown)
                {
                    if (m_Asset.IsEmpty()) return BuildStatus.Unknown;

                    if (m_Asset.IsInEditorFolder())
                    {
                        m_BuildStatus = BuildStatus.NotIncludable;
                    }
                    else
                    {
                        m_BuildStatus = BuildStatus.Includable;
                    }

                    if (m_ReferenceSet.Count > 0 || m_DependencySet.Count > 0)
                    {
                        m_BuildStatus |= BuildStatus.Referenced;
                    }
                }

                return m_BuildStatus;
            }
        }

        public AssetInfo(string assetPath)
        {
            m_Asset = new AssetPathField(assetPath);
            
            string[] dependencies = m_Asset.GetDependencies();
            m_DependencySet = new HashSet<string>(dependencies.Where(t => !t.Equals(assetPath)).ToArray());

            //if (m_Asset.EditorAsset is MonoScript script)
            //{
            //}
        }
        internal void BuildReferenceSet(Dictionary<string, AssetInfo> assetDatabase)
        {
            foreach (var item in m_DependencySet)
            {
                if (!assetDatabase.TryGetValue(item, out AssetInfo dep))
                {
                    continue;
                }

                dep.m_ReferenceSet.Add(m_Asset.AssetPath);
            }
        }
        internal void RemoveReferenceSet(Dictionary<string, AssetInfo> assetDatabase)
        {
            foreach (var item in m_DependencySet)
            {
                if (!assetDatabase.TryGetValue(item, out AssetInfo dep))
                {
                    continue;
                }

                dep.m_ReferenceSet.Remove(m_Asset.AssetPath);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_References = m_ReferenceSet.ToArray();
            m_Dependencies = m_DependencySet.ToArray();
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_ReferenceSet = new HashSet<string>(m_References);
            m_DependencySet = new HashSet<string>(m_Dependencies);
        }
    }
}

#endif
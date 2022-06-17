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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE


using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [InitializeOnLoad]
    public static class AssetInspector
    {
        private static Texture2D
            s_LinkBlack, s_LinkWhite, s_LinkBlue;
        private static GUIContent
            s_DisplayReferencesContent = new GUIContent(
                "Display References", "이 에셋을 참조하는 에셋입니다."),
            s_DisplayDependenciesContent = new GUIContent(
                "Display Dependencies", "이 에셋이 참조하는 에셋입니다.");

        private static bool
            s_DisplayAssetInspector = false,
            s_DisplayReferences, s_DisplayDependencies;
        private static BackgroundTask
            s_Cor;

        static AssetInspector()
        {
            LoadResources();

            UnityEditor.Editor.finishedDefaultHeaderGUI -= OnPostHeaderGUI;
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }
        // https://docs.unity3d.com/ScriptReference/AssetModificationProcessor.html
        private sealed class AssetProcessor : AssetPostprocessor
        {
            public static void OnPostprocessAllAssets(
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                //if (!UtilityMenu.EnableAssetInspector) return;

                foreach (string importedAsset in importedAssets)
                {
                    AssetInspectorDatabase.Add(importedAsset);
                }
                foreach (string deletedAsset in deletedAssets)
                {
                    AssetInspectorDatabase.Remove(deletedAsset);
                }
            }
        }

        private static void LoadResources()
        {
            s_LinkBlack = AssetHelper.LoadAsset<Texture2D>("link_block", "PointEditor");
            s_LinkWhite = AssetHelper.LoadAsset<Texture2D>("link_white", "PointEditor");
            s_LinkBlue = AssetHelper.LoadAsset<Texture2D>("link_blue", "PointEditor");
        }
        private static void OnPostHeaderGUI(UnityEditor.Editor editor)
        {
            if (!UtilityMenu.EnableAssetInspector) return;

            if (editor.target is AssetImporter || editor.target is MonoScript)
            {
                return;
            }

            // 프리팹 오브젝트이지만, 루트가 아닌 자식 오브젝트일 경우 무시
            if (editor.target is GameObject targetGameObject &&
                PrefabUtility.IsPartOfAnyPrefab(editor.target))
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(editor.target) &&
                    !PrefabUtility.IsPartOfPrefabInstance(editor.target))
                {
                    EditorGUILayout.LabelField("not root");
                    return;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                s_DisplayAssetInspector =
                    CoreGUI.LabelToggle(s_DisplayAssetInspector, "Asset Inspector", 13, TextAnchor.MiddleCenter);

                using (new EditorGUI.DisabledGroupScope(s_Cor != null && s_Cor.IsRunning))
                {
                    if (GUILayout.Button("Build", GUILayout.Width(45)))
                    {
                        s_Cor = AssetInspectorDatabase.Build();

                        return;
                    }
                }
            }

            if (s_Cor != null && s_Cor.IsRunning)
            {
                CoreGUI.Label("Assetdatabase is now building ..", 11, TextAnchor.MiddleCenter);
            }

            if (!AssetInspectorDatabase.Builded) return;

            string assetPath = GetAssetPath(editor.target);
            if (assetPath.IsNullOrEmpty())
            {
                EditorGUILayout.LabelField("asset path null");
                return;
            }

            var assetInfo = AssetInspectorDatabase.Instance[assetPath];
            if (assetInfo == null)
            {
                EditorGUILayout.LabelField("database not found");
                return;
            }

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
                        foreach (var referencer in assetInfo.References)
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
                        foreach (var dependency in assetInfo.Dependencies)
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

        private static string GetAssetPath(UnityEngine.Object obj)
        {
            string assetPath;
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                if (PrefabUtility.IsPartOfPrefabInstance(obj))
                {
                    assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                    //if (obj == null) "?".ToLogError();
                }
                else assetPath = AssetDatabase.GetAssetPath(obj);

                //assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
            }
            else assetPath = AssetDatabase.GetAssetPath(obj);

            return assetPath;
        }
    }
}

#endif
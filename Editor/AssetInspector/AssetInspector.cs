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
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [InitializeOnLoad]
    public static class AssetInspector
    {
        // // https://github.com/ogxd/project-curator

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

        private static Vector2
            m_ReferenceScroll, m_DependenciesScroll;
        private static SearchField
            m_ReferenceSearch = new SearchField(),
            m_DependenciesSearch = new SearchField();
        private static string
            m_ReferenceSearchText = string.Empty, m_DependenciesSearchText = string.Empty;

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

            string assetPath = AssetHelper.GetAssetPath(editor.target);
            if (assetPath.IsNullOrEmpty())
            {
                return;
            }

            using (new CoreGUI.BoxBlock(Color.black, 
                GUILayout.MaxWidth(Screen.width)))
            {
                if (!DrawSetup())
                {
                    return;
                }
                if (!AssetInspectorDatabase.Builded) return;

                DrawItems(editor, assetPath);
            }
        }

        private static bool DrawSetup()
        {
            string headerText;
            if (s_Cor != null && s_Cor.IsRunning)
            {
#if UNITY_2020_1_OR_NEWER
                headerText = $"Assetdatabase is now building .. {s_Cor.Percent}";
#else
                headerText = $"Assetdatabase is now building ..";
#endif
            }
            else if (!AssetInspectorDatabase.Builded)
            {
                headerText = "Asset Inspector (Require Build)";
            }
            else
            {
                return true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                CoreGUI.Label(headerText, 11, TextAnchor.MiddleCenter);
                
                using (new EditorGUI.DisabledGroupScope(s_Cor != null && s_Cor.IsRunning))
                {
                    if (GUILayout.Button(
                        AssetInspectorDatabase.Builded ? "Rebuild" : "Build",
                        GUILayout.Width(80)))
                    {
                        s_Cor = AssetInspectorDatabase.Build();
                    }
                }
            }

            return false;
        }
        private static void DrawItems(UnityEditor.Editor editor, string assetPath)
        {
            #region Top

            if (assetPath.IsNullOrEmpty())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    CoreGUI.Label("!! Asset path null !!", 13, TextAnchor.MiddleCenter);

                    if (GUILayout.Button("Rebuild", GUILayout.Width(80)))
                    {
                        s_Cor = AssetInspectorDatabase.Build();
                    }
                }

                return;
            }

            var assetInfo = AssetInspectorDatabase.Instance[assetPath];
            if (assetInfo == null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    CoreGUI.Label("!! Database not found !!", 13, TextAnchor.MiddleCenter);
                    
                    if (GUILayout.Button("Rebuild", GUILayout.Width(80)))
                    {
                        s_Cor = AssetInspectorDatabase.Build();
                    }
                }
                
                return;
            }

            #endregion

            const float c_IconWidth = 36;
            const float c_ButtonWidth = 80;
            float middleTextWidth = Screen.width - c_IconWidth - c_ButtonWidth - 20;

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(AssetDatabase.GetCachedIcon(assetPath), 
                        GUILayout.Width(36), GUILayout.Height(36));
                    using (new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label(
                            Path.GetFileName(assetPath), 
                            GUILayout.Width(middleTextWidth));
                        // Display directory (without "Assets/" prefix)
                        GUILayout.Label(
                            Regex.Match(Path.GetDirectoryName(assetPath), "(\\\\.*)$").Value,
                            GUILayout.Width(middleTextWidth));
                    }
                    using (new EditorGUILayout.VerticalScope(GUILayout.Width(80)))
                    {
                        if (GUILayout.Button(s_DisplayAssetInspector ? "Close" : "Open"))
                        {
                            s_DisplayAssetInspector = !s_DisplayAssetInspector;
                        }

                        using (new EditorGUI.DisabledGroupScope(s_Cor != null && s_Cor.IsRunning))
                        {
                            if (GUILayout.Button("Rebuild"))
                            {
                                s_Cor = AssetInspectorDatabase.Build();
                            }
                        }
                    }
                }

                if (!s_DisplayAssetInspector) return;

                float toogleWidth = Screen.width * .5f - 30;

                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(Screen.width - 20)))
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new CoreGUI.BoxBlock(Color.gray))
                        {
                            s_DisplayReferences = EditorGUILayout.ToggleLeft(
                                s_DisplayReferencesContent, s_DisplayReferences,
                                GUILayout.Width(toogleWidth));
                            Rect lastRect = GUILayoutUtility.GetLastRect();

                            if (s_DisplayReferences)
                            {
                                Rect searchRect = GUILayoutUtility.GetRect(lastRect.width, 30);
                                m_ReferenceSearchText = m_ReferenceSearch.OnGUI(
                                    searchRect, m_ReferenceSearchText);

                                using (var scroll = new EditorGUILayout.ScrollViewScope(
                                    m_ReferenceScroll, false, true,
                                    GUILayout.Height(300)))
                                using (new EditorGUI.DisabledGroupScope(true))
                                {
                                    foreach (var referencer in assetInfo.References)
                                    {
                                        if (!m_ReferenceSearchText.IsNullOrEmpty())
                                        {
                                            try
                                            {
                                                if (!Regex.Match(referencer, m_ReferenceSearchText).Success)
                                                {
                                                    continue;
                                                }
                                            }
                                            catch (System.Exception)
                                            {
                                                continue;
                                            }
                                        }

                                        EditorGUILayout.ObjectField(
                                            GUIContent.none,
                                            AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(referencer),
                                            TypeHelper.TypeOf<UnityEngine.Object>.Type, true);
                                    }

                                    m_ReferenceScroll = scroll.scrollPosition;
                                }
                            }
                        }
                    }
                    //
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new CoreGUI.BoxBlock(Color.gray))
                        {
                            s_DisplayDependencies = EditorGUILayout.ToggleLeft(
                                s_DisplayDependenciesContent, s_DisplayDependencies,
                                GUILayout.Width(toogleWidth));
                            Rect lastRect = GUILayoutUtility.GetLastRect();

                            if (s_DisplayDependencies)
                            {
                                Rect searchRect = GUILayoutUtility.GetRect(lastRect.width, 30);
                                m_DependenciesSearchText = m_DependenciesSearch.OnGUI(
                                     searchRect, m_DependenciesSearchText);

                                using (var scroll = new EditorGUILayout.ScrollViewScope(
                                    m_DependenciesScroll, false, true,
                                    GUILayout.Height(300)))
                                using (new EditorGUI.DisabledGroupScope(true))
                                {
                                    foreach (var dependency in assetInfo.Dependencies)
                                    {
                                        if (!m_DependenciesSearchText.IsNullOrEmpty())
                                        {
                                            try
                                            {
                                                if (!Regex.Match(dependency, m_DependenciesSearchText).Success)
                                                {
                                                    continue;
                                                }
                                            }
                                            catch (System.Exception)
                                            {
                                                continue;
                                            }
                                        }

                                        EditorGUILayout.ObjectField(
                                            GUIContent.none,
                                            AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(dependency),
                                            TypeHelper.TypeOf<UnityEngine.Object>.Type, true);
                                    }

                                    m_DependenciesScroll = scroll.scrollPosition;
                                }
                            }
                        }
                    }
                    //
                }
                //
            }
        }
    }
}

#endif
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AssetBundlePatching;

namespace Point.Collections.ResourceControl.Editor
{
    [DisplayName("Asset Addresses")]
    internal sealed class AssetAddressesSetting : PointStaticSettingBase<AssetAddressesSetting>
    {
        [Flags]
        public enum AssetImportHandles
        {
            None = 0,

            Audio = 0x00001,

            All = ~0
        }
        [Serializable]
        private sealed class PlatformDependsPath
        {
            [SerializeField] private string m_Path = "AssetBundles/";
            [SerializeField] private BuildTarget m_BuildTarget = BuildTarget.StandaloneWindows64;
            [SerializeField] private bool m_Enable = false;

            public string Path
            {
                get => m_Path;
                set => m_Path = value;
            }
            public BuildTarget Target
            {
                get => m_BuildTarget;
                set => m_BuildTarget = value;
            }
            public bool Enable
            {
                get => m_Enable;
                set => m_Enable = value;
            }

            public PlatformDependsPath(BuildTarget target)
            {
                m_BuildTarget = target;
            }
            public PlatformDependsPath(string path, BuildTarget target, bool enable = false)
            {
                m_Path = path;
                m_BuildTarget = target;
            }
        }

        [Serializable]
        private sealed class AssetBundleOptions
        {
            [SerializeField] private bool m_CleanUpBeforeBuild = true;
            [SerializeField] private bool m_CopyToStreamingFolderAfterBuild = true;
            [SerializeField] private string[] m_TrackedAssetBundleNames = Array.Empty<string>();
            [SerializeField] private bool m_UseAssetDatabase = true;

            public bool CleanUpBeforeBuild
            {
                get => m_CleanUpBeforeBuild;
                set => m_CleanUpBeforeBuild = value;
            }
            public bool CopyToStreamingFolderAfterBuild
            {
                get => m_CopyToStreamingFolderAfterBuild;
                set => m_CopyToStreamingFolderAfterBuild = value;
            }
            public string[] TrackedAssetBundleNames
            {
                get => m_TrackedAssetBundleNames;
                set => m_TrackedAssetBundleNames = value;
            }
            public bool UseAssetDatabase
            {
                get => m_UseAssetDatabase;
                set => m_UseAssetDatabase = value;
            }
        }

        [SerializeField] private AssetImportHandles m_AssetImportHandles = AssetImportHandles.None;
        [SerializeField] private AssetStrategy m_Strategy = AssetStrategy.AssetBundle;
        [SerializeField] private string[] m_TrackedAssets = Array.Empty<string>();

        [SerializeField]
        private PlatformDependsPath[] m_PlatformDependsPaths = new PlatformDependsPath[]
        {
            new PlatformDependsPath("AssetBundles/iOS/", BuildTarget.iOS),
            new PlatformDependsPath("AssetBundles/Android/", BuildTarget.Android),
            new PlatformDependsPath("AssetBundles/x86/", BuildTarget.StandaloneWindows),
            new PlatformDependsPath("AssetBundles/x64/", BuildTarget.StandaloneWindows64, true),
        };
        [SerializeField] private AssetBundleOptions m_AssetBundleOptions = new AssetBundleOptions();

        [NonSerialized] private BuildTarget m_InspectedPlatformDepends = BuildTarget.StandaloneWindows64;

        public AssetImportHandles ImportHandles => m_AssetImportHandles;
        public AssetStrategy Strategy => m_Strategy;
        public IReadOnlyList<string> AssetIDs => m_TrackedAssets;

        private static class GUIStyleContents
        {
            public static GUIContent
                AssetStrategy = new GUIContent(
                    "Asset Strategy",
                    "AssetBundle 과 Addressable 중, 주 리소스 관리자를 선택합니다."
                    ),
                AssetImportHandles = new GUIContent(
                    "Auto Asset Import",
                    "해당 타입의 에셋이 Import 되었을 때, 자동으로 ResourceAddresses 에서 해당 에셋을 관리합니다.")

                ;
        }

        protected override void OnInitialize()
        {

        }
        protected override void OnSettingGUI(string searchContext)
        {
            using (new EditorUtilities.BoxBlock(Color.black))
            {
                EditorUtilities.StringRich("Generals", 13);
                EditorUtilities.Line();
                EditorGUI.indentLevel++;

                EditorGUILayout.HelpBox(GUIStyleContents.AssetStrategy.tooltip, MessageType.Info);
                m_Strategy
                    = (AssetStrategy)EditorGUILayout.EnumPopup(GUIStyleContents.AssetStrategy, m_Strategy);
                EditorGUILayout.HelpBox(GUIStyleContents.AssetImportHandles.tooltip, MessageType.Info);
                m_AssetImportHandles
                    = (AssetImportHandles)EditorGUILayout.EnumFlagsField(GUIStyleContents.AssetImportHandles, m_AssetImportHandles);

                EditorGUILayout.Space();

                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.IntField("Tracked Assets", m_TrackedAssets.Length);
                }

                EditorGUI.indentLevel--;
            }

            if (m_Strategy == AssetStrategy.AssetBundle)
            {
                DrawAssetBundleGUI();
            }
        }
        private void DrawAssetBundleGUI()
        {
            using (new EditorUtilities.BoxBlock(Color.black))
            {
                EditorUtilities.StringRich("AssetBundle", 13);
                EditorUtilities.Line();
                EditorGUI.indentLevel++;

                m_AssetBundleOptions.CleanUpBeforeBuild
                    = EditorGUILayout.ToggleLeft("Cleanup destination folder before build", m_AssetBundleOptions.CleanUpBeforeBuild);

                m_AssetBundleOptions.CopyToStreamingFolderAfterBuild
                    = EditorGUILayout.ToggleLeft("Copy to StreamingAssets after build", m_AssetBundleOptions.CopyToStreamingFolderAfterBuild);

                using (new EditorUtilities.BoxBlock(Color.blue))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Tracked AssetBundles");

                        using (new EditorGUI.DisabledGroupScope(m_AssetBundleOptions.TrackedAssetBundleNames.Length == 0))
                        {
                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                string[] newArr = new string[m_AssetBundleOptions.TrackedAssetBundleNames.Length - 1];
                                Array.Copy(m_AssetBundleOptions.TrackedAssetBundleNames, newArr, newArr.Length);

                                m_AssetBundleOptions.TrackedAssetBundleNames = newArr;
                            }
                        }
                    }

                    EditorGUI.indentLevel++;

                    List<string> temp = AssetDatabase.GetAllAssetBundleNames().ToList();
                    temp.Insert(0, "None");
                    string[] allBundleNames = temp.ToArray();

                    for (int i = 0; i < m_AssetBundleOptions.TrackedAssetBundleNames.Length; i++)
                    {
                        string name = m_AssetBundleOptions.TrackedAssetBundleNames[i];
                        int index = temp.IndexOf(name);

                        // AssetBundle 를 지웠거나 이름이 바뀐 경우
                        if (index < 0)
                        {
                            var list = m_AssetBundleOptions.TrackedAssetBundleNames.ToList();
                            list.RemoveAt(i);
                            m_AssetBundleOptions.TrackedAssetBundleNames = list.ToArray();

                            i--;
                            continue;
                        }

                        using (var change = new EditorGUI.ChangeCheckScope())
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            index = EditorGUILayout.Popup(index, allBundleNames);

                            if (GUILayout.Button("-", GUILayout.Width(20)))
                            {
                                var list = m_AssetBundleOptions.TrackedAssetBundleNames.ToList();
                                list.RemoveAt(i);
                                i--;
                                m_AssetBundleOptions.TrackedAssetBundleNames = list.ToArray();

                                continue;
                            }

                            if (change.changed)
                            {
                                if (index == 0)
                                {
                                    var list = m_AssetBundleOptions.TrackedAssetBundleNames.ToList();
                                    list.RemoveAt(i);
                                    i--;
                                    m_AssetBundleOptions.TrackedAssetBundleNames = list.ToArray();

                                    continue;
                                }
                                else if (m_AssetBundleOptions.TrackedAssetBundleNames.Contains(allBundleNames[index]))
                                {
                                    PointCore.LogError(LogChannel.Editor,
                                        $"Target AssetBundle({allBundleNames[index]}) already registered.");
                                }
                                else
                                {
                                    m_AssetBundleOptions.TrackedAssetBundleNames[i] = allBundleNames[index];
                                }
                            }
                        }
                    }

                    using (var change = new EditorGUI.ChangeCheckScope())
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        int newIndex = 0;
                        newIndex = EditorGUILayout.Popup(newIndex, allBundleNames);

                        if (change.changed && newIndex != 0)
                        {
                            string[] newArr = new string[m_AssetBundleOptions.TrackedAssetBundleNames.Length + 1];
                            Array.Copy(m_AssetBundleOptions.TrackedAssetBundleNames, newArr, m_AssetBundleOptions.TrackedAssetBundleNames.Length);

                            newArr[newArr.Length - 1] = allBundleNames[newIndex];

                            m_AssetBundleOptions.TrackedAssetBundleNames = newArr;
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorUtilities.Line();

                using (new EditorUtilities.BoxBlock(Color.red))
                {
                    m_InspectedPlatformDepends
                        = (BuildTarget)EditorGUILayout.EnumPopup("Target Build", m_InspectedPlatformDepends);

                    PlatformDependsPath path = GetPlatformDependsPath(m_InspectedPlatformDepends);
                    EditorGUI.indentLevel++;

                    path.Enable = EditorGUILayout.ToggleLeft("Enable Build", path.Enable);
                    path.Path = EditorGUILayout.TextField("Path", path.Path);

                    EditorGUI.indentLevel--;
                }

                if (GUILayout.Button("Build"))
                {
                    for (int i = 0; i < m_PlatformDependsPaths.Length; i++)
                    {
                        if (!m_PlatformDependsPaths[i].Enable) continue;

                        BuildAssetBundles(
                            m_PlatformDependsPaths[i].Path,
                            BuildAssetBundleOptions.None,
                            m_PlatformDependsPaths[i].Target,
                            m_AssetBundleOptions.CleanUpBeforeBuild,
                            m_AssetBundleOptions.CopyToStreamingFolderAfterBuild);
                    }
                }
                EditorGUI.indentLevel--;
            }

            PlatformDependsPath GetPlatformDependsPath(BuildTarget buildTarget)
            {
                for (int i = 0; i < m_PlatformDependsPaths.Length; i++)
                {
                    if (m_PlatformDependsPaths[i].Target.Equals(buildTarget))
                    {
                        return m_PlatformDependsPaths[i];
                    }
                }

                PlatformDependsPath[] newArr = new PlatformDependsPath[m_PlatformDependsPaths.Length + 1];
                Array.Copy(m_PlatformDependsPaths, newArr, m_PlatformDependsPaths.Length);
                m_PlatformDependsPaths = newArr;

                m_PlatformDependsPaths[m_PlatformDependsPaths.Length - 1]
                    = new PlatformDependsPath(buildTarget);

                return m_PlatformDependsPaths[m_PlatformDependsPaths.Length - 1];
            }
        }

        private static AssetBundleManifest BuildAssetBundles(
            string path,
            BuildAssetBundleOptions bundleOptions,
            BuildTarget buildTarget,
            bool cleanUp,
            bool copyToStreaming)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                if (cleanUp) EditorUtilities.DeleteFilesRecursively(path);
            }
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
                path,
                bundleOptions,
                buildTarget);
            if (manifest == null)
            {
                PointCore.LogError(LogChannel.Editor,
                    $"There is nothing to build.");

                return null;
            }

            if (copyToStreaming && !path.Equals(Application.streamingAssetsPath))
            {
                EditorUtilities.CopyFilesRecursively(path, Application.streamingAssetsPath + Path.DirectorySeparatorChar);
            }

            return manifest;
        }


        private void asd()
        {
            AssetDatabase.GetAllAssetBundleNames();

        }
    }
}

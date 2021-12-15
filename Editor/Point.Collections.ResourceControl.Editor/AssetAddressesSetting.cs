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
        public enum AssetStrategy
        {
            AssetBundle,
            Addressable
        }

        [Serializable]
        private sealed class AssetBundleOptions
        {
            [SerializeField] private string m_AssetBundlePath = "AssetBundles/";
            [SerializeField] private bool m_CopyToStreamingFolderAfterBuild = true;

            public string AssetBundlePath
            {
                get => m_AssetBundlePath;
                set => m_AssetBundlePath = value;
            }
            public bool CopyToStreamingFolderAfterBuild
            {
                get => m_CopyToStreamingFolderAfterBuild;
                set => m_CopyToStreamingFolderAfterBuild = value;
            }
        }

        [SerializeField] private AssetImportHandles m_AssetImportHandles = AssetImportHandles.None;
        [SerializeField] private AssetStrategy m_Strategy = AssetStrategy.AssetBundle;
        [SerializeField] private AssetID[] m_AssetIDs = Array.Empty<AssetID>();

        [SerializeField] private AssetBundleOptions m_AssetBundleOptions = new AssetBundleOptions();

        [NonSerialized] private Dictionary<AssetID, int> m_RegisteredAssets;

        public AssetImportHandles ImportHandles => m_AssetImportHandles;
        public AssetStrategy Strategy => m_Strategy;
        public IReadOnlyList<AssetID> AssetIDs => m_AssetIDs;

        private static class GUIStyleContents
        {
            public static GUIContent
                AssetImportHandles = new GUIContent(
                    "Auto Asset Import", 
                    "해당 타입의 에셋이 등록되었을 때, 자동으로 ResourceAddresses 에서 해당 에셋을 관리합니다."),
                AssetStrategy = new GUIContent(
                    "Asset Strategy",
                    ""
                    )
                ;
        }

        protected override void OnInitialize()
        {
            HashAssets();
        }
        protected override void OnSettingGUI(string searchContext)
        {
            using (new EditorUtilities.BoxBlock(Color.black))
            {
                EditorUtilities.StringRich("Generals", 13);
                EditorUtilities.Line();
                EditorGUI.indentLevel++;

                m_Strategy
                    = (AssetStrategy)EditorGUILayout.EnumPopup(GUIStyleContents.AssetStrategy, m_Strategy);
                m_AssetImportHandles
                    = (AssetImportHandles)EditorGUILayout.EnumFlagsField(GUIStyleContents.AssetImportHandles, m_AssetImportHandles);

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

                m_AssetBundleOptions.CopyToStreamingFolderAfterBuild
                    = EditorGUILayout.ToggleLeft("Copy to StreamingAssets after build", m_AssetBundleOptions.CopyToStreamingFolderAfterBuild);
                m_AssetBundleOptions.AssetBundlePath
                    = EditorGUILayout.TextField("Path", m_AssetBundleOptions.AssetBundlePath);

                if (GUILayout.Button("Build"))
                {
                    BuildAssetBundles(
                        m_AssetBundleOptions.AssetBundlePath, 
                        BuildAssetBundleOptions.None, 
                        BuildTarget.StandaloneWindows,
                        m_AssetBundleOptions.CopyToStreamingFolderAfterBuild);
                }
                EditorGUI.indentLevel--;
            }
        }

        private void HashAssets()
        {
            m_RegisteredAssets = new Dictionary<AssetID, int>();
            for (int i = 0; i < m_AssetIDs.Length; i++)
            {
                m_RegisteredAssets.Add(m_AssetIDs[i], i);
            }
        }

        public bool IsTrackedAsset(in string assetPath) => IsTrackedAsset(new AssetID(new Hash(assetPath)));
        public bool IsTrackedAsset(in AssetID id) => m_RegisteredAssets.ContainsKey(id);

        public AssetID RegisterAsset(in string assetPath)
        {
            AssetID id = new AssetID(new Hash(assetPath));
            if (IsTrackedAsset(id))
            {
                throw new Exception();
            }

            AssetID[] newArr = new AssetID[m_AssetIDs.Length];
            Array.Copy(m_AssetIDs, newArr, m_AssetIDs.Length);

            newArr[newArr.Length - 1] = id;
            m_AssetIDs = newArr;

            m_RegisteredAssets.Add(id, newArr.Length - 1);

            return id;
        }
        public AssetID UpdateAsset(in string prevAssetPath, in string targetAssetPath)
        {
            AssetID
                prev = new AssetID(new Hash(prevAssetPath)),
                target = new AssetID(new Hash(targetAssetPath));

            if (!IsTrackedAsset(prev))
            {
                throw new System.Exception("1");
            }
            else if (IsTrackedAsset(target))
            {
                throw new System.Exception("2");
            }

            int index = m_RegisteredAssets[prev];
            m_RegisteredAssets.Remove(prev);
            
            m_AssetIDs[index] = target;
            m_RegisteredAssets.Add(target, index);

            return target;
        }
        public void RemoveAsset(in string assetPath)
        {
            AssetID id = new AssetID(new Hash(assetPath));
            if (!m_RegisteredAssets.ContainsKey(id))
            {
                throw new System.Exception();
            }

            List<AssetID> temp = m_AssetIDs.ToList();
            int index = m_RegisteredAssets[id];

            temp.RemoveAt(index);
            m_AssetIDs = temp.ToArray();

            HashAssets();
        }

        private static AssetBundleManifest BuildAssetBundles(
            string path, 
            BuildAssetBundleOptions bundleOptions, 
            BuildTarget buildTarget,
            bool copyToStreaming)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
                path,
                bundleOptions, 
                buildTarget);
            
            if (copyToStreaming && !path.Equals(Application.streamingAssetsPath))
            {
                string[] bundleNames = manifest.GetAllAssetBundles();
                string[] allFiles = Directory.GetFiles(path);

                for (int i = 0; i < allFiles.Length; i++)
                {
                    string fileName = Path.GetFileNameWithoutExtension(allFiles[i]);
                    if (!bundleNames.Contains(fileName)) continue;

                    fileName = Path.GetFileName(allFiles[i]);
                    string dest = Path.Combine(Application.streamingAssetsPath, fileName);
                    if (File.Exists(dest))
                    {
                        if (File.GetLastAccessTimeUtc(dest).Equals(File.GetLastAccessTimeUtc(allFiles[i])))
                        {
                            continue;
                        }

                        File.Delete(dest);
                    }

                    File.Copy(allFiles[i], dest);
                }
            }

            return manifest;
        }

        private void asd()
        {
            AssetDatabase.GetAllAssetBundleNames();
            
        }
    }
}

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


using Point.Collections.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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

        static AssetInspector()
        {
            LoadResources();

            UnityEditor.Editor.finishedDefaultHeaderGUI -= OnPostHeaderGUI;
            UnityEditor.Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
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
                PrefabUtility.IsPartOfAnyPrefab(editor.target) && 
                !PrefabUtility.IsAnyPrefabInstanceRoot(targetGameObject))
            {
                return;
            }

            string assetPath = GetAssetPath(editor.target);
            if (assetPath.IsNullOrEmpty()) return;
        }

        private static string GetAssetPath(UnityEngine.Object obj)
        {
            string assetPath;
            if (PrefabUtility.IsPartOfAnyPrefab(obj))
            {
                assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
            }
            else assetPath = AssetDatabase.GetAssetPath(obj);

            return assetPath;
        }
    }

    [PreferBinarySerialization]
    internal sealed class AssetInspectorDatabase : EditorStaticScriptableObject<AssetInspectorDatabase>
    {
        [SerializeField]
        private AssetInfo[] m_Assets = Array.Empty<AssetInfo>();

        private AtomicSafeBoolen m_Builded = false;
        private AtomicOperator m_Op = new AtomicOperator();
        private Dictionary<string, AssetInfo> m_Database = new Dictionary<string, AssetInfo>();

        public AssetInfo this[string key]
        {
            get
            {
                AssetInfo result;
                m_Op.Enter();
                {
                    result = m_Database[key];
                }
                m_Op.Exit();

                return result;
            }
        }
        public bool Builded => m_Builded;

        private async Task<Dictionary<string, AssetInfo>> BuildDatabase()
        {
            m_Op.Enter();
            for (int i = 0; i < m_Assets.Length; i++)
            {
                m_Database.Add(m_Assets[i].Asset.AssetPath, m_Assets[i]);
            }
            m_Op.Exit();

            m_Builded.Value = true;
            return m_Database;
        }

        public static Task<Dictionary<string, AssetInfo>> Build()
        {
            return Task.Run(Instance.BuildDatabase);
        }
    }
}

#endif
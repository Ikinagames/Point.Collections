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

#if UNITY_2019_1_OR_NEWER && UNITY_ADDRESSABLES
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using Point.Collections.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.ResourceControl.Editor
{
    [CustomEditor(typeof(ResourceHashMap))]
    internal sealed class ResourceHashMapEditor : InspectorEditor<ResourceHashMap>
    {
        private SerializedProperty m_ResourceLists;

        private void OnEnable()
        {
            m_ResourceLists = serializedObject.FindProperty("m_ResourceLists");
        }
        protected override void OnInspectorGUIContents()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (CoreGUI.BoxButton("+", Color.gray))
                {
                    int index = m_ResourceLists.arraySize;
                    
                    ResourceList list = CreateInstance<ResourceList>();
                    list.name = "ResourceList " + index;
                    AssetDatabase.AddObjectToAsset(list, assetPath);
                    
                    m_ResourceLists.InsertArrayElementAtIndex(index);
                    m_ResourceLists.GetArrayElementAtIndex(index).objectReferenceValue = list;
                }
                if (CoreGUI.BoxButton("-", Color.gray))
                {
                    int index = m_ResourceLists.arraySize - 1;
                    ResourceList list = m_ResourceLists.GetArrayElementAtIndex(index).objectReferenceValue as ResourceList;
                    m_ResourceLists.DeleteArrayElementAtIndex(index);

                    AssetDatabase.RemoveObjectFromAsset(list);
                }
            }

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUIContents();
        }
    }

    [CustomEditor(typeof(ResourceList))]
    internal sealed class ResourceListEditor : InspectorEditor<ResourceList>
    {
        private SerializedProperty m_CatalogProperty, m_CatalogNameProperty;
        private SerializedProperty m_AssetListProperty;
        private List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();

        private bool m_RequireRebuild = false;

        private void OnEnable()
        {
            m_CatalogProperty = serializedObject.FindProperty("m_Catalog");
            m_CatalogNameProperty = CatalogReferencePropertyDrawer.Helper.GetCatalogName(m_CatalogProperty);
            m_AssetListProperty = serializedObject.FindProperty("m_AssetList");

            Validate();
        }
        private void Validate()
        {
            entries.Clear();

            AddressableAssetGroup addressableAssetGroup = GetGroup(m_CatalogNameProperty);
            if (addressableAssetGroup == null)
            {
                m_RequireRebuild = false;
                return;
            }

            addressableAssetGroup.GatherAllAssets(entries, false, true, true);

            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (target.Contains(entries[i].guid))
                {
                    entries.RemoveAt(i);
                    continue;
                }
            }

            if (entries.Count > 0)
            {
                m_RequireRebuild = true;
            }
        }

        protected override void OnInspectorGUIContents()
        {
            bool catalogChanged = false;
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(m_CatalogProperty);
                catalogChanged = changed.changed;

                if (catalogChanged) Validate();
            }

            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                target.name = EditorGUILayout.DelayedTextField("Name", target.name);

                if (changed.changed)
                {
                    EditorUtility.SetDirty(target);
                    EditorUtility.SetDirty(ResourceHashMap.Instance);
                    AssetDatabase.ImportAsset(
                        AssetDatabase.GetAssetPath(ResourceHashMap.Instance), 
                        ImportAssetOptions.ForceUpdate);
                }
            }

            if (m_RequireRebuild)
            {
                if (GUILayout.Button("Require Rebuild"))
                {
                    m_AssetListProperty.ClearArray();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        target.AddAsset(string.Empty, entries[i].TargetAsset);
                    }

                    EditorUtility.SetDirty(target);
                    m_RequireRebuild = false;
                }
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_AssetListProperty);

            serializedObject.ApplyModifiedProperties();
            //base.OnInspectorGUIContents();
        }

        private static AddressableAssetGroup GetGroup(SerializedProperty catalogNameProperty)
        {
            var catalogName = SerializedPropertyHelper.ReadFixedString128Bytes(catalogNameProperty);
            if (catalogName.IsEmpty) return null;

            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            return settings.FindGroup(catalogName.ToString());
        }
    }
}

#endif
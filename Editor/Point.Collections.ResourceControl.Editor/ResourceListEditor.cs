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
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.ResourceControl.Editor
{
    [CustomEditor(typeof(ResourceList))]
    //internal sealed class ResourceListEditor : InspectorEditor<ResourceList>
    internal sealed class ResourceListEditor : UnityEditor.Editor
    {
        private new ResourceList target => base.target as ResourceList;

        VisualTreeAsset VisualTreeAsset { get; set; }

        private SerializedProperty m_GroupProperty, m_GroupNameProperty;
        private SerializedProperty m_AssetListProperty;

        private bool m_IsBindedToCatalog = false;
        private bool m_RequireRebuild = false;

        private void OnEnable()
        {
            m_GroupProperty = serializedObject.FindProperty("m_Group");
            m_GroupNameProperty = GroupReferencePropertyDrawer.Helper.GetCatalogName(m_GroupProperty);
            m_AssetListProperty = serializedObject.FindProperty("m_AssetList");

            Validate();

            VisualTreeAsset = AssetHelper.LoadAsset<VisualTreeAsset>("Uxml ResourceList", "PointEditor");
        }
        private void Validate()
        {
            AddressableAssetGroup addressableAssetGroup = GetGroup(m_GroupNameProperty);
            if (addressableAssetGroup == null)
            {
                m_IsBindedToCatalog = false;
                m_RequireRebuild = false;

                return;
            }

            m_IsBindedToCatalog = true;
            if (!Validate(target))
            {
                m_RequireRebuild = true;
                return;
            }
            m_RequireRebuild = false;
        }
        public static void Rebuild(ResourceList list)
        {
            AddressableAssetGroup addressableAssetGroup = GetGroup(list.Group);
            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();
            addressableAssetGroup.GatherAllAssets(entries, true, true, true);

            list.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                list.AddAsset(string.Empty, entries[i].TargetAsset);
            }
            EditorUtility.SetDirty(list);
        }
        private void Rebuild()
        {
            AddressableAssetGroup addressableAssetGroup = GetGroup(m_GroupNameProperty);
            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();
            addressableAssetGroup.GatherAllAssets(entries, true, true, true);

            m_AssetListProperty.ClearArray();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            for (int i = 0; i < entries.Count; i++)
            {
                target.AddAsset(string.Empty, entries[i].TargetAsset);
            }

            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var tree = VisualTreeAsset.CloneTree();
            tree.Bind(serializedObject);

            IMGUIContainer objectName = tree.Q<IMGUIContainer>("ObjectName");
            objectName.onGUIHandler += ObjectNameFieldGUI;

            IMGUIContainer iMGUIContainer = tree.Q<IMGUIContainer>("AssetLists");
            iMGUIContainer.onGUIHandler += GUI;

            //ScrollView assetList = tree.Q<ScrollView>("AssetList");
            //for (int i = 0; i < assetListProp.arraySize; i++)
            //{
            //    PropertyField propertyField = new PropertyField(
            //        assetListProp.GetArrayElementAtIndex(i));
            //    assetList.Add(propertyField);
            //}

            return tree;
        }

        private void ObjectNameFieldGUI()
        {
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
        }

        private void GUI()
        {
            if (!m_IsBindedToCatalog)
            {
                EditorGUILayout.PropertyField(m_AssetListProperty);
                return;
            }

            var groupName = SerializedPropertyHelper.ReadFixedString128Bytes(m_GroupNameProperty);
            CoreGUI.Label($"Binded to {groupName}", 15, TextAnchor.MiddleCenter);

            using (new CoreGUI.BoxBlock(Color.gray))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    m_AssetListProperty.isExpanded =
                        CoreGUI.LabelToggle(m_AssetListProperty.isExpanded, m_AssetListProperty.displayName, 13, TextAnchor.MiddleLeft);
                    //CoreGUI.Label(m_AssetListProperty.displayName, 13, TextAnchor.MiddleLeft);

                    if (GUILayout.Button("Rebuild", GUILayout.Width(100)))
                    {
                        Rebuild();
                        m_RequireRebuild = false;
                    }
                }

                if (!m_AssetListProperty.isExpanded) return;

                CoreGUI.Line();

                using (new EditorGUI.IndentLevelScope())
                {
                    for (int i = 0; i < m_AssetListProperty.arraySize; i++)
                    {
                        var prop = m_AssetListProperty.GetArrayElementAtIndex(i);
                        string displayName;
                        {
                            var refAsset = target.GetAddressableAsset(i);

                            if (refAsset.EditorAsset != null)
                            {
                                displayName = refAsset.FriendlyName.IsNullOrEmpty() ?
                                    refAsset.EditorAsset.name : refAsset.FriendlyName;

                                displayName += $" ({AssetDatabase.GetAssetPath(refAsset.EditorAsset)})";
                            }
                            else displayName = prop.displayName;
                        }

                        prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, displayName, true);
                        if (!prop.isExpanded) continue;

                        using (new EditorGUI.IndentLevelScope())
                        {
                            prop.Next(true);
                            EditorGUILayout.PropertyField(prop);
                            prop.Next(false);
                            using (new EditorGUI.DisabledGroupScope(true))
                            {
                                EditorGUILayout.PropertyField(prop);
                            }
                        }
                        //
                    }
                }
            }
        }

        //protected override void OnInspectorGUIContents()
        //{
        //    bool catalogChanged = false;
        //    using (var changed = new EditorGUI.ChangeCheckScope())
        //    {
        //        EditorGUILayout.PropertyField(m_GroupProperty);
        //        catalogChanged = changed.changed;

        //        if (catalogChanged) Validate();
        //    }

        //    using (var changed = new EditorGUI.ChangeCheckScope())
        //    {
        //        target.name = EditorGUILayout.DelayedTextField("Name", target.name);

        //        if (changed.changed)
        //        {
        //            EditorUtility.SetDirty(target);
        //            EditorUtility.SetDirty(ResourceHashMap.Instance);
        //            AssetDatabase.ImportAsset(
        //                AssetDatabase.GetAssetPath(ResourceHashMap.Instance), 
        //                ImportAssetOptions.ForceUpdate);
        //        }
        //    }

        //    if (m_RequireRebuild)
        //    {
        //        EditorGUILayout.Space();
        //        if (GUILayout.Button("!! Require Rebuild !!"))
        //        {
        //             Rebuild();
        //             m_RequireRebuild = false;
        //        }
        //    }

        //    EditorGUILayout.Space();
        //    if (m_IsBindedToCatalog)
        //    {
        //        var groupName = SerializedPropertyHelper.ReadFixedString128Bytes(m_GroupNameProperty);
        //        CoreGUI.Label($"Binded to {groupName}", 15, TextAnchor.MiddleCenter);

        //        using (new CoreGUI.BoxBlock(Color.gray))
        //        {
        //            using (new EditorGUILayout.HorizontalScope())
        //            {
        //                CoreGUI.Label(m_AssetListProperty.displayName, 13, TextAnchor.MiddleLeft);

        //                if (GUILayout.Button("Rebuild", GUILayout.Width(100)))
        //                {
        //                    Rebuild();
        //                    m_RequireRebuild = false;
        //                }
        //            }

        //            CoreGUI.Line();

        //            using (new EditorGUI.IndentLevelScope())
        //            {
        //                for (int i = 0; i < m_AssetListProperty.arraySize; i++)
        //                {
        //                    var prop = m_AssetListProperty.GetArrayElementAtIndex(i);
        //                    string displayName;
        //                    {
        //                        var refAsset = target.GetAddressableAsset(i);

        //                        if (refAsset.EditorAsset != null)
        //                        {
        //                            displayName = refAsset.FriendlyName.IsNullOrEmpty() ?
        //                                refAsset.EditorAsset.name : refAsset.FriendlyName;

        //                            displayName += $" ({AssetDatabase.GetAssetPath(refAsset.EditorAsset)})";
        //                        }
        //                        else displayName = prop.displayName;
        //                    }

        //                    prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, displayName, true);
        //                    if (!prop.isExpanded) continue;

        //                    using (new EditorGUI.IndentLevelScope())
        //                    {
        //                        prop.Next(true);
        //                        EditorGUILayout.PropertyField(prop);
        //                        prop.Next(false);
        //                        using (new EditorGUI.DisabledGroupScope(true))
        //                        {
        //                            EditorGUILayout.PropertyField(prop);
        //                        }
        //                    }
        //                    //
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        EditorGUILayout.PropertyField(m_AssetListProperty);
        //    }

        //    serializedObject.ApplyModifiedProperties();
        //    //base.OnInspectorGUIContents();
        //}

        #region Utils

        private static AddressableAssetGroup GetGroup(SerializedProperty groupNameProperty)
        {
            var catalogName = SerializedPropertyHelper.ReadFixedString128Bytes(groupNameProperty);
            return GetGroup(catalogName.IsEmpty ? string.Empty : catalogName.ToString());
        }
        private static AddressableAssetGroup GetGroup(string groupName)
        {
            if (groupName.IsNullOrEmpty()) return null;

            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);

            return settings.FindGroup(groupName.ToString());
        }

        public static bool Validate(ResourceList list)
        {
            AddressableAssetGroup addressableAssetGroup = GetGroup(list.Group);
            if (addressableAssetGroup == null)
            {
                return true;
            }

            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();
            addressableAssetGroup.GatherAllAssets(entries, true, true, true);

            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (!list.Contains(entries[i].guid))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}

#endif
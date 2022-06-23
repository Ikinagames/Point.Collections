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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using Point.Collections.Editor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_ADDRESSABLES
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Point.Collections.ResourceControl.Editor
{
    [CustomEditor(typeof(ResourceList))]
    internal sealed class ResourceListEditor : InspectorEditorUXML<ResourceList>
    {
        private static Regex s_PathRegex = new Regex(
            @"[p P][a A][t T][h H]:(.+)");

        private new ResourceList target => base.target as ResourceList;

        VisualTreeAsset VisualTreeAsset { get; set; }
#if UNITY_ADDRESSABLES
        bool IsBindedToCatalog
        {
            get
            {
                AddressableAssetGroup addressableAssetGroup = GetGroup(m_GroupNameProperty);
                if (addressableAssetGroup == null) return false;
                return true;
            }
        }
#endif

        private SerializedProperty m_GroupProperty;
        private SerializedProperty m_AssetListProperty;
#if UNITY_ADDRESSABLES
        private SerializedProperty m_GroupNameProperty;
#endif

        private bool m_RequireRebuild = false;

        private string m_SearchString;

        private void OnEnable()
        {
            Validate();
        }
        private void Validate()
        {
            m_GroupProperty = serializedObject.FindProperty("m_Group");
#if UNITY_ADDRESSABLES
            m_GroupNameProperty = GroupReferencePropertyDrawer.Helper.GetCatalogName(m_GroupProperty);
#endif
            m_AssetListProperty = serializedObject.FindProperty("m_AssetList");
#if UNITY_ADDRESSABLES
            AddressableAssetGroup addressableAssetGroup = GetGroup(m_GroupNameProperty);
            if (addressableAssetGroup == null)
            {
                //m_IsBindedToCatalog = false;
                m_RequireRebuild = false;

                return;
            }

            //m_IsBindedToCatalog = true;
            if (!Validate(target))
            {
                m_RequireRebuild = true;
                return;
            }
            m_RequireRebuild = false;
#endif
        }
        public static void Rebuild(ResourceList list)
        {
#if UNITY_ADDRESSABLES
            AddressableAssetGroup addressableAssetGroup = GetGroup(list.Group);
            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>();
            addressableAssetGroup.GatherAllAssets(entries, true, true, true);

            list.Clear();
            for (int i = 0; i < entries.Count; i++)
            {
                list.AddAsset(string.Empty, entries[i].TargetAsset);
            }
#endif
            EditorUtility.SetDirty(list);
        }
        private void Rebuild()
        {
#if UNITY_ADDRESSABLES
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
#endif
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        protected override VisualElement CreateVisualElement()
        {
            VisualTreeAsset = AssetHelper.LoadAsset<VisualTreeAsset>("Uxml ResourceList", "PointEditor");
            var tree = VisualTreeAsset.CloneTree();

            ToolbarSearchField toolbarSearchField = tree.Q<ToolbarSearchField>("SearchField");
            toolbarSearchField.RegisterValueChangedCallback(OnSearchFieldStringChanged);

            TextField objectName = tree.Q<TextField>("ObjectName");
            objectName.value = target.name;
            objectName.RegisterValueChangedCallback(t =>
            {
                target.name = t.newValue;

                EditorUtility.SetDirty(target);
                EditorUtility.SetDirty(ResourceHashMap.Instance);
                AssetDatabase.ImportAsset(
                    AssetDatabase.GetAssetPath(ResourceHashMap.Instance),
                    ImportAssetOptions.ForceUpdate);
            });
            PropertyField groupNameField = tree.Q<PropertyField>("GroupName");
            groupNameField.RegisterValueChangeCallback(t =>
            {
                serializedObject.ApplyModifiedProperties();
                Validate();
            });

            return tree;
        }
        protected override void SetupVisualElement(VisualElement root)
        {
            PropertyField groupField = root.Q<PropertyField>("GroupName");
            VisualElement 
                assetContainer = root.Q("AssetContainer"),
                headerContainer = assetContainer.Q("HeaderContainer"),
                contents = assetContainer.Q("Contents");
            Label headerLabel = headerContainer.Q<Label>("Label");
            Button rebuildBtt = assetContainer.Q<Button>("RebuildBtt");
            Button
                addBtt = headerContainer.Q<Button>("AddBtt"),
                removeBtt = headerContainer.Q<Button>("RemoveBtt");

            Action onGroupValueChanged = delegate
            {
#if UNITY_ADDRESSABLES
                if (IsBindedToCatalog)
                {
                    var groupName = SerializedPropertyHelper.ReadFixedString128Bytes(m_GroupNameProperty);
                    headerLabel.text = $"Binded to {groupName}";

                    rebuildBtt.style.Hide(false);
                    addBtt.SetEnabled(false);
                    removeBtt.SetEnabled(false);
                }
                else
#endif
                {
                    headerLabel.text = "Assets";

                    rebuildBtt.style.Hide(true);
                    addBtt.SetEnabled(true);
                    removeBtt.SetEnabled(true);
                }
            };
            onGroupValueChanged.Invoke();

            groupField.RegisterValueChangeCallback(t => onGroupValueChanged.Invoke());
            rebuildBtt.clicked += Rebuild;

            addBtt.clicked += delegate
            {
                int index = m_AssetListProperty.arraySize;
                m_AssetListProperty.InsertArrayElementAtIndex(index);

                var prop = m_AssetListProperty.GetArrayElementAtIndex(index);
                PropertyField field = new PropertyField(prop);
                contents.Add(field);
                field.BindProperty(prop);

                m_AssetListProperty.serializedObject.ApplyModifiedProperties();

                if (index == 0)
                {
                    removeBtt.SetEnabled(true);
                }
            };
            removeBtt.clicked += delegate
            {
                int index = m_AssetListProperty.arraySize - 1;
                m_AssetListProperty.DeleteArrayElementAtIndex(index);

                var ve = contents.ElementAt(index);
                ve.RemoveFromHierarchy();

                m_AssetListProperty.serializedObject.ApplyModifiedProperties();

                if (index == 0)
                {
                    removeBtt.SetEnabled(false);
                }
            };

            for (int i = 0; i < m_AssetListProperty.arraySize; i++)
            {
                PropertyField propertyField
                    = new PropertyField(m_AssetListProperty.GetArrayElementAtIndex(i));
                //propertyField.userData = new ElementData
                //{

                //};

                contents.Add(propertyField);
            }
        }
        private void OnSearchFieldStringChanged(ChangeEvent<string> value)
        {
            m_SearchString = value.newValue;

            VisualElement
                assetContainer = RootVisualElement.Q("AssetContainer"),
                contents = assetContainer.Q("Contents");

            if (m_SearchString.IsNullOrEmpty())
            {
                for (int i = 0; i < contents.childCount; i++)
                {
                    var element = contents.ElementAt(i) as PropertyField;
                    element.RemoveFromClassList("hide");
                }
                return;
            }

            for (int i = 0; i < contents.childCount; i++)
            {
                var element = contents.ElementAt(i) as PropertyField;
                var prop = serializedObject.FindProperty(element.bindingPath);

                prop.Next(true);
                string friendlyName = prop.stringValue;
                if (!friendlyName.ToLowerInvariant().Contains(m_SearchString.ToLowerInvariant()))
                {
                    element.AddToClassList("hide");
                }
                else
                {
                    element.RemoveFromClassList("hide");
                }
            }
        }

        private bool ElementCheck(SerializedProperty prop, string displayName)
        {
            if (m_SearchString.IsNullOrEmpty()) return true;

            var match = s_PathRegex.Match(m_SearchString);
            if (match.Success)
            {
                string pathString = match.Groups[1].Value.Trim().Split(' ')[0].ToLowerInvariant();

                prop.Next(true);
                prop.Next(false);

                var assetGUIDProp = prop.FindPropertyRelative("m_AssetGUID");
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDProp.stringValue).ToLowerInvariant();
                if (assetPath.Contains(pathString)) return true;
            }

            if (displayName.Contains(m_SearchString)) return true;

            return false;
        }

        #region Utils
#if UNITY_ADDRESSABLES

        private static AddressableAssetGroup GetGroup(SerializedProperty groupNameProperty)
        {
            var catalogName = SerializedPropertyHelper.ReadFixedChar128Bytes(groupNameProperty);
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
#endif

        #endregion
    }
}

#endif
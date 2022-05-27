﻿// Copyright 2022 Ikina Games
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
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Point.Collections.ResourceControl.Editor
{
    [CustomPropertyDrawer(typeof(AssetIndex))]
    internal sealed class AssetIndexPropertyDrawer : PropertyDrawer<AssetIndex>
    {
        private bool m_Changed = false;

        public static string NicifyDisplayName(AddressableAsset asset)
        {
            if (asset.FriendlyName.IsNullOrEmpty())
            {
                return asset.EditorAsset.name;
            }

            const string c_Format = "{0}({1})";
            return string.Format(c_Format, asset.FriendlyName, asset.EditorAsset.name);
        }

        private static class Helper
        {
            public static SerializedProperty GetIndex(SerializedProperty property)
            {
                const string c_Str = "m_Index";
                return property.FindPropertyRelative(c_Str);
            }
            public static SerializedProperty GetListIndex(SerializedProperty property)
            {
                const string c_Str = "x";

                SerializedProperty listProp = GetIndex(property);
                return listProp.FindPropertyRelative(c_Str);
            }
            public static SerializedProperty GetAssetIndex(SerializedProperty property)
            {
                const string c_Str = "y";

                SerializedProperty listProp = GetIndex(property);
                return listProp.FindPropertyRelative(c_Str);
            }
            public static SerializedProperty GetIsCreated(SerializedProperty property)
            {
                const string c_Str = "m_IsCreated";
                return property.FindPropertyRelative(c_Str);
            }
        }

        protected override float PropertyHeight(SerializedProperty property, GUIContent label)
        {
            return PropertyDrawerHelper.GetPropertyHeight(1);
        }
        protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty
                listIndexProp = Helper.GetListIndex(property),
                assetIndexProp = Helper.GetAssetIndex(property),
                isCreatedProp = Helper.GetIsCreated(property);

            AssetIndex index = new AssetIndex(listIndexProp.intValue, assetIndexProp.intValue);
            if (isCreatedProp.boolValue && !index.IsValid())
            {
                listIndexProp.intValue = 0;
                assetIndexProp.intValue = 0;
                isCreatedProp.boolValue = false;

                index = AssetIndex.Empty;
            }
            AssetReference asset = index.AssetReference;

            Rect lane = rect.Pop();
            Rect[] rects = AutoRect.DivideWithRatio(lane, .2f, .8f);

            string displayName;
            AddressableAsset refAsset = null;
            if (isCreatedProp.boolValue && asset.IsValid())
            {
                refAsset = ResourceHashMap.Instance[listIndexProp.intValue].GetAddressableAsset(assetIndexProp.intValue);
                displayName = NicifyDisplayName(refAsset);
            }
            else displayName = "Invalid";

            EditorGUI.LabelField(rects[0], label);
            bool clicked = CoreGUI.BoxButton(rects[1], displayName, Color.gray, onContextClick: () =>
            {
                GenericMenu menu = new GenericMenu();
                menu.AddDisabledItem(new GUIContent(displayName));
                menu.AddSeparator(string.Empty);

                GUIContent 
                    context1 = new GUIContent("Select", "이 에셋을 프로젝트 창에서 선택합니다.");

                if (refAsset == null)
                {
                    menu.AddDisabledItem(context1);
                }
                else
                {
                    menu.AddItem(context1, false, () =>
                    {
                        EditorGUIUtility.PingObject(refAsset.EditorAsset);
                        Selection.activeObject = refAsset.EditorAsset;
                    });
                }
            });

            if (clicked)
            {
                Vector2 pos = Event.current.mousePosition;
                pos = GUIUtility.GUIToScreenPoint(pos);

                var provider = ScriptableObject.CreateInstance<AssetIndexSearchProvider>();
                provider.m_OnClick = index =>
                {
                    if (index.x < 0)
                    {
                        index = 0;
                        isCreatedProp.boolValue = false;
                    }
                    else
                    {
                        isCreatedProp.boolValue = true;
                    }
                    listIndexProp.intValue = index.x;
                    assetIndexProp.intValue = index.y;
                    property.serializedObject.ApplyModifiedProperties();

                    m_Changed = true;
                };

                SearchWindow.Open(new SearchWindowContext(pos), provider);
            }

            if (m_Changed)
            {
                GUI.changed = true;
                m_Changed = false;
            }
        }
        private sealed class AssetIndexSearchProvider : SearchProviderBase
        {
            public Action<int2> m_OnClick;

            public AssetIndexSearchProvider(Action<int2> onClick)
            {
                m_OnClick = onClick;
            }
            public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                List<SearchTreeEntry> list = new List<SearchTreeEntry>();
                list.Add(new SearchTreeGroupEntry(new GUIContent("Assets")));
                list.Add(new SearchTreeEntry(new GUIContent("None", CoreGUI.EmptyIcon))
                {
                    userData = new int2(-1),
                    level = 1,
                });

                IReadOnlyList<ResourceList> resourceLists = ResourceHashMap.Instance.ResourceLists;
                for (int i = 0; i < resourceLists.Count; i++)
                {
                    ResourceList resourceList = resourceLists[i];
                    SearchTreeGroupEntry listGroup = new SearchTreeGroupEntry(new GUIContent(resourceList.name), 1);
                    list.Add(listGroup);
                    for (int j = 0; j < resourceList.Count; j++)
                    {
                        AddressableAsset asset = resourceList.GetAddressableAsset(j);
                        string displayName = AssetIndexPropertyDrawer.NicifyDisplayName(asset);

                        SearchTreeEntry entry = new SearchTreeEntry(
                            new GUIContent(displayName, CoreGUI.EmptyIcon))
                        {
                            userData = new int2(i, j),
                            level = 2
                        };

                        list.Add(entry);
                    }
                }

                return list;
            }
            public override bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
            {
                if (SearchTreeEntry is SearchTreeGroupEntry) return true;

                m_OnClick?.Invoke((int2)SearchTreeEntry.userData);
                return true;
            }
        }
    }

    [CustomPropertyDrawer(typeof(GroupReference))]
    internal sealed class CatalogReferencePropertyDrawer : PropertyDrawer<GroupReference>
    {
        public static class Helper
        {
            public static SerializedProperty GetCatalogName(SerializedProperty property)
            {
                const string c_Str = "m_Name";
                return property.FindPropertyRelative(c_Str);
            }
        }

        private bool m_Changed = false;

        protected override float PropertyHeight(SerializedProperty property, GUIContent label)
        {
            return PropertyDrawerHelper.GetPropertyHeight(1);
        }

        protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty
                catalogNameProp = Helper.GetCatalogName(property);

            Rect lane = rect.Pop();
            Rect[] rects = AutoRect.DivideWithRatio(lane, .2f, .8f);
            EditorGUI.LabelField(rects[0], label);

            string currentNameValue = SerializedPropertyHelper.ReadFixedString128Bytes(catalogNameProp).ToString();
            string displayName = currentNameValue.IsNullOrEmpty() ? "Invalid" : currentNameValue;
            bool clicked = CoreGUI.BoxButton(rects[1], displayName, Color.gray);

            if (clicked)
            {
                Vector2 pos = Event.current.mousePosition;
                pos = GUIUtility.GUIToScreenPoint(pos);

                var provider = ScriptableObject.CreateInstance<CatalogSearchProvider>();
                provider.m_OnClick = str =>
                {
                    SerializedPropertyHelper.SetFixedString128Bytes(catalogNameProp, str);
                    property.serializedObject.ApplyModifiedProperties();

                    m_Changed = true;
                };
                SearchWindow.Open(new SearchWindowContext(pos), provider);
            }

            if (m_Changed)
            {
                GUI.changed = true;
                m_Changed = false;
            }
        }

        private sealed class CatalogSearchProvider : SearchProviderBase
        {
            public Action<string> m_OnClick;

            public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                const string c_BuiltInData = "Built In Data";

                List<SearchTreeEntry> list = new List<SearchTreeEntry>();
                list.Add(new SearchTreeGroupEntry(new GUIContent("Catalogs")));
                list.Add(new SearchTreeEntry(new GUIContent("None", CoreGUI.EmptyIcon))
                {
                    userData = string.Empty,
                    level = 1,
                });

                var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                var groupNames = settings.groups.Select(t => t.Name);
                foreach (string groupName in groupNames)
                {
                    if (groupName.Equals(c_BuiltInData)) continue;

                    SearchTreeEntry entry = new SearchTreeEntry(
                        new GUIContent(groupName, CoreGUI.EmptyIcon))
                    {
                        level = 1,
                        userData = groupName
                    };
                    list.Add(entry);
                }

                return list;
            }
            public override bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
            {
                if (SearchTreeEntry is SearchTreeGroupEntry) return true;

                m_OnClick?.Invoke((string)SearchTreeEntry.userData);
                return true;
            }
        }
    }
}

#endif
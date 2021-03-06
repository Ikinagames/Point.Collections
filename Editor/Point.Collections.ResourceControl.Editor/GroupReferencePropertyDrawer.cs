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

#if UNITY_2020_1_OR_NEWER && UNITY_ADDRESSABLES
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using Point.Collections.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Point.Collections.ResourceControl.Editor
{
    [CustomPropertyDrawer(typeof(GroupReference))]
    internal sealed class GroupReferencePropertyDrawer : PropertyDrawer<GroupReference>
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

            string currentNameValue = SerializedPropertyHelper.ReadFixedChar128Bytes(catalogNameProp).ToString();
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

            protected override string DisplayName => "Catalogs";

            protected override void BuildSearchTree(in SearchWindowContext ctx, in List<SearchTreeEntry> list)
            {
                const string c_BuiltInData = "Built In Data";

                list.Add(new SearchTreeEntry(new GUIContent("None", CoreGUI.EmptyIcon))
                {
                    userData = string.Empty,
                    level = 1,
                });

                var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
                foreach (var group in settings.groups)
                {
                    if (group.Name.Equals(c_BuiltInData)) continue;

                    SearchTreeEntry entry = new SearchTreeEntry(
                        new GUIContent(group.Name, CoreGUI.EmptyIcon))
                    {
                        level = 1,
                        userData = group.Name
                    };
                    list.Add(entry);
                }
            }
            protected override bool OnSelectEntry(in SearchWindowContext ctx, in SearchTreeEntry entry)
            {
                if (entry is SearchTreeGroupEntry) return true;

                m_OnClick?.Invoke((string)entry.userData);
                return true;
            }
        }
    }
}

#endif
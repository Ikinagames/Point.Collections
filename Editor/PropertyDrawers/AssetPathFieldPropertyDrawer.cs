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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(AssetPathField), true)]
    internal sealed class AssetPathFieldPropertyDrawer : AssetPathFieldPropertyDrawerFallback
    {
        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            AssetPathFieldView ve = new AssetPathFieldView(property);
            if (!property.IsInArray() || property.GetParent().ChildCount() > 1)
            {
                ve.label = property.displayName;
            }

            return ve;
        }
    }
    internal abstract class AssetPathFieldPropertyDrawerFallback : PropertyDrawerUXML<AssetPathField>
    {
        private static UnityEngine.Object GetObjectAtPath(in string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        }
        private static UnityEngine.Object LoadAsset(SerializedProperty property)
        {
            string path = property.FindPropertyRelative("p_AssetPath").stringValue;
            return GetObjectAtPath(path);
        }
        private static void ApplyProperty(SerializedProperty property, UnityEngine.Object obj)
        {
            if (obj == null)
            {
                property.FindPropertyRelative("p_AssetGUID").stringValue = string.Empty;
                property.FindPropertyRelative("p_AssetPath").stringValue = string.Empty;
                property.FindPropertyRelative("p_SubAssetName").stringValue = string.Empty;
                return;
            }

            string path = AssetDatabase.GetAssetPath(obj);
            property.FindPropertyRelative("p_AssetPath").stringValue = path;
            property.FindPropertyRelative("p_AssetGUID").stringValue = AssetDatabase.AssetPathToGUID(path);

            if (AssetDatabase.IsSubAsset(obj))
            {
                property.FindPropertyRelative("p_SubAssetName").stringValue = obj.name;
            }
            else
            {
                property.FindPropertyRelative("p_SubAssetName").stringValue = string.Empty;
            }
        }

        private Type GetFieldType()
        {
            if (fieldInfo.FieldType.GetGenericArguments().Length == 0)
            {
                return TypeHelper.TypeOf<UnityEngine.Object>.Type;
            }

            return fieldInfo.FieldType.GetGenericArguments()[0];
        }
        

        protected override void OnPropertyGUI(ref AutoRect autoRect, SerializedProperty property, GUIContent label)
        {
            Type targetType = GetFieldType();

            Rect rect = autoRect.Pop();

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                UnityEngine.Object obj = EditorGUI.ObjectField(rect, property.displayName, LoadAsset(property), targetType, false);

                if (change.changed)
                {
                    ApplyProperty(property, obj);
                }
            }
        }
    }
}

#endif
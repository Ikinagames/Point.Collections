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
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(AssetPathField), true)]
    public sealed class AssetPathFieldPropertyDrawer : PropertyDrawer<AssetPathField>
    {
        private const string c_AssetPathField = "p_AssetPath";
        private static Type s_GenericType = typeof(AssetPathField<>);

        protected override float PropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CoreGUI.GetLineHeight(1);
        }
        protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            SerializedProperty pathProperty = property.FindPropertyRelative(c_AssetPathField);

            string assetPath = pathProperty.stringValue;
            UnityEngine.Object asset = GetObjectAtPath(in assetPath);

            Type targetType;
            if (TypeHelper.InheritsFrom(fieldInfo.FieldType, s_GenericType))
            {
                if (fieldInfo.FieldType.IsGenericType && 
                    s_GenericType.Equals(fieldInfo.FieldType.GetGenericTypeDefinition()))
                {
                    targetType = fieldInfo.FieldType.GenericTypeArguments[0];
                }
                else
                {
                    Type genericDef = TypeHelper.GetGenericBaseType(fieldInfo.FieldType, s_GenericType);
                    targetType = genericDef.GenericTypeArguments[0];
                }
            }
            else
            {
                targetType = TypeHelper.TypeOf<UnityEngine.Object>.Type;
            }

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                Rect pos = rect.Pop();
                UnityEngine.Object obj 
                    = EditorGUI.ObjectField(pos, label, asset, targetType, false);

                if (changeCheck.changed)
                {
                    pathProperty.stringValue
                        = AssetDatabase.GetAssetPath(obj);
                }
            }
        }

        private static UnityEngine.Object GetObjectAtPath(in string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        }
    }
}

#endif
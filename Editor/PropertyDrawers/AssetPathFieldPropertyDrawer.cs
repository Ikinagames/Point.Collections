﻿// Copyright 2021 Ikina Games
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

using System;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(AssetPathField), true)]
    public sealed class AssetPathFieldPropertyDrawer : PropertyDrawer
    {
        private const string c_AssetPathField = "p_AssetPath";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                SerializedProperty pathProperty = property.FindPropertyRelative(c_AssetPathField);

                string assetPath = pathProperty.stringValue;
                UnityEngine.Object asset = GetObjectAtPath(in assetPath);

                Type targetType;
                if (TypeHelper.TypeOf<AssetPathField>.Type.IsAssignableFrom(fieldInfo.FieldType) &&
                    fieldInfo.FieldType.BaseType.GenericTypeArguments.Length > 0)
                {
                    targetType = fieldInfo.FieldType.BaseType.GenericTypeArguments[0];
                }
                else targetType = TypeHelper.TypeOf<UnityEngine.Object>.Type;

                using (var changeCheck = new EditorGUI.ChangeCheckScope())
                {
                    UnityEngine.Object obj = EditorGUI.ObjectField(position, label, asset, targetType, false);

                    if (changeCheck.changed)
                    {
                        pathProperty.stringValue
                            = AssetDatabase.GetAssetPath(obj);
                    }
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
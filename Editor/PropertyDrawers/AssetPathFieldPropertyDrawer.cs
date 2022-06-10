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
    public sealed class AssetPathFieldPropertyDrawer : PropertyDrawerUXML<AssetPathField>
    {
        private const string c_AssetPathField = "p_AssetPath", c_AssetGUIDField = "p_AssetGUID";
        private static Type s_GenericType = typeof(AssetPathField<>);

        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            SerializedProperty pathProperty = property.FindPropertyRelative(c_AssetPathField);
            SerializedProperty guidProperty = property.FindPropertyRelative(c_AssetGUIDField);

            string assetPath = pathProperty.stringValue;
            UnityEngine.Object asset = GetObjectAtPath(in assetPath);

            Type fieldType;
            if (fieldInfo.FieldType.IsArray) fieldType = fieldInfo.FieldType.GetElementType();
            else fieldType = fieldInfo.FieldType;

            Type targetType;
            if (TypeHelper.InheritsFrom(fieldType, s_GenericType))
            {
                if (fieldType.IsGenericType &&
                    s_GenericType.Equals(fieldType.GetGenericTypeDefinition()))
                {
                    targetType = fieldType.GenericTypeArguments[0];
                }
                else
                {
                    Type genericDef = TypeHelper.GetGenericBaseType(fieldType, s_GenericType);
                    targetType = genericDef.GenericTypeArguments[0];
                }
            }
            else
            {
                targetType = TypeHelper.TypeOf<UnityEngine.Object>.Type;
            }

            VisualElement element = new VisualElement();
            element.styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);
            element.style.flexDirection = FlexDirection.Row;
            element.style.flexGrow = 1;

            ObjectField objectfield = new ObjectField();
            objectfield.style.flexGrow = 1;
            objectfield.objectType = targetType;
            objectfield.value = asset;
            {
                objectfield.RegisterValueChangedCallback(t =>
                {
                    pathProperty.stringValue = AssetDatabase.GetAssetPath(t.newValue);
                    guidProperty.stringValue = AssetDatabase.AssetPathToGUID(pathProperty.stringValue);

                    pathProperty.serializedObject.ApplyModifiedProperties();
                });
            }
            element.Add(objectfield);

            TextField textField = new TextField();
            textField.style.flexGrow = 1;
            textField.value = pathProperty.stringValue;
            {
                textField.RegisterValueChangedCallback(t =>
                {
                    pathProperty.stringValue = t.newValue;
                    guidProperty.stringValue = AssetDatabase.AssetPathToGUID(pathProperty.stringValue);

                    pathProperty.serializedObject.ApplyModifiedProperties();
                });
            }
            element.Add(textField);

            if (property.IsInArray() && property.GetParent().ChildCount() == 1)
            {
                //label = GUIContent.none;
                objectfield.label = String.Empty;
                textField.label = String.Empty;
            }
            else
            {
                objectfield.label = property.displayName;
                textField.label = property.displayName;
            }

            if (!property.isExpanded)
            {
                textField.AddToClassList("hide");
            }
            else
            {
                objectfield.AddToClassList("hide");
            }

            Button btt = new Button();
            btt.style.width = 60;
            btt.text = "Raw";
            {
                btt.clicked += delegate
                {
                    property.isExpanded = !property.isExpanded;
                    property.serializedObject.ApplyModifiedProperties();

                    if (!property.isExpanded)
                    {
                        textField.AddToClassList("hide");
                        objectfield.RemoveFromClassList("hide");
                    }
                    else
                    {
                        textField.RemoveFromClassList("hide");
                        objectfield.AddToClassList("hide");
                    }
                };
            }
            element.Add(btt);

            return element;
        }

        private static UnityEngine.Object GetObjectAtPath(in string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        }
    }
}

#endif
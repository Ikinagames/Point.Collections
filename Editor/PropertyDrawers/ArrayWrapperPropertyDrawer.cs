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

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(ArrayWrapper<>), true)]
    public class ArrayWrapperPropertyDrawer : PropertyDrawerUXML<Array>
    {
        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            VisualElement root = CoreGUI.VisualElement.ListContainer(property.displayName,
                out Label headerLabel,
                out Button addBtt, out Button removeBtt, out var contentContainer);

            property.Next(true);

            if (property.arraySize == 0)
            {
                contentContainer.AddToClassList("hide");
                removeBtt.SetEnabled(false);
            }
            else
            {
                if (!property.isExpanded)
                {
                    contentContainer.AddToClassList("hide");
                }
                else contentContainer.RemoveFromClassList("hide");
            }

            headerLabel.RegisterCallback<MouseDownEvent>(t =>
            {
                property.isExpanded = !property.isExpanded;

                if (property.arraySize == 0) return;

                if (!property.isExpanded)
                {
                    if (!contentContainer.ClassListContains("hide"))
                    {
                        contentContainer.AddToClassList("hide");
                    }
                }
                else contentContainer.RemoveFromClassList("hide");

                property.serializedObject.ApplyModifiedProperties();
            });

            addBtt.clicked += delegate
            {
                int index = property.arraySize;
                property.InsertArrayElementAtIndex(index);
                var prop = property.GetArrayElementAtIndex(index);

                prop.SetDefaultValue();

                PropertyField field = new PropertyField(prop);
                field.BindProperty(prop);
                contentContainer.Add(field);

                if (index == 0)
                {
                    contentContainer.RemoveFromClassList("hide");
                    removeBtt.SetEnabled(true);
                }

                property.serializedObject.ApplyModifiedProperties();
            };
            removeBtt.clicked += delegate
            {
                int index = property.arraySize - 1;

                property.DeleteArrayElementAtIndex(index);
                contentContainer.ElementAt(index).RemoveFromHierarchy();

                if (index == 0)
                {
                    contentContainer.AddToClassList("hide");
                    removeBtt.SetEnabled(false);
                }

                property.serializedObject.ApplyModifiedProperties();
            };

            for (int i = 0; i < property.arraySize; i++)
            {
                var prop = property.GetArrayElementAtIndex(i);
                PropertyField field = new PropertyField(prop);

                contentContainer.Add(field);
            }

            return root;
        }
        protected override void SetupVisualElement(SerializedProperty property, VisualElement root)
        {

        }
    }
}

#endif
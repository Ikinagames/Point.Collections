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
        private static VisualElement ElementFactory(SerializedProperty element)
        {
            if (element.ChildCount() > 1)
            {
                VisualElement ve = new VisualElement();
                ve.AddToClassList("content-container");
                ve.AddToClassList("inner-container");

                foreach (var item in element.ForEachChild())
                {
                    var childVe = CoreGUI.VisualElement.PropertyField(item);
                    ve.Add(childVe);
                }

                return ve;
            }

            return CoreGUI.VisualElement.PropertyField(element);
        }
        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            string displayName = property.displayName;
            string tooltip = property.tooltip;
            property.Next(true);
            //ListContainerView list = new ListContainerView(displayName, tooltip, property);
            ListContainerView list = new ListContainerView(displayName);
            for (int i = 0; i < property.arraySize; i++)
            {
                var elementProp = property.GetArrayElementAtIndex(i);
                list.Add(ElementFactory(elementProp));
            }
            list.isExpanded = property.isExpanded;
            list.onExpand += delegate (bool expanded)
            {
                property.isExpanded = expanded;
                property.serializedObject.ApplyModifiedProperties();
            };

            list.onAddButtonClicked += delegate (int index)
            {
                property.InsertArrayElementAtIndex(index);
                var element = property.GetArrayElementAtIndex(index);
                element.SetDefaultValue();
                property.serializedObject.ApplyModifiedProperties();

                return ElementFactory(element);
            };
            list.onRemoveButtonClicked += delegate (int index)
            {
                property.DeleteArrayElementAtIndex(index);
                property.serializedObject.ApplyModifiedProperties();
            };

            return list;
        }
    }
}

#endif
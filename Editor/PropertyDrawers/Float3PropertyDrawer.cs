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

#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(float3))]
    internal sealed class Float3PropertyDrawer : PropertyDrawerUXML<float3>
    {
        private static GUIContent[] s_ElementContents = new GUIContent[]
        {
            new GUIContent("X"),
            new GUIContent("Y"),
            new GUIContent("Z")
        };

        public static void Draw(Rect rect, SerializedProperty property, GUIContent content)
        {
            Rect[] rects = AutoRect.DivideWithRatio(rect, .25f, .25f, .25f, .25f);

            SerializedProperty
                x = property.FindPropertyRelative("x"),
                y = property.FindPropertyRelative("y"),
                z = property.FindPropertyRelative("z");

            EditorGUI.LabelField(rects[0], content);
            Rect[] elementRaws = new Rect[2];
            float[] elementRawRatios = new float[2] { .1f, .9f };

            AutoRect.DivideWithRatio(rects[1], elementRaws, elementRawRatios);
            elementRaws[0].width = rects[1].width;
            EditorGUI.LabelField(elementRaws[0], s_ElementContents[0]);
            x.floatValue = EditorGUI.FloatField(elementRaws[1], x.floatValue);

            AutoRect.DivideWithRatio(rects[2], elementRaws, elementRawRatios);
            elementRaws[0].width = rects[1].width;
            EditorGUI.LabelField(elementRaws[0], s_ElementContents[1]);
            y.floatValue = EditorGUI.FloatField(elementRaws[1], y.floatValue);

            AutoRect.DivideWithRatio(rects[3], elementRaws, elementRawRatios);
            elementRaws[0].width = rects[1].width;
            EditorGUI.LabelField(elementRaws[0], s_ElementContents[2]);
            z.floatValue = EditorGUI.FloatField(elementRaws[1], z.floatValue);
        }

        //protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        //{
        //    Rect elementRect = rect.Pop();
        //    Draw(elementRect, property, label);
        //}

        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            SerializedProperty
                x = property.FindPropertyRelative("x"),
                y = property.FindPropertyRelative("y"),
                z = property.FindPropertyRelative("z");

            Vector3Field field = new Vector3Field(property.displayName);
            field.value = new Vector3(x.floatValue, y.floatValue, z.floatValue);
            field.RegisterValueChangedCallback(t =>
            {
                var value = t.newValue;
                x.floatValue = value.x;
                y.floatValue = value.y;
                z.floatValue = value.z;

                x.serializedObject.ApplyModifiedProperties();
            });

            return field;
        }
    }
}

#endif
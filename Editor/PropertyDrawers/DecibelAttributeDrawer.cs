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
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(DecibelAttribute))]
    internal sealed class DecibelAttributeDrawer : PropertyDrawer<DecibelAttribute>
    {
        static class MinMaxFieldHelper
        {
            private const string 
                c_MinText = "m_Min", c_MaxText = "m_Max";

            public static SerializedProperty GetMin(SerializedProperty field)
            {
                return field.FindPropertyRelative(c_MinText);
            }
            public static SerializedProperty GetMax(SerializedProperty field)
            {
                return field.FindPropertyRelative(c_MaxText);
            }
        }

        const float minimum = -80, maximum = 0;

        protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo.FieldType == TypeHelper.TypeOf<float>.Type)
            {
                FloatField(ref rect, property, label);
            }
            else if (fieldInfo.FieldType == TypeHelper.TypeOf<MinMaxFloatField>.Type)
            {
                MinMaxFloatField(ref rect, property, label);
            }
        }

        private void FloatField(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            Rect pos = rect.Pop(PropertyDrawerHelper.GetPropertyHeight(1));
            float value = Math.TodB(property.floatValue);
            
            EditorGUI.BeginChangeCheck();
            value = CoreGUI.Slider(
                pos,
                label,
                value,
                minimum,
                maximum
                );

            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = Math.FromdB(value);
            }
        }
        private void MinMaxFloatField(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            Rect pos = rect.Pop(PropertyDrawerHelper.GetPropertyHeight(1));
            SerializedProperty
                minProp = MinMaxFieldHelper.GetMin(property),
                maxProp = MinMaxFieldHelper.GetMax(property);

            float
                min = Math.TodB(minProp.floatValue),
                max = Math.TodB(maxProp.floatValue);

            min = (float)System.Math.Round(min, 1);
            max = (float)System.Math.Round(max, 1);

            EditorGUI.BeginChangeCheck();
            CoreGUI.MinMaxSlider(
                pos,
                label,
                ref min,
                ref max,
                minimum,
                maximum
                );

            if (EditorGUI.EndChangeCheck())
            {
                minProp.floatValue = Math.FromdB(min);
                maxProp.floatValue = Math.FromdB(max);
            }
        }
    }
}

#endif
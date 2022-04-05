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

using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(int3))]
    internal sealed class Int3PropertyDrawer : PropertyDrawer<int3>
    {
        private static GUIContent[] m_ElementContents = new GUIContent[]
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
            EditorGUI.LabelField(elementRaws[0], m_ElementContents[0]);
            x.intValue = EditorGUI.IntField(elementRaws[1], x.intValue);

            AutoRect.DivideWithRatio(rects[2], elementRaws, elementRawRatios);
            elementRaws[0].width = rects[1].width;
            EditorGUI.LabelField(elementRaws[0], m_ElementContents[1]);
            y.intValue = EditorGUI.IntField(elementRaws[1], y.intValue);

            AutoRect.DivideWithRatio(rects[3], elementRaws, elementRawRatios);
            elementRaws[0].width = rects[1].width;
            EditorGUI.LabelField(elementRaws[0], m_ElementContents[2]);
            z.intValue = EditorGUI.IntField(elementRaws[1], z.intValue);
        }

        protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            Rect elementRect = rect.Pop();
            //Rect[] rects = AutoRect.DivideWithRatio(elementRect, .2f, .4f, .4f);
            Draw(elementRect, property, label);
        }
    }
}

#endif
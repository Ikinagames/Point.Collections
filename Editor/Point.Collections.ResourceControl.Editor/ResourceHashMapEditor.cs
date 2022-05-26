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

#if UNITY_2019_1_OR_NEWER && UNITY_ADDRESSABLES
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using Point.Collections.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.ResourceControl.Editor
{
    [CustomEditor(typeof(ResourceHashMap))]
    internal sealed class ResourceHashMapEditor : InspectorEditor<ResourceHashMap>
    {
        private SerializedProperty m_ResourceLists;

        private void OnEnable()
        {
            m_ResourceLists = serializedObject.FindProperty("m_ResourceLists");
        }
        protected override void OnInspectorGUIContents()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (CoreGUI.BoxButton("+", Color.gray))
                {
                    int index = m_ResourceLists.arraySize;

                    ResourceList list = CreateInstance<ResourceList>();
                    list.name = "ResourceList " + index;
                    AssetDatabase.AddObjectToAsset(list, assetPath);

                    m_ResourceLists.InsertArrayElementAtIndex(index);
                    m_ResourceLists.GetArrayElementAtIndex(index).objectReferenceValue = list;
                }
                if (CoreGUI.BoxButton("-", Color.gray))
                {
                    int index = m_ResourceLists.arraySize - 1;
                    ResourceList list = m_ResourceLists.GetArrayElementAtIndex(index).objectReferenceValue as ResourceList;
                    m_ResourceLists.DeleteArrayElementAtIndex(index);

                    AssetDatabase.RemoveObjectFromAsset(list);
                }
            }

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUIContents();
        }
    }
}

#endif
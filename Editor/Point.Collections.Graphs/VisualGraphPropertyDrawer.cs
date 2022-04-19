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

#if UNITY_2020_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.Graphs;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(VisualGraph), true)]
    internal sealed class VisualGraphPropertyDrawer : PropertyDrawer<VisualGraph>
    {
        protected override void BeforePropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            rect.SetLeftPadding(5);
            rect.SetUpperPadding(5);
        }
        protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            Rect block = rect.TotalRect;
            block.height -= 3;
            CoreGUI.DrawBlock(EditorGUI.IndentedRect(block), Color.black);

            property.isExpanded = LabelToggle(
                ref rect, property.isExpanded, label, 15, TextAnchor.MiddleLeft);

            if (!property.isExpanded) return;

            Space(ref rect, 3);
            EditorGUI.indentLevel++;
            {
                if (property.objectReferenceValue == null)
                {
                    PropertyField(ref rect, property, GUIContent.none);

                    if (Button(ref rect, "Create Default Graph"))
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, "Create Default Graph");

                        ScriptableObject obj
                            = VisualGraphAssetCallbacks.CreateGraphAsset(property.GetFieldInfo().FieldType, false);
                        AssetDatabase.AddObjectToAsset(obj, property.serializedObject.targetObject);

                        property.objectReferenceValue = obj;
                    }

                    if (Button(ref rect, "Create Graph"))
                    {
                        Debug.Log("not implement");
                    }
                }
                else
                {
                    bool isSubAsset = AssetDatabase.IsSubAsset(property.objectReferenceValue);
                    using (new EditorGUI.DisabledGroupScope(isSubAsset))
                    {
                        PropertyField(ref rect, property, GUIContent.none);
                    }

                    if (isSubAsset && Button(ref rect, "Remove"))
                    {
                        AssetDatabase.RemoveObjectFromAsset(property.objectReferenceValue);
                        AssetDatabase.SaveAssets();

                        property.objectReferenceValue = null;
                    }
                    if (Button(ref rect, "Open graph window"))
                    {
                        VisualGraphAssetCallbacks.OpenGraphAsset(property.objectReferenceValue as VisualGraph);

                        //EditorWindow.GetWindow<VisualGraphWindow>().InitializeGraph(property.objectReferenceValue as VisualGraph);
                    }
                }

                
            }
            EditorGUI.indentLevel--;
        }
    }
}

#endif
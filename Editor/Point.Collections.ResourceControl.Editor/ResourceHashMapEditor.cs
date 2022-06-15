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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using Point.Collections.Editor;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.ResourceControl.Editor
{
    [CustomEditor(typeof(ResourceHashMap))]
    internal sealed class ResourceHashMapEditor : InspectorEditorUXML<ResourceHashMap>
    {
        private SerializedProperty 
            m_SceneBindedLabelsProperty, m_ResourceListsProperty,
            m_StreamingAssetBundlesProperty;

        VisualTreeAsset VisualTreeAsset { get; set; }
        VisualElement ResourceListContainer { get; set; }
        protected override bool ShouldHideOpenButton() => true;

        private void OnEnable()
        {
            m_SceneBindedLabelsProperty = serializedObject.FindProperty("m_SceneBindedLabels");
            m_ResourceListsProperty = serializedObject.FindProperty("m_ResourceLists");
            m_StreamingAssetBundlesProperty = serializedObject.FindProperty("m_StreamingAssetBundles");

            VisualTreeAsset = AssetHelper.LoadAsset<VisualTreeAsset>("Uxml ResourceHashMap", "PointEditor");
        }
        protected override void SetupVisualElement(VisualElement root)
        {
            //Streaming AssetBundle
            {
                Button
                    assetBundleAddBtt = root.Q<Button>("StreamingAssetBundleAddButton"),
                    assetBundleRemoveBtt = root.Q<Button>("StreamingAssetBundleRemoveButton");
                IMGUIContainer assetBundleGUI = root.Q<IMGUIContainer>("StreamingAssetBundleGUI");

                if (m_StreamingAssetBundlesProperty.arraySize == 0)
                {
                    assetBundleGUI.parent.style.Hide(true);
                    assetBundleRemoveBtt.SetEnabled(false);
                }

                assetBundleAddBtt.clicked += delegate
                {
                    int index = m_StreamingAssetBundlesProperty.arraySize;
                    m_StreamingAssetBundlesProperty.InsertArrayElementAtIndex(index);

                    serializedObject.ApplyModifiedProperties();

                    var prop = m_StreamingAssetBundlesProperty.GetArrayElementAtIndex(index);
                    var ve = CoreGUI.VisualElement.PropertyField(prop);
                    assetBundleGUI.Add(ve);

                    if (index == 0)
                    {
                        assetBundleGUI.parent.style.Hide(false);
                        assetBundleRemoveBtt.SetEnabled(true);
                    }

                    assetBundleGUI.MarkDirtyRepaint();
                };
                assetBundleRemoveBtt.clicked += delegate
                {
                    if (m_StreamingAssetBundlesProperty.arraySize == 0) return;

                    int index = m_StreamingAssetBundlesProperty.arraySize - 1;
                    m_StreamingAssetBundlesProperty.DeleteArrayElementAtIndex(index);

                    serializedObject.ApplyModifiedProperties();

                    assetBundleGUI.RemoveAt(index);

                    if (index == 0)
                    {
                        assetBundleGUI.parent.style.Hide(true);
                        assetBundleRemoveBtt.SetEnabled(false);
                    }

                    assetBundleGUI.MarkDirtyRepaint();
                };

                for (int i = 0; i < m_StreamingAssetBundlesProperty.arraySize; i++)
                {
                    var prop = m_StreamingAssetBundlesProperty.GetArrayElementAtIndex(i);
                    var ve = CoreGUI.VisualElement.PropertyField(prop);

                    assetBundleGUI.Add(ve);
                }
                //assetBundleGUI.onGUIHandler += delegate
                //{
                //    for (int i = 0; i < m_StreamingAssetBundlesProperty.arraySize; i++)
                //    {
                //        var element = m_StreamingAssetBundlesProperty.GetArrayElementAtIndex(i);
                //        EditorGUILayout.PropertyField(element);
                //    }
                //};
            }

            // Scene Binded Labels
            {
#if UNITY_ADDRESSABLES
                Button
                    bindLabelAddBtt = root.Q<Button>("SceneBindedLabelAddButton"),
                    bindLabelRemoveBtt = root.Q<Button>("SceneBindedLabelRemoveButton");
                IMGUIContainer bindLabelGUI = root.Q<IMGUIContainer>("SceneBindedLabelsGUI");

                if (m_SceneBindedLabelsProperty.arraySize == 0)
                {
                    bindLabelGUI.parent.AddToClassList("hide");
                    bindLabelRemoveBtt.SetEnabled(false);
                }

                bindLabelAddBtt.clicked += delegate
                {
                    int index = m_SceneBindedLabelsProperty.arraySize;
                    m_SceneBindedLabelsProperty.InsertArrayElementAtIndex(index);

                    serializedObject.ApplyModifiedProperties();

                    if (index == 0)
                    {
                        bindLabelGUI.parent.RemoveFromClassList("hide");
                        bindLabelRemoveBtt.SetEnabled(true);
                    }

                    bindLabelGUI.parent.MarkDirtyRepaint();
                };
                bindLabelRemoveBtt.clicked += delegate
                {
                    if (m_SceneBindedLabelsProperty.arraySize == 0) return;

                    int index = m_SceneBindedLabelsProperty.arraySize - 1;
                    m_SceneBindedLabelsProperty.DeleteArrayElementAtIndex(index);

                    serializedObject.ApplyModifiedProperties();

                    if (index == 0)
                    {
                        bindLabelGUI.parent.AddToClassList("hide");
                        bindLabelRemoveBtt.SetEnabled(false);
                    }

                    bindLabelGUI.parent.MarkDirtyRepaint();
                };
                bindLabelGUI.onGUIHandler += delegate
                {
                    using (new EditorGUI.IndentLevelScope())
                    using (var change = new EditorGUI.ChangeCheckScope())
                    {
                        for (int i = 0; i < m_SceneBindedLabelsProperty.arraySize; i++)
                        {
                            var element = m_SceneBindedLabelsProperty.GetArrayElementAtIndex(i);
                            EditorGUILayout.PropertyField(element);
                        }

                        if (change.changed) serializedObject.ApplyModifiedProperties();
                    }
                };
#else
                VisualElement rootSceneBindedLabels = root.Q("SceneBindedLabels");

                rootSceneBindedLabels.style.Hide(true);
                rootSceneBindedLabels.SetEnabled(false);
#endif
            }

            // Resource Lists

            Button
                addResourceListBtt = root.Q<Button>("AddResourceListBtt"),
                removeResourceListBtt = root.Q<Button>("RemoveResourceListBtt");
            ResourceListContainer = root.Q<VisualElement>("ResourceListContainer");
            if (m_ResourceListsProperty.arraySize == 0)
            {
                ResourceListContainer.AddToClassList("hide");
                removeResourceListBtt.SetEnabled(false);
            }

            addResourceListBtt.clicked += delegate
            {
                int index = m_ResourceListsProperty.arraySize;

                ResourceList list = AssetHelper.AddSubAssetAt<ResourceList>(
                    assetPath, "ResourceList " + index);
                //list.name = ;
                //AssetDatabase.AddObjectToAsset(list, assetPath);

                m_ResourceListsProperty.InsertArrayElementAtIndex(index);
                var prop = m_ResourceListsProperty.GetArrayElementAtIndex(index);
                prop.objectReferenceValue = list;

                PropertyField element = new PropertyField(prop, prop.displayName);
                element.RemoveFromHierarchy();

                ResourceListContainer.Add(element);
                element.Bind(serializedObject);
                element.SetEnabled(false);

                ResourceListContainer.MarkDirtyRepaint();
                serializedObject.ApplyModifiedProperties();

                // if was zero element
                if (index == 0)
                {
                    ResourceListContainer.RemoveFromClassList("hide");
                    removeResourceListBtt.SetEnabled(true);
                }
            };
            removeResourceListBtt.clicked += delegate
            {
                if (m_ResourceListsProperty.arraySize == 0) return;

                int index = m_ResourceListsProperty.arraySize - 1;

                var ve = ResourceListContainer.ElementAt(index);
                ve.RemoveFromHierarchy();

                ResourceList list = m_ResourceListsProperty.GetArrayElementAtIndex(index).objectReferenceValue as ResourceList;
                m_ResourceListsProperty.DeleteArrayElementAtIndex(index);
                AssetDatabase.RemoveObjectFromAsset(list);

                serializedObject.ApplyModifiedProperties();

                if (index == 0)
                {
                    ResourceListContainer.AddToClassList("hide");
                    removeResourceListBtt.SetEnabled(false);
                }
                ResourceListContainer.MarkDirtyRepaint();
            };

            for (int i = 0; i < m_ResourceListsProperty.arraySize; i++)
            {
                SerializedProperty element = m_ResourceListsProperty.GetArrayElementAtIndex(i);

                PropertyField propertyField
                    = new PropertyField(element, element.displayName);
                propertyField.SetEnabled(false);
                ResourceListContainer.Add(propertyField);
            }
        }
        protected override VisualElement CreateVisualElement()
        {
            var tree = VisualTreeAsset.CloneTree();
            
            return tree;
        }
    }
}

#endif
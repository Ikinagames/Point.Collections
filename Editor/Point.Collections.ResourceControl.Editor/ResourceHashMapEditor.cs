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
        //VisualElement ResourceListContainer { get; set; }
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
            SetupStreamingAssetBundleList(root);

            // Scene Binded Labels
            {
                ListContainerView sceneBindedLabelList = root.Q<ListContainerView>("SceneBindedLabelList");
#if UNITY_ADDRESSABLES
                sceneBindedLabelList.isExpanded = m_SceneBindedLabelsProperty.isExpanded;
                sceneBindedLabelList.onExpand += delegate (bool isExpand)
                {
                    m_SceneBindedLabelsProperty.isExpanded = isExpand;
                    serializedObject.ApplyModifiedProperties();
                };

                sceneBindedLabelList.onAddButtonClicked += delegate(int index)
                {
                    m_SceneBindedLabelsProperty.InsertArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();

                    var prop = m_SceneBindedLabelsProperty.GetArrayElementAtIndex(index);
                    var ve = CoreGUI.VisualElement.PropertyField(prop);
                    return ve;
                };
                sceneBindedLabelList.onRemoveButtonClicked += delegate(int index)
                {
                    m_SceneBindedLabelsProperty.DeleteArrayElementAtIndex(index);
                    serializedObject.ApplyModifiedProperties();
                };

                for (int i = 0; i < m_SceneBindedLabelsProperty.arraySize; i++)
                {
                    var prop = m_SceneBindedLabelsProperty.GetArrayElementAtIndex(i);
                    var ve = CoreGUI.VisualElement.PropertyField(prop);

                    sceneBindedLabelList.Add(ve);
                }
#else
                sceneBindedLabelList.style.Hide(true);
                //VisualElement rootSceneBindedLabels = root.Q("SceneBindedLabels");

                //rootSceneBindedLabels.style.Hide(true);
                //rootSceneBindedLabels.SetEnabled(false);
#endif
            }

            // Resource Lists
            SetupResourceList(root);
        }
        private void SetupStreamingAssetBundleList(VisualElement root)
        {
            ListContainerView assetbundleList = root.Q<ListContainerView>("StreamingAssetBundleList");
            assetbundleList.isExpanded = m_StreamingAssetBundlesProperty.isExpanded;
            assetbundleList.onExpand += delegate (bool isExpand)
            {
                m_StreamingAssetBundlesProperty.isExpanded = isExpand;
                serializedObject.ApplyModifiedProperties();
            };
            assetbundleList.onAddButtonClicked += delegate(int index)
            {
                m_StreamingAssetBundlesProperty.InsertArrayElementAtIndex(index);

                serializedObject.ApplyModifiedProperties();

                var prop = m_StreamingAssetBundlesProperty.GetArrayElementAtIndex(index);
                var ve = CoreGUI.VisualElement.PropertyField(prop);

                return ve;
            };
            assetbundleList.onRemoveButtonClicked += delegate(int index)
            {
                m_StreamingAssetBundlesProperty.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            };

            for (int i = 0; i < m_StreamingAssetBundlesProperty.arraySize; i++)
            {
                var prop = m_StreamingAssetBundlesProperty.GetArrayElementAtIndex(i);
                var ve = CoreGUI.VisualElement.PropertyField(prop);

                assetbundleList.Add(ve);
            }
        }
        private void SetupResourceList(VisualElement root)
        {
            ListContainerView resourceList = root.Q<ListContainerView>("ResourceList");
            resourceList.isExpanded = m_ResourceListsProperty.isExpanded;
            resourceList.onExpand += delegate (bool isExpand)
            {
                m_ResourceListsProperty.isExpanded = isExpand;
                serializedObject.ApplyModifiedProperties();
            };
            resourceList.onAddButtonClicked += delegate(int index)
            {
                ResourceList list = AssetHelper.AddSubAssetAt<ResourceList>(
                    assetPath, "ResourceList " + index);

                m_ResourceListsProperty.InsertArrayElementAtIndex(index);
                var prop = m_ResourceListsProperty.GetArrayElementAtIndex(index);
                prop.objectReferenceValue = list;

                PropertyField element = CoreGUI.VisualElement.PropertyField(prop);
                element.SetEnabled(false);

                serializedObject.ApplyModifiedProperties();

                return element;
            };
            resourceList.onRemoveButtonClicked += delegate(int index)
            {
                ResourceList list = m_ResourceListsProperty.GetArrayElementAtIndex(index).objectReferenceValue as ResourceList;
                m_ResourceListsProperty.DeleteArrayElementAtIndex(index);
                AssetDatabase.RemoveObjectFromAsset(list);

                serializedObject.ApplyModifiedProperties();
            };

            for (int i = 0; i < m_ResourceListsProperty.arraySize; i++)
            {
                SerializedProperty element = m_ResourceListsProperty.GetArrayElementAtIndex(i);

                PropertyField propertyField = CoreGUI.VisualElement.PropertyField(element);
                propertyField.SetEnabled(false);
                resourceList.Add(propertyField);
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
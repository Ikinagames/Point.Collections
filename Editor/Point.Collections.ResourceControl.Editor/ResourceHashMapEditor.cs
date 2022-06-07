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

#if UNITY_2019_1_OR_NEWER && UNITY_ADDRESSABLES
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

        private void OnEnable()
        {
            m_SceneBindedLabelsProperty = serializedObject.FindProperty("m_SceneBindedLabels");
            m_ResourceListsProperty = serializedObject.FindProperty("m_ResourceLists");
            m_StreamingAssetBundlesProperty = serializedObject.FindProperty("m_StreamingAssetBundles");

            VisualTreeAsset = AssetHelper.LoadAsset<VisualTreeAsset>("Uxml ResourceHashMap", "PointEditor");
        }
        protected override VisualElement CreateVisualElement()
        {
            var tree = VisualTreeAsset.CloneTree();
            tree.Bind(serializedObject);

            IMGUIContainer streamingAssetBundleGUI = tree.Q<IMGUIContainer>("StreamingAssetBundleGUI");
            streamingAssetBundleGUI.onGUIHandler += StreamingAssetBundleGUI;

            Button 
                addResourceListBtt = tree.Q<Button>("AddResourceListBtt"),
                removeResourceListBtt = tree.Q<Button>("RemoveResourceListBtt");
            addResourceListBtt.clicked += ResourceListAddButton;
            removeResourceListBtt.clicked += ResourceListRemoveButton;

            //IMGUIContainer resourceListGUI = tree.Q<IMGUIContainer>("ResourceListGUI");
            //resourceListGUI.onGUIHandler += ResourceListGUI;

            ResourceListContainer = tree.Q<VisualElement>("ResourceListContainer");
            for (int i = 0; i < m_ResourceListsProperty.arraySize; i++)
            {
                SerializedProperty element = m_ResourceListsProperty.GetArrayElementAtIndex(i);
                
                //VisualElement box = new VisualElement();
                //box.name = $"{m_ResourceListsProperty.displayName}[{i}]";
                //box.AddToClassList("row-align");

                PropertyField propertyField
                    = new PropertyField(element, element.displayName);
                propertyField.SetEnabled(false);
                //box.Add(propertyField);

                //int index = i;
                //Button button = new Button(() =>
                //{
                //    $"clicked {index}".ToLog();
                //})
                //{
                //    text = "-"
                //};
                //box.Add(button);

                //ResourceListContainer.Add(box);
                ResourceListContainer.Add(propertyField);
            }

            return tree;
        }
        private void StreamingAssetBundleGUI()
        {
            EditorGUILayout.PropertyField(m_StreamingAssetBundlesProperty);
        }

        private void ResourceListAddButton()
        {
            int index = m_ResourceListsProperty.arraySize;

            ResourceList list = CreateInstance<ResourceList>();
            list.name = "ResourceList " + index;
            AssetDatabase.AddObjectToAsset(list, assetPath);

            m_ResourceListsProperty.InsertArrayElementAtIndex(index);
            m_ResourceListsProperty.GetArrayElementAtIndex(index).objectReferenceValue = list;

            "add".ToLog();
        }
        private void ResourceListRemoveButton()
        {
            int index = m_ResourceListsProperty.arraySize - 1;
            ResourceList list = m_ResourceListsProperty.GetArrayElementAtIndex(index).objectReferenceValue as ResourceList;
            m_ResourceListsProperty.DeleteArrayElementAtIndex(index);

            AssetDatabase.RemoveObjectFromAsset(list);

            "remove".ToLog();
        }

        private void ResourceListGUI()
        {
            using (new EditorGUI.DisabledGroupScope(true))
            {
                EditorGUILayout.PropertyField(m_ResourceListsProperty);
            }
        }

        //protected override void OnInspectorGUIContents()
        //{
        //    EditorGUILayout.PropertyField(m_SceneBindedLabelsProperty);

        //    EditorGUILayout.Space();
        //    CoreGUI.Line();

        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        if (CoreGUI.BoxButton("+", Color.gray))
        //        {
        //            int index = m_ResourceListsProperty.arraySize;

        //            ResourceList list = CreateInstance<ResourceList>();
        //            list.name = "ResourceList " + index;
        //            AssetDatabase.AddObjectToAsset(list, assetPath);

        //            m_ResourceListsProperty.InsertArrayElementAtIndex(index);
        //            m_ResourceListsProperty.GetArrayElementAtIndex(index).objectReferenceValue = list;
        //        }
        //        if (CoreGUI.BoxButton("-", Color.gray))
        //        {
        //            int index = m_ResourceListsProperty.arraySize - 1;
        //            ResourceList list = m_ResourceListsProperty.GetArrayElementAtIndex(index).objectReferenceValue as ResourceList;
        //            m_ResourceListsProperty.DeleteArrayElementAtIndex(index);

        //            AssetDatabase.RemoveObjectFromAsset(list);
        //        }
        //    }
        //    using (new EditorGUI.DisabledGroupScope(true))
        //    {
        //        EditorGUILayout.PropertyField(m_ResourceListsProperty);
        //    }

        //    serializedObject.ApplyModifiedProperties();
        //    //base.OnInspectorGUIContents();
        //}
    }
}

#endif
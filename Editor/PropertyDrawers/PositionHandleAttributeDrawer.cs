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

#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(PositionHandleAttribute))]
    internal sealed class PositionHandleAttributeDrawer : Vector3AttributeDrawer
    {
        protected override string OpenedButtonText => "Pick";
        protected override string OpenedButtonTooltip => "Scene view 에서 오브젝트 위치를 수정합니다.";
        protected override string ClosedButtonText => "Close";
        protected override string ClosedButtonTooltip => "Scene view 에서 오브젝트 위치를 수정합니다.";

        protected override bool Opened => Popup.Instance.IsOpened;

        protected override void OnButtonClick(SerializedProperty property)
        {
            if (!Opened)
            {
                Popup.Instance.SetProperty(property, attribute as PositionHandleAttribute);
                Popup.Instance.Open();

                Selection.selectionChanged += Close;
            }
            else
            {
                Close();
            }
        }
        private void Close()
        {
            Selection.selectionChanged -= Close;

            Popup.Instance.Close();
        }

        private sealed class Popup : CLRSingleTone<Popup>
        {
            private PositionHandleAttribute m_Attribute;
            private Transform m_Tr;

            private UnityEngine.Object m_Object;
            private string
                m_X, m_Y, m_Z;
            private Vector3 m_Value;

            public bool IsOpened { get; private set; } = false;

            protected override void OnDispose()
            {
                SceneView.duringSceneGui -= OnSceneGUI;
            }

            public void Open()
            {
                if (IsOpened) return;

                Tools.hidden = true;
                SceneView.duringSceneGui += OnSceneGUI;

                SceneView.RepaintAll();
                IsOpened = true;
            }
            public void Close()
            {
                SceneView.duringSceneGui -= OnSceneGUI;

                Apply();

                Tools.hidden = false;
                SceneView.RepaintAll();
                IsOpened = false;
            }
            private void Apply()
            {
                using (SerializedObject obj = new SerializedObject(m_Object))
                {
                    obj.FindProperty(m_X).floatValue = m_Value.x;
                    obj.FindProperty(m_Y).floatValue = m_Value.y;
                    obj.FindProperty(m_Z).floatValue = m_Value.z;

                    obj.ApplyModifiedProperties();
                }
            }
            public void SetProperty(SerializedProperty property, PositionHandleAttribute attribute)
            {
                m_Attribute = attribute;
                m_Object = property.serializedObject.targetObject;

                SerializedProperty
                    xProp = property.FindPropertyRelative("x"),
                    yProp = property.FindPropertyRelative("y"),
                    zProp = property.FindPropertyRelative("z");

                m_X = xProp.propertyPath;
                m_Y = yProp.propertyPath;
                m_Z = zProp.propertyPath;
                m_Value = new Vector3(
                    xProp.floatValue,
                    yProp.floatValue,
                    zProp.floatValue
                    );

                if (m_Attribute.Local)
                {
                    if (property.serializedObject.targetObject is GameObject obj)
                    {
                        m_Tr = obj.transform;
                    }
                    else if (property.serializedObject.targetObject is UnityEngine.Component com)
                    {
                        m_Tr = com.transform;
                    }
                }
            }

            private void OnSceneGUI(SceneView sceneView)
            {
                Handles.BeginGUI();
                float
                    width = 100,
                    height = PropertyDrawerHelper.GetPropertyHeight(1);

                var rect = AutoRect.LeftBottomAlign(width, height);
                GUI.BeginGroup(rect, EditorStyleUtilities.Box);
                AutoRect auto = new AutoRect(new Rect(0, 0, width, height));

                if (GUI.Button(auto.Pop(), "Close"))
                {
                    GUIUtility.hotControl = 0;
                    Close();
                }

                GUI.EndGroup();

                Handles.EndGUI();

                Vector3 changed;
                if (m_Tr != null)
                {
                    changed = Handles.DoPositionHandle(m_Value + m_Tr.position, quaternion.identity);
                    changed -= m_Tr.position;
                }
                else
                {
                    changed = Handles.DoPositionHandle(m_Value, quaternion.identity);
                }

                if (!m_Value.Equals(changed))
                {
                    m_Value = changed;
                    Apply();
                }

                // https://gamedev.stackexchange.com/questions/149514/use-unity-handles-for-interaction-in-the-scene-view
            }
            //
        }
        //
    }
}

#endif
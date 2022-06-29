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
#endif

using System.Reflection;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(RotationHandleAttribute))]
    internal sealed class RotationHandleAttributeDrawer : Vector3AttributeDrawer
    {
        new RotationHandleAttribute attribute => (RotationHandleAttribute)base.attribute;

        protected override string OpenedButtonText => "Pick";
        protected override string OpenedButtonTooltip => "Scene view 에서 오브젝트 위치를 수정합니다.";
        protected override string ClosedButtonText => "Close";
        protected override string ClosedButtonTooltip => "Scene view 에서 오브젝트 위치를 수정합니다.";

        protected override bool Opened => Popup.Instance.IsOpened;

        protected override float PropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.PropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;
        }

        protected override void OnButtonClick(SerializedProperty property)
        {
            if (!Opened)
            {
                SerializedProperty positionField = null;
                if (!attribute.PositionField.IsNullOrEmpty())
                {
                    var parent = property.GetParent();
                    positionField = parent.FindPropertyRelative(attribute.PositionField);
                }

                Popup.Instance.SetProperty(property, positionField);
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
            private UnityEngine.Object m_Object;
            private string
                m_Position,
                m_X, m_Y, m_Z, m_W;
            private Vector3 m_PositionValue;

            private bool m_IsLocalPosition;
            private Transform m_Transform;
            private Quaternion m_Value;

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

                m_Object = null;
                m_Transform = null;
                m_IsLocalPosition = false;
                m_Position = null;
                m_PositionValue = Vector3.zero;
            }
            public void Apply()
            {
                using (SerializedObject obj = new SerializedObject(m_Object))
                {
                    if (m_W != null)
                    {
                        obj.FindProperty(m_X).floatValue = m_Value.x;
                        obj.FindProperty(m_Y).floatValue = m_Value.y;
                        obj.FindProperty(m_Z).floatValue = m_Value.z;
                        obj.FindProperty(m_W).floatValue = m_Value.w;
                    }
                    else
                    {
                        var euler = m_Value.eulerAngles;
                        obj.FindProperty(m_X).floatValue = euler.x;
                        obj.FindProperty(m_Y).floatValue = euler.y;
                        obj.FindProperty(m_Z).floatValue = euler.z;
                    }

                    obj.ApplyModifiedProperties();
                }
            }
            public void SetProperty(SerializedProperty property, SerializedProperty positionProperty)
            {
                m_Object = property.serializedObject.targetObject;
                if (positionProperty != null)
                {
                    m_Position = positionProperty.propertyPath;
                    var positionAtt = positionProperty.GetFieldInfo().GetCustomAttribute<PositionHandleAttribute>();
                    if (positionAtt != null)
                    {
                        m_IsLocalPosition = positionAtt.Local;
                    }

                    m_PositionValue = positionProperty.GetVector3();
                    if (m_IsLocalPosition)
                    {
                        if (m_Object is GameObject obj)
                        {
                            m_Transform = obj.transform;
                        }
                        else if (m_Object is UnityEngine.Component com)
                        {
                            m_Transform = com.transform;
                        }

                        m_PositionValue = positionProperty.GetVector3() + m_Transform.position;
                    }
                    else m_PositionValue = positionProperty.GetVector3();
                }
                else
                {
                    m_Position = null;
                }

                SerializedProperty
                    xProp = property.FindPropertyRelative("x"),
                    yProp = property.FindPropertyRelative("y"),
                    zProp = property.FindPropertyRelative("z"),
                    wProp = property.FindPropertyRelative("w");

                m_X = xProp.propertyPath;
                m_Y = yProp.propertyPath;
                m_Z = zProp.propertyPath;
                
                if (wProp == null)
                {
                    m_W = null;
                    m_Value = Quaternion.Euler(new float3(
                        xProp.floatValue,
                        yProp.floatValue,
                        zProp.floatValue
                        ));
                }
                else
                {
                    m_W = wProp.propertyPath;
                    
                    if (xProp.floatValue == 0 && yProp.floatValue == 0 && 
                        zProp.floatValue == 0 && wProp.floatValue == 0)
                    {
                        m_Value = Quaternion.identity;
                    }
                    else
                    {
                        m_Value = new Quaternion(
                            xProp.floatValue,
                            yProp.floatValue,
                            zProp.floatValue,
                            wProp.floatValue
                            );
                    }
                }
            }

            private void OnSceneGUI(SceneView sceneView)
            {
                Handles.BeginGUI();
                float
                    width = 100,
                    height = CoreGUI.GetLineHeight(1);

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

                var changed = Handles.DoRotationHandle(m_Value, m_PositionValue);
                if (!m_Value.Equals(changed))
                {
                    m_Value = changed;
                    Apply();
                }

                var prevMatrix = Handles.matrix;
                {
                    var size = HandleUtility.GetHandleSize(m_PositionValue);
                    Handles.matrix = Matrix4x4.TRS(m_PositionValue, m_Value, Vector3.one);

                    Handles.DrawWireCube(Vector3.zero, Vector3.one * size * .5f);
                }
                Handles.matrix = prevMatrix;
                // https://gamedev.stackexchange.com/questions/149514/use-unity-handles-for-interaction-in-the-scene-view
            }
            //
        }
        //
    }
}

#endif
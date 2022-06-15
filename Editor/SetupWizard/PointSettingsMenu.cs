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

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    internal sealed class PointSettingsMenu : SetupWizardMenuItem
    {
        public override string Name => "Settings";
        public override int Order => -10000;

        SerializedObject serializedObject;
        SerializedProperty
            m_LogChannel, m_UserChannelNames,
            m_EnableLogFile, m_LogFilePath, m_LogDisplayLines,
            m_DisplayMainApplication,
            m_InActiveTime;

        public PointSettingsMenu()
        {
            serializedObject = new SerializedObject(PointSettings.Instance);

            m_LogChannel = serializedObject.FindProperty(nameof(m_LogChannel));
            m_UserChannelNames = serializedObject.FindProperty(nameof(m_UserChannelNames));
            m_EnableLogFile = serializedObject.FindProperty(nameof(m_EnableLogFile));
            m_LogFilePath = serializedObject.FindProperty(nameof(m_LogFilePath));
            m_LogDisplayLines = serializedObject.FindProperty(nameof(m_LogDisplayLines));
            m_DisplayMainApplication = serializedObject.FindProperty(nameof(m_DisplayMainApplication));
            m_InActiveTime = serializedObject.FindProperty(nameof(m_InActiveTime));
        }
        public override void OnFocus()
        {
            serializedObject.Update();
        }

        public override bool Predicate()
        {
            return true;
        }
        protected override VisualElement CreateVisualElement()
        {
            ScrollView root = new ScrollView();
#if UNITY_2021_1_OR_NEWER
            root.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            root.verticalScrollerVisibility = ScrollerVisibility.Auto;
#endif
            root.style.flexGrow = 1;

            PropertyField field = CoreGUI.VisualElement.PropertyField(m_LogChannel);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);

            field = CoreGUI.VisualElement.PropertyField(m_UserChannelNames);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);

            root.Add(CoreGUI.VisualElement.Space());

            field = CoreGUI.VisualElement.PropertyField(m_EnableLogFile);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);

            field = CoreGUI.VisualElement.PropertyField(m_LogFilePath);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);

            field = CoreGUI.VisualElement.PropertyField(m_LogDisplayLines);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);

            root.Add(CoreGUI.VisualElement.Space());

            field = CoreGUI.VisualElement.PropertyField(m_DisplayMainApplication);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);

#if ENABLE_INPUT_SYSTEM
            field = CoreGUI.VisualElement.PropertyField(m_InActiveTime);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);
#endif

            return root;
        }
        private void OnValueChanged(SerializedPropertyChangeEvent ev)
        {
            EditorUtility.SetDirty(PointSettings.Instance);
        }
    }
}

#endif
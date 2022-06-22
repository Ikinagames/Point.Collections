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
#if !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#endif
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
            m_DisplayMainApplication;
#if ENABLE_INPUT_SYSTEM
        SerializedProperty m_InActiveTime;
#endif

        public PointSettingsMenu()
        {
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
            serializedObject = new SerializedObject(PointSettings.Instance);

            m_LogChannel = serializedObject.FindProperty(nameof(m_LogChannel));
            m_UserChannelNames = serializedObject.FindProperty(nameof(m_UserChannelNames));
            m_EnableLogFile = serializedObject.FindProperty(nameof(m_EnableLogFile));
            m_LogFilePath = serializedObject.FindProperty(nameof(m_LogFilePath));
            m_LogDisplayLines = serializedObject.FindProperty(nameof(m_LogDisplayLines));
            m_DisplayMainApplication = serializedObject.FindProperty(nameof(m_DisplayMainApplication));

#if ENABLE_INPUT_SYSTEM
            m_InActiveTime = serializedObject.FindProperty(nameof(m_InActiveTime));
#endif

            ScrollView root = new ScrollView();
#if UNITY_2021_1_OR_NEWER
            root.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            root.verticalScrollerVisibility = ScrollerVisibility.Auto;
#endif
            root.style.flexGrow = 1;

            PropertyField 
                logField = CoreGUI.VisualElement.PropertyField(m_LogChannel),
                channelNameField = CoreGUI.VisualElement.PropertyField(m_UserChannelNames),
                enableLogField = CoreGUI.VisualElement.PropertyField(m_EnableLogFile),
                logFileField = CoreGUI.VisualElement.PropertyField(m_LogFilePath),
                logDisplayLinesField = CoreGUI.VisualElement.PropertyField(m_LogDisplayLines),
                displayMainAppfield = CoreGUI.VisualElement.PropertyField(m_DisplayMainApplication);

#if !UNITYENGINE_OLD
            logField.RegisterValueChangeCallback(OnValueChanged);
            channelNameField.RegisterValueChangeCallback(OnValueChanged);
            enableLogField.RegisterValueChangeCallback(OnValueChanged);
            logFileField.RegisterValueChangeCallback(OnValueChanged);
            logDisplayLinesField.RegisterValueChangeCallback(OnValueChanged);
            displayMainAppfield.RegisterValueChangeCallback(OnValueChanged);
#endif

            root.Add(logField);
            root.Add(channelNameField);
            root.Add(CoreGUI.VisualElement.Space());
            root.Add(enableLogField);
            root.Add(logFileField);
            root.Add(logDisplayLinesField);
            root.Add(CoreGUI.VisualElement.Space());
            root.Add(displayMainAppfield);

#if ENABLE_INPUT_SYSTEM
            var field = CoreGUI.VisualElement.PropertyField(m_InActiveTime);
            field.RegisterValueChangeCallback(OnValueChanged);
            root.Add(field);
#endif

            return root;
        }
#if !UNITYENGINE_OLD
        private void OnValueChanged(SerializedPropertyChangeEvent ev)
        {
            EditorUtility.SetDirty(PointSettings.Instance);
        }
#endif
    }
}

#endif
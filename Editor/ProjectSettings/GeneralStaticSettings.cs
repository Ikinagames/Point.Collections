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

#if UNITY_2020
#define UNITYENGINE
#endif

#if UNITYENGINE

using System;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [DisplayName("General Settings")]
    public sealed class GeneralStaticSettings : PointStaticSetting<GeneralStaticSettings>
    {
        private sealed class GUIContentHelper
        {
            public static readonly GUIContent Default = new GUIContent("Invalid");
            public static readonly GUIContent LogChannel = new GUIContent(
                "Log Channel",
                "로그에 표시할 채널을 지정합니다. 제외된 채널로 출력하는 메세지는 로그에서 제외됩니다.");
        }

        private SerializedObject m_PointSettings;

        private SerializedProperty m_LogChannel;
        private SerializedProperty m_UserChannelNames;
        private GUIContent[] m_LogChannelNames;
        private int[] m_LogChannelValues;

        private bool m_OpenCustomChannel = false;

        protected override void OnInitialize()
        {
            m_PointSettings = new SerializedObject(PointSettings.Instance);
            m_LogChannel = m_PointSettings.FindProperty("m_LogChannel");
            m_UserChannelNames = m_PointSettings.FindProperty("m_UserChannelNames");

            LogChannel[] values = TypeHelper.Enum<LogChannel>.Values;
            m_LogChannelNames = new GUIContent[TypeHelper.Enum<LogChannel>.Length];
            m_LogChannelValues = TypeHelper.Enum<LogChannel>.Values.Select(t => Convert.ToInt32(t)).ToArray();
            for (int i = 0; i < m_LogChannelNames.Length; i++)
            {
                m_LogChannelNames[i] = new GUIContent(PointSettings.Instance.GetUserChannelName(values[i]));
            }
        }
        protected override void OnSettingGUI(string searchContext)
        {
            using (var change = new EditorGUI.ChangeCheckScope())
            {
                PointSettings.Instance.LogChannel
                    = (LogChannel)EditorGUILayout.EnumFlagsField(
                        GUIContentHelper.LogChannel, 
                        PointSettings.Instance.LogChannel
                        //, 
                        //m_LogChannelNames, 
                        //m_LogChannelValues
                        );

                if (change.changed)
                {
                    EditorUtility.SetDirty(PointSettings.Instance);
                    m_PointSettings.ApplyModifiedProperties();
                }
            }

            m_OpenCustomChannel =
                EditorUtilities.Foldout(m_OpenCustomChannel, "Custom Channel Names");
            if (m_OpenCustomChannel)
            {
                EditorGUI.indentLevel++;
                using (var change = new EditorGUI.ChangeCheckScope())
                using (new EditorUtilities.BoxBlock(Color.black))
                {
                    for (int i = 1; i < m_LogChannelNames.Length && i < 28; i++)
                    {
                        m_LogChannelNames[i].text =
                            EditorGUILayout.DelayedTextField(m_LogChannelNames[i].text);
                    }

                    if (change.changed)
                    {
                        for (int i = 0; i < 27; i++)
                        {
                            var element = m_UserChannelNames.GetArrayElementAtIndex(i);
                            element.stringValue = m_LogChannelNames[i + 1].text;
                        }

                        m_PointSettings.ApplyModifiedProperties();
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}

#endif
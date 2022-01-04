// Copyright 2021 Ikina Games
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

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.ComponentModel;

namespace Point.Collections.Editor
{
    public sealed class PointProjectSettings : EditorStaticScriptableObject<PointProjectSettings>
    {
        [SerializeField] private bool[] m_OpenStaticSettings = Array.Empty<bool>();
        [SerializeField] private IPointStaticSetting[] m_StaticSettings = Array.Empty<IPointStaticSetting>();

        [SerializeField] private int m_Number;

        [SerializeField] private string m_SomeString;

        public bool[] OpenStaticSettings => m_OpenStaticSettings;
        public IReadOnlyList<IPointStaticSetting> StaticSettings => m_StaticSettings;

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(Instance);
        }

        public TSetting GetSetting<TSetting>() where TSetting : class, IPointStaticSetting
        {
            for (int i = 0; i < m_StaticSettings.Length; i++)
            {
                if (m_StaticSettings[i] is TSetting setting) return setting;
            }

            return null;
        }

        protected override void OnInitialize()
        {
            var customSettingsIter = TypeHelper.GetTypesIter((other) => !other.IsAbstract && !other.IsInterface && TypeHelper.TypeOf<IPointStaticSetting>.Type.IsAssignableFrom(other));
            m_StaticSettings = new IPointStaticSetting[customSettingsIter.Count()];
            m_OpenStaticSettings = new bool[m_StaticSettings.Length];

            int index = 0;
            foreach (var type in customSettingsIter)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

                var insField = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                IPointStaticSetting setting = insField.GetGetMethod().Invoke(null, null) as IPointStaticSetting;
                m_StaticSettings[index] = setting;

                index++;
            }
        }
    }
    internal sealed class PointProjectSettingsProvider : SettingsProvider
    {
        private sealed class Styles
        {
            public static GUIContent 
                AssetImportHandles = new GUIContent("Auto Asset Import", "해당 타입의 에셋이 등록되었을 때, 자동으로 ResourceAddresses 에서 해당 에셋을 관리합니다.");
            public static GUIContent number = new GUIContent("My Number");
            public static GUIContent someString = new GUIContent("Some string");
        }

        // Register the SettingsProvider
        [SettingsProvider, InitializeOnLoadMethod]
        public static SettingsProvider CreatePointProjectSettingsProvider()
        {
            var provider = new PointProjectSettingsProvider("Project/Point Framework", SettingsScope.Project);

            // Automatically extract all keywords from the Styles.
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
            return provider;
        }

        private SerializedObject m_CustomSettings;

        public PointProjectSettingsProvider(
            string path, 
            SettingsScope scopes = SettingsScope.User, 
            IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }
        public PointProjectSettingsProvider(
            string path, 
            SettingsScope scopes = SettingsScope.User) : base(path, scopes)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            m_CustomSettings = PointProjectSettings.GetSerializedSettings();
        }
        public override void OnGUI(string searchContext)
        {
            EditorUtilities.Line();

            using (var change = new EditorGUI.ChangeCheckScope())
            {
                var list = PointProjectSettings.Instance.StaticSettings;
                for (int i = 0; i < list.Count; i++)
                {
                    DrawSetting(i, list[i]);

                    if (i + 1 < list.Count) EditorUtilities.Line();
                }

                if (change.changed)
                {
                    EditorUtility.SetDirty(PointProjectSettings.Instance);
                }
            }
            m_CustomSettings.ApplyModifiedProperties();

            void DrawSetting(int i, IPointStaticSetting setting)
            {
                using (var settingChange = new EditorGUI.ChangeCheckScope())
                using (new EditorUtilities.BoxBlock(Color.black))
                {
                    PointProjectSettings.Instance.OpenStaticSettings[i]
                        = EditorUtilities.Foldout(PointProjectSettings.Instance.OpenStaticSettings[i], GetSettingDisplayName(setting), 16);

                    GUILayout.Space(6);

                    if (!PointProjectSettings.Instance.OpenStaticSettings[i]) return;

                    EditorGUI.indentLevel++;
                    setting.OnSettingGUI(searchContext);
                    EditorGUI.indentLevel--;

                    if (settingChange.changed)
                    {
                        EditorUtility.SetDirty((UnityEngine.Object)setting);
                    }
                }
            }
        }

        private static string GetSettingDisplayName(IPointStaticSetting setting)
        {
            Type t = setting.GetType();
            DisplayNameAttribute nameAttribute = t.GetCustomAttribute<DisplayNameAttribute>();
            if (nameAttribute != null)
            {
                return nameAttribute.DisplayName;
            }

            return TypeHelper.ToString(t);
        }
    }
}

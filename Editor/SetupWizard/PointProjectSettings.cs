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

namespace Point.Collections.Editor
{
    public sealed class PointProjectSettings : ScriptableObject
    {
        [SerializeField]
        private int m_Number;

        [SerializeField]
        private string m_SomeString;

        internal static PointProjectSettings GetOrCreateSettings()
        {
            string filename = Path.GetFileName(PointProjectSettingsProvider.c_SettingsPath);
            string path = PointProjectSettingsProvider.c_SettingsPath.Substring(0, PointProjectSettingsProvider.c_SettingsPath.Length - filename.Length);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var settingsArr = AssetDatabase.LoadAllAssetsAtPath(PointProjectSettingsProvider.c_SettingsPath);
            //$"{settingsArr.Length}".ToLog();
            PointProjectSettings settings = settingsArr.Length == 0 ? null : (PointProjectSettings)settingsArr[0];
            if (settings == null)
            //if (!File.Exists(PointProjectSettingsProvider.c_SettingsPath))
            {
                settings = ScriptableObject.CreateInstance<PointProjectSettings>();
                settings.m_Number = 42;
                settings.m_SomeString = "The answer to the universe";

                //string tempPath = Path.Combine("Assets", filename);

                //AssetDatabase.CreateAsset(settings, tempPath);
                AssetDatabase.CreateAsset(settings, PointProjectSettingsProvider.c_SettingsPath);
                AssetDatabase.SaveAssets();
                //File.Move(tempPath, PointProjectSettingsProvider.c_SettingsPath);
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
    internal sealed class PointProjectSettingsProvider : SettingsProvider
    {
        public const string c_SettingsPath = "Assets/Editor/PointProjectSettings.asset";
        //public const string c_SettingsPath = "ProjectSettings/PointProjectSettings.asset";

        private sealed class Styles
        {
            public static GUIContent number = new GUIContent("My Number");
            public static GUIContent someString = new GUIContent("Some string");
        }

        // Register the SettingsProvider
        [SettingsProvider, InitializeOnLoadMethod]
        public static SettingsProvider CreatePointProjectSettingsProvider()
        {
            //PointProjectSettings.GetOrCreateSettings();

            //if (IsSettingsAvailable())
            {
                var provider = new PointProjectSettingsProvider("Project/PointProjectSettingsProvider", SettingsScope.Project);

                // Automatically extract all keywords from the Styles.
                provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
                return provider;
            }

            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return null;
        }
        public static bool IsSettingsAvailable()
        {
            return File.Exists(c_SettingsPath);
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
            // Use IMGUI to display UI:
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_Number"), Styles.number);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_SomeString"), Styles.someString);
        }
    }
}

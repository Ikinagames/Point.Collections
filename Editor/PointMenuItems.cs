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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.Native;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    public sealed class PointMenuItems : EditorWindow
    {
        static PointSetupWizard m_SetupWizard;

        [MenuItem("Point/Setup Wizard")]
        public static void CoreSystemSetupWizard()
        {
            m_SetupWizard = (PointSetupWizard)GetWindow(TypeHelper.TypeOf<PointSetupWizard>.Type, true, "Point Framework Setup Wizard");
            m_SetupWizard.ShowUtility();
            m_SetupWizard.minSize = new Vector2(600, 500);
            m_SetupWizard.maxSize = m_SetupWizard.minSize;
            var position = new Rect(Vector2.zero, m_SetupWizard.minSize);
            Vector2 screenCenter = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2;
            position.center = screenCenter / EditorGUIUtility.pixelsPerPoint;
            m_SetupWizard.position = position;
        }

        [MenuItem("Point/Utils/Generate Guid")]
        public static void GenerateGUID()
        {
            Guid guid = Guid.NewGuid();
            EditorGUIUtility.systemCopyBuffer = guid.ToString();

            PointHelper.Log(LogChannel.Editor, $"{guid} is copied to clipboard.");
        }

        [MenuItem("Assets/Create/Point/Create T4")]
        public static void CreateAsset()
        {
            string filePath;
            if (Selection.assetGUIDs.Length == 0)
                filePath = "Assets/New TMP Color Gradient.asset";
            else
                filePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

            $"{filePath}".ToLog();
            filePath = Path.Combine(filePath, "t4Templete.txml");

            File.WriteAllText(filePath, string.Empty);
            AssetDatabase.ImportAsset(filePath);
        }
    }
}

#endif
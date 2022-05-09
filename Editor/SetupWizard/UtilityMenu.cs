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

using UnityEngine;
using UnityEditor;

namespace Point.Collections.Editor
{
    internal sealed class UtilityMenu : SetupWizardMenuItem
    {
        private const string
            c_EnableAssetInspectorKey = "point-enable-asset-inspector";
        public static bool EnableAssetInspector
        {
            get => EditorPrefs.GetBool(c_EnableAssetInspectorKey, false);
            set => EditorPrefs.SetBool(c_EnableAssetInspectorKey, value);
        }

        public override string Name => "Utility";
        public override int Order => -9998;

        private GUIContent EnableAssetInspectorGUIContent = new GUIContent
        {
            text = "Enable AssetInspector"
        };

        public override bool Predicate()
        {
            return true;
        }
        public override void OnGUI()
        {
            bool enableAssetInspector = EnableAssetInspector;

            EditorGUI.BeginChangeCheck();
            enableAssetInspector = EditorGUILayout.ToggleLeft(EnableAssetInspectorGUIContent, enableAssetInspector);
            if (EditorGUI.EndChangeCheck())
            {
                EnableAssetInspector = enableAssetInspector;
            }
        }
    }
}

#endif
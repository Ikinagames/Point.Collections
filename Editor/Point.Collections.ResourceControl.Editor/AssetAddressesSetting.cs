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

using Point.Collections.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.ResourceControl.Editor
{
    [DisplayName("Asset Addresses")]
    internal sealed class AssetAddressesSetting : PointStaticSettingBase<AssetAddressesSetting>
    {
        [Flags]
        public enum AssetImportHandles
        {
            None = 0,

            Audio = 0x00001,

            All = ~0
        }
        public enum AssetStrategy
        {
            AssetBundle,
            Addressable
        }

        [SerializeField] private AssetImportHandles m_AssetImportHandles = AssetImportHandles.None;
        [SerializeField] private AssetStrategy m_Strategy = AssetStrategy.AssetBundle;
        [SerializeField] private AssetID[] m_AssetIDs = Array.Empty<AssetID>();

        public IReadOnlyList<AssetID> AssetIDs => m_AssetIDs;

        private static class GUIStyleContents
        {
            public static GUIContent
                AssetImportHandles = new GUIContent("Auto Asset Import", "해당 타입의 에셋이 등록되었을 때, 자동으로 ResourceAddresses 에서 해당 에셋을 관리합니다.");
        }

        protected override void OnSettingGUI(string searchContext)
        {
            using (new EditorUtilities.BoxBlock(Color.black))
            {
                EditorUtilities.StringRich("Generals", 13);
                EditorUtilities.Line();
                EditorGUI.indentLevel++;

                m_AssetImportHandles
                    = (AssetImportHandles)EditorGUILayout.EnumFlagsField(GUIStyleContents.AssetImportHandles, m_AssetImportHandles);

                EditorGUI.indentLevel--;
            }
        }
    }
}

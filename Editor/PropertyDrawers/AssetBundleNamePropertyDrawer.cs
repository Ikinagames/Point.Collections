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

#if UNITY_EDITOR
#define DEBUG_MODE

using System;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    //[CustomPropertyDrawer(typeof(AssetBundleName))]
    internal sealed class AssetBundleNamePropertyDrawer : PropertyDrawer<AssetBundleName>
    {
        private string[] m_AssetBundleNames = Array.Empty<string>();

        protected override void OnInitialize(SerializedProperty property)
        {
            m_AssetBundleNames = AssetDatabase.GetAllAssetBundleNames();

            base.OnInitialize(property);
        }

        protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            base.OnPropertyGUI(ref rect, property, label);
        }
    }
}

#endif
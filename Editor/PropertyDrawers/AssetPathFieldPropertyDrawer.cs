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

using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(AssetPathField), true)]
    internal sealed class AssetPathFieldPropertyDrawer : AssetPathFieldPropertyDrawerFallback
    {
        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            AssetPathFieldView ve = new AssetPathFieldView(property);
            if (!property.IsInArray() || property.GetParent().ChildCount() > 1)
            {
                ve.label = property.displayName;
            }

            return ve;
        }
    }
    internal abstract class AssetPathFieldPropertyDrawerFallback : PropertyDrawerUXML<AssetPathField>
    {


        private static UnityEngine.Object GetObjectAtPath(in string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        }
    }
}

#endif
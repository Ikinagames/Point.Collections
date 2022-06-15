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

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(Hash))]
    internal sealed class HashPropertyDrawer : PropertyDrawerUXML<Hash>
    {
        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            PropertyField field = new PropertyField(
                property.FindPropertyRelative("m_Value"), property.displayName);
            
            return field;
        }
        protected override void SetupVisualElement(SerializedProperty property, VisualElement root)
        {
            base.SetupVisualElement(property, root);
        }
    }
}

#endif
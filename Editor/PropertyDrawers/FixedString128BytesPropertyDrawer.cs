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
#if UNITY_MATHEMATICS
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
#endif


namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(FixedString128Bytes))]
    internal sealed class FixedString128BytesPropertyDrawer : PropertyDrawerUXML<FixedString128Bytes>
    {
        //protected override void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        //{
        //    string str = SerializedPropertyHelper.ReadFixedString128Bytes(property).ToString();
        //    str = TextField(ref rect, label, str);
        //    SerializedPropertyHelper.SetFixedString128Bytes(property, str);

        //    //base.OnPropertyGUI(ref rect, property, label);
        //}
        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            property = property.Copy();
            string str = SerializedPropertyHelper.ReadFixedString128Bytes(property).ToString();

            TextField field = new TextField(property.displayName);
            field.value = str;
            field.RegisterValueChangedCallback(t =>
            {
                SerializedPropertyHelper.SetFixedString128Bytes(property, t.newValue);
            });

            return field;
        }
    }
}

#endif
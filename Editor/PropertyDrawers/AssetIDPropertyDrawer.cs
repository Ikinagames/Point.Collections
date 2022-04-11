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

//using Point.Collections.ResourceControl;
//using System.Collections.Generic;
//using Unity.Collections;
//using UnityEditor;
//using UnityEngine;

//namespace Point.Collections.Editor
//{
//    //[CustomPropertyDrawer(typeof(AssetID))]
//    //public sealed class AssetIDPropertyDrawer : PropertyDrawer
//    //{
//    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    //    {
//    //        return EditorGUIUtility.singleLineHeight * 2;
//    //        //return base.GetPropertyHeight(property, label);
//    //    }
//    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    //    {
//    //        SerializedProperty stringKeyProp = property.FindPropertyRelative("m_StringKey");
//    //        FixedString4096Bytes stringKey = (FixedString4096Bytes)PropertyDrawerHelper.GetTargetObjectOfProperty(stringKeyProp);

//    //        List<string> items = new List<string>()
//    //        {
//    //            "StringKey", "asd"
//    //        };

//    //        var rects = EditorGUIUtility.GetFlowLayoutedRects(position, "Box", EditorGUIUtility.singleLineHeight, EditorGUIUtility.standardVerticalSpacing,
//    //            items);

//    //        using (new EditorGUI.PropertyScope(position, label, property))
//    //        {
//    //            EditorGUI.LabelField(rects[0], stringKey.ToString());

//    //            EditorGUI.LabelField(rects[1], "asd");
//    //        }
//    //    }
//    //}
//}

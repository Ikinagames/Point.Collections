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

using System;
using System.Collections;
using System.Reflection;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    [CustomPropertyDrawer(typeof(NotEditableAttribute), true)]
    public sealed class NotEditablePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUI.PropertyField(position, property);
                }
            }
        }
    }
    
    //[CustomPropertyDrawer(typeof(FixedString4096Bytes))]
    //public sealed class FixedString4096BytesPropertyDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        object obj = PropertyDrawerHelper.GetTargetObjectOfProperty(property);
            
    //        using (var changed = new EditorGUI.ChangeCheckScope())
    //        using (new EditorGUI.PropertyScope(position, label, property))
    //        {
    //            FixedString4096Bytes value = EditorGUI.DelayedTextField(position, property.displayName, obj.ToString());

    //            if (changed.changed)
    //            {
    //                //if (PropertyDrawerHelper.IsPropertyInArray(property))
    //                {
    //                    SerializedProperty arrayProp = PropertyDrawerHelper.GetParentArrayOfProperty(property, out int i);
    //                    IList array = PropertyDrawerHelper.GetTargetObjectOfProperty(arrayProp) as IList;

    //                    var temp = __makeref(obj);
    //                    //object boxed = array[i];
    //                    //fieldInfo.SetValue(boxed, value);
    //                    fieldInfo.SetValueDirect(temp, value);
    //                    //array[i] = boxed;
    //                }
    //                //else
    //                //{
    //                //    SerializedProperty parent = PropertyDrawerHelper.GetParentOfProperty(property);
    //                //    object parentObj = PropertyDrawerHelper.GetTargetObjectOfProperty(parent);

    //                //    fieldInfo.SetValue(parentObj, value);

    //                //    if (!parentObj.GetType().IsClass)
    //                //    {
    //                //        parent = PropertyDrawerHelper.GetParentOfProperty(parent);
    //                //        if (parent == null)
    //                //        {
    //                //            UnityEngine.Object target = property.serializedObject.targetObject;
                                
    //                //        }
    //                //        else
    //                //        {
    //                //            throw new NotImplementedException();
    //                //        }
    //                //    }
    //                //    $"{property.propertyPath} {parent.displayName} :: {parent.type} :: {parentObj}".ToLog();
    //                //}
    //            }
    //        }
    //    }
    //}
}

﻿// Copyright 2022 Ikina Games
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
using Unity.Mathematics;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    public static class SerializedPropertyHelper
    {
        private static GUIContent
            s_None = new GUIContent(("None")),
            s_Invalid = new GUIContent("Invalid");

        #region Hash

        public static void ApplyToProperty(this in Hash hash, SerializedProperty property) => SetHash(property, hash);

        public static Hash ReadHash(SerializedProperty property)
        {
            SerializedProperty
                bits = property.FindPropertyRelative("mBits");

            return new Hash((uint)bits.intValue);
        }
        public static void SetHash(SerializedProperty property, Hash hash)
        {
            SerializedProperty
                bits = property.FindPropertyRelative("mBits");

            bits.longValue = (long)(ulong)hash;
        }

        #endregion

#if UNITY_COLLECTIONS

        #region Unity.Collections

        public static void ApplyToProperty(this in FixedString128Bytes t, SerializedProperty property) => SetFixedString128Bytes(property, t);

        private static class FixedString128Fields
        {
            private static FieldInfo
                s_Utf8LengthInBytes, s_bytes;

            public const string
                utf8LengthInBytesStr = "utf8LengthInBytes",
                bytesStr = "bytes";

            public static FieldInfo utf8LengthInBytes
            {
                get
                {
                    if (s_Utf8LengthInBytes == null)
                    {
                        s_Utf8LengthInBytes = TypeHelper.TypeOf<FixedString128Bytes>.GetFieldInfo(utf8LengthInBytesStr);
                    }
                    return s_Utf8LengthInBytes;
                }
            }
            public static FieldInfo bytes
            {
                get
                {
                    if (s_bytes == null)
                    {
                        s_bytes = TypeHelper.TypeOf<FixedString128Bytes>.GetFieldInfo(bytesStr);
                    }

                    return s_bytes;
                }
            }
        }

        public static void SetFixedString128Bytes(
            SerializedProperty property, FixedString128Bytes str)
        {
            SerializedProperty
                utf8LengthInBytes = property.FindPropertyRelative(FixedString128Fields.utf8LengthInBytesStr),
                bytes = property.FindPropertyRelative(FixedString128Fields.bytesStr);

            utf8LengthInBytes.intValue = (ushort)FixedString128Fields.utf8LengthInBytes.GetValue(str);
            FixedBytes126 bytes126 = (FixedBytes126)FixedString128Fields.bytes.GetValue(str);

            SetFixedBytes126(bytes, bytes126);
        }
        public static FixedString128Bytes ReadFixedString128Bytes(SerializedProperty property)
        {
            SerializedProperty
                utf8LengthInBytes = property.FindPropertyRelative(FixedString128Fields.utf8LengthInBytesStr),
                bytes = property.FindPropertyRelative(FixedString128Fields.bytesStr);

            FixedString128Bytes result = new FixedString128Bytes();
            object boxed = result;

            FixedString128Fields.utf8LengthInBytes.SetValue(boxed, (ushort)utf8LengthInBytes.intValue);
            FixedString128Fields.bytes.SetValue(boxed, ReadFixedBytes126(bytes));

            result = (FixedString128Bytes)boxed;

            return result;
        }

        public static FixedBytes126 ReadFixedBytes126(SerializedProperty property)
        {
            SerializedProperty item = property.FindPropertyRelative("offset0000");
            FixedBytes126 result = new FixedBytes126();
            result.offset0000 = ReadFixedBytes16(item);

            item.Next(false);
            result.offset0016 = ReadFixedBytes16(item);

            item.Next(false);
            result.offset0032 = ReadFixedBytes16(item);

            item.Next(false);
            result.offset0048 = ReadFixedBytes16(item);

            item.Next(false);
            result.offset0064 = ReadFixedBytes16(item);

            item.Next(false);
            result.offset0080 = ReadFixedBytes16(item);

            item.Next(false);
            result.offset0096 = ReadFixedBytes16(item);

            item.Next(false);
            result.byte0112 = (byte)item.intValue;

            item.Next(false);
            result.byte0113 = (byte)item.intValue;

            item.Next(false);
            result.byte0114 = (byte)item.intValue;

            item.Next(false);
            result.byte0115 = (byte)item.intValue;

            item.Next(false);
            result.byte0116 = (byte)item.intValue;

            item.Next(false);
            result.byte0117 = (byte)item.intValue;

            item.Next(false);
            result.byte0118 = (byte)item.intValue;

            item.Next(false);
            result.byte0119 = (byte)item.intValue;

            item.Next(false);
            result.byte0120 = (byte)item.intValue;

            item.Next(false);
            result.byte0121 = (byte)item.intValue;

            item.Next(false);
            result.byte0122 = (byte)item.intValue;

            item.Next(false);
            result.byte0123 = (byte)item.intValue;

            item.Next(false);
            result.byte0124 = (byte)item.intValue;

            item.Next(false);
            result.byte0125 = (byte)item.intValue;

            return result;
        }
        public static void SetFixedBytes126(SerializedProperty property, FixedBytes126 bytes)
        {
            SerializedProperty item = property.FindPropertyRelative("offset0000");
            SetFixedBytes16(item, bytes.offset0000);

            item.Next(false);
            SetFixedBytes16(item, bytes.offset0016);

            item.Next(false);
            SetFixedBytes16(item, bytes.offset0032);

            item.Next(false);
            SetFixedBytes16(item, bytes.offset0048);

            item.Next(false);
            SetFixedBytes16(item, bytes.offset0064);

            item.Next(false);
            SetFixedBytes16(item, bytes.offset0080);

            item.Next(false);
            SetFixedBytes16(item, bytes.offset0096);

            item.Next(false);
            item.intValue = bytes.byte0112;

            item.Next(false);
            item.intValue = bytes.byte0113;

            item.Next(false);
            item.intValue = bytes.byte0114;

            item.Next(false);
            item.intValue = bytes.byte0115;

            item.Next(false);
            item.intValue = bytes.byte0116;

            item.Next(false);
            item.intValue = bytes.byte0117;

            item.Next(false);
            item.intValue = bytes.byte0118;

            item.Next(false);
            item.intValue = bytes.byte0119;

            item.Next(false);
            item.intValue = bytes.byte0120;

            item.Next(false);
            item.intValue = bytes.byte0121;

            item.Next(false);
            item.intValue = bytes.byte0122;

            item.Next(false);
            item.intValue = bytes.byte0123;

            item.Next(false);
            item.intValue = bytes.byte0124;

            item.Next(false);
            item.intValue = bytes.byte0125;
        }

        public static FixedBytes16 ReadFixedBytes16(SerializedProperty property)
        {
            SerializedProperty item = property.FindPropertyRelative("byte0000");
            FixedBytes16 result = new FixedBytes16();
            result.byte0000 = (byte)item.intValue;

            item.Next(false);
            result.byte0001 = (byte)item.intValue;

            item.Next(false);
            result.byte0002 = (byte)item.intValue;

            item.Next(false);
            result.byte0003 = (byte)item.intValue;

            item.Next(false);
            result.byte0004 = (byte)item.intValue;

            item.Next(false);
            result.byte0005 = (byte)item.intValue;

            item.Next(false);
            result.byte0006 = (byte)item.intValue;

            item.Next(false);
            result.byte0007 = (byte)item.intValue;

            item.Next(false);
            result.byte0008 = (byte)item.intValue;

            item.Next(false);
            result.byte0009 = (byte)item.intValue;

            item.Next(false);
            result.byte0010 = (byte)item.intValue;

            item.Next(false);
            result.byte0011 = (byte)item.intValue;

            item.Next(false);
            result.byte0012 = (byte)item.intValue;

            item.Next(false);
            result.byte0013 = (byte)item.intValue;

            item.Next(false);
            result.byte0014 = (byte)item.intValue;

            item.Next(false);
            result.byte0015 = (byte)item.intValue;

            return result;
        }
        public static void SetFixedBytes16(SerializedProperty property, FixedBytes16 bytes)
        {
            SerializedProperty item = property.FindPropertyRelative("byte0000");
            item.intValue = bytes.byte0000;

            item.Next(false);
            item.intValue = bytes.byte0001;

            item.Next(false);
            item.intValue = bytes.byte0002;

            item.Next(false);
            item.intValue = bytes.byte0003;

            item.Next(false);
            item.intValue = bytes.byte0004;

            item.Next(false);
            item.intValue = bytes.byte0005;

            item.Next(false);
            item.intValue = bytes.byte0006;

            item.Next(false);
            item.intValue = bytes.byte0007;

            item.Next(false);
            item.intValue = bytes.byte0008;

            item.Next(false);
            item.intValue = bytes.byte0009;

            item.Next(false);
            item.intValue = bytes.byte0010;

            item.Next(false);
            item.intValue = bytes.byte0011;

            item.Next(false);
            item.intValue = bytes.byte0012;

            item.Next(false);
            item.intValue = bytes.byte0013;

            item.Next(false);
            item.intValue = bytes.byte0014;

            item.Next(false);
            item.intValue = bytes.byte0015;
        }

        #endregion

#endif

        #region ConstActionReference

        public static void SetConstActionReference(SerializedProperty property, Guid guid, params object[] args)
        {
            ConstActionReferenceSetGuid(property, guid);
            ConstActionReferenceSetArguments(property, args);
        }
        public static void ConstActionReferenceSetGuid(SerializedProperty property, Guid guid)
        {
            var guidProp = property.FindPropertyRelative("m_Guid");
            guidProp.stringValue = guid.ToString();
        }
        public static void ConstActionReferenceSetArguments(SerializedProperty property, params object[] args)
        {
            var argsProp = property.FindPropertyRelative("m_Arguments");

            argsProp.ClearArray();
            for (int i = 0; i < args.Length; i++)
            {
                argsProp.InsertArrayElementAtIndex(0);
            }
            for (int i = 0; i < args.Length; i++)
            {
                argsProp.GetArrayElementAtIndex(i).managedReferenceValue = args[i];
            }
        }

        #endregion

        #region AssetPathField

        private static class AssetPathFieldHelper
        {
            const string c_Str = "p_AssetPath";

            public static SerializedProperty GetAssetPathField(SerializedProperty property)
            {
                return property.FindPropertyRelative(c_Str);
            }
        }
        public static void SetAssetPathField(SerializedProperty property, UnityEngine.Object obj)
        {
            SerializedProperty pathProp = AssetPathFieldHelper.GetAssetPathField(property);

            pathProp.stringValue = obj == null ? string.Empty : AssetDatabase.GetAssetPath(obj);
        }
        public static void SetAssetPathFieldPath(SerializedProperty property, string path)
        {
            SerializedProperty pathProp = AssetPathFieldHelper.GetAssetPathField(property);

            pathProp.stringValue = path;
        }
        public static string GetAssetPathFieldPath(SerializedProperty property)
        {
            SerializedProperty pathProp = AssetPathFieldHelper.GetAssetPathField(property);

            return pathProp.stringValue;
        }
        public static UnityEngine.Object GetAssetPathField(SerializedProperty property)
        {
            SerializedProperty pathProp = AssetPathFieldHelper.GetAssetPathField(property);

            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pathProp.stringValue);
        }
        public static T GetAssetPathField<T>(SerializedProperty property)
            where T : UnityEngine.Object
        {
            return GetAssetPathField(property) as T;
        }

        #endregion

        #region MinMaxFloatField

        private static class MinMaxFloatFieldHelper
        {
            const string c_Min = "m_Min", c_Max = "m_Max";

            public static SerializedProperty GetMinProperty(SerializedProperty property)
            {
                return property.FindPropertyRelative(c_Min);
            }
            public static SerializedProperty GetMaxProperty(SerializedProperty property)
            {
                return property.FindPropertyRelative(c_Max);
            }
        }

        public static Vector2 GetMinMaxField(SerializedProperty property)
        {
            SerializedProperty
                minProp = MinMaxFloatFieldHelper.GetMinProperty(property),
                maxProp = MinMaxFloatFieldHelper.GetMaxProperty(property);

            return new Vector2(minProp.floatValue, maxProp.floatValue);
        }
        public static void SetMinMaxField(SerializedProperty property, Vector2 minMax)
        {
            SerializedProperty
                minProp = MinMaxFloatFieldHelper.GetMinProperty(property),
                maxProp = MinMaxFloatFieldHelper.GetMaxProperty(property);

            minProp.floatValue = minMax.x;
            maxProp.floatValue = minMax.y;
        }

        #endregion

        public static Vector3 GetVector3(this SerializedProperty t)
        {
            if (t.propertyType == SerializedPropertyType.Vector3)
            {
                return t.vector3Value;
            }
            else if (t.propertyType == SerializedPropertyType.Vector3Int)
            {
                return t.vector3IntValue;
            }
            else if (t.IsTypeOf<float3>())
            {
                SerializedProperty
                    x = t.FindPropertyRelative("x"),
                    y = t.FindPropertyRelative("y"),
                    z = t.FindPropertyRelative("z");

                return new Vector3(x.floatValue, y.floatValue, z.floatValue);
            }
            else if (t.IsTypeOf<int3>())
            {
                SerializedProperty
                    x = t.FindPropertyRelative("x"),
                    y = t.FindPropertyRelative("y"),
                    z = t.FindPropertyRelative("z");

                return new Vector3(x.intValue, y.intValue, z.intValue);
            }

            throw new NotImplementedException();
        }

        public static Type GetSystemType(this SerializedProperty t)
        {
            return PropertyDrawerHelper.GetTargetObjectOfProperty(t).GetType();
        }

        public static bool IsTypeOf<T>(this SerializedProperty t)
        {
            return TypeHelper.TypeOf<T>.Type.Name.Equals(t.type);
        }
        public static bool IsInArray(this SerializedProperty t)
        {
            return PropertyDrawerHelper.IsPropertyInArray(t);
        }

        public static SerializedProperty GetParent(this SerializedProperty t)
        {
            return PropertyDrawerHelper.GetParentOfProperty(t);
        }

        public static int ChildCount(this SerializedProperty t)
        {
            var temp = t.Copy();
            if (!temp.Next(true))
            {
                return 0;
            }

            int count = 0;
            do
            {
                count++;
            } while (temp.Next(false) && temp.depth > t.depth);

            return count;
        }
        public static IEnumerable<SerializedProperty> ForEachChild(this SerializedProperty t)
        {
            var temp = t.Copy();
            if (!temp.Next(true))
            {
                yield break;
            }

            do
            {
                yield return temp.Copy();
            } while (temp.Next(false) && temp.depth > t.depth);
        }

        #region Draw Method

        private static Dictionary<Type, PropertyDrawer> s_CachedPropertyDrawers = new Dictionary<Type, PropertyDrawer>();
        private static FieldInfo s_CachedPropertyTypeField, s_CachedPropertyUseChildField;
        private static FieldInfo CachedPropertyTypeField
        {
            get
            {
                if (s_CachedPropertyTypeField == null)
                {
                    const string c_Name = "m_Type";

                    Type drawerAttType = TypeHelper.TypeOf<CustomPropertyDrawer>.Type;
                    s_CachedPropertyTypeField = drawerAttType.GetField(c_Name, BindingFlags.NonPublic | BindingFlags.Instance);
                }

                return s_CachedPropertyTypeField;
            }
        }
        private static FieldInfo CachedPropertyUseChildField
        {
            get
            {
                if (s_CachedPropertyUseChildField == null)
                {
                    const string c_Name = "m_UseForChildren";

                    Type drawerAttType = TypeHelper.TypeOf<CustomPropertyDrawer>.Type;
                    s_CachedPropertyUseChildField = drawerAttType.GetField(c_Name, BindingFlags.NonPublic | BindingFlags.Instance);
                }

                return s_CachedPropertyUseChildField;
            }
        }

        public static void Draw(this SerializedProperty t, Rect rect, GUIContent label, bool includeChildren)
        {
            PropertyDrawer propertyDrawer = GetPropertyDrawer(t);

            if (propertyDrawer == null)
            {
                EditorGUI.PropertyField(rect, t, label, includeChildren);
                return;
            }

            propertyDrawer.OnGUI(rect, t, label);
        }
        public static void Draw(this SerializedProperty t, ref AutoRect rect, GUIContent label, bool includeChildren)
        {
            PropertyDrawer propertyDrawer = GetPropertyDrawer(t);

            if (propertyDrawer == null)
            {
                EditorGUI.PropertyField(
                    rect.Pop(EditorGUI.GetPropertyHeight(t))
                    , t, label, includeChildren);
                return;
            }

            propertyDrawer.OnGUI(rect.Pop(propertyDrawer.GetPropertyHeight(t, label)), t, label);
        }
        public static bool HasCustomPropertyDrawer(this SerializedProperty t)
        {
            return GetPropertyDrawer(t) != null;
        }
        private static PropertyDrawer GetPropertyDrawer(SerializedProperty t)
        {
            Type propertyType = t.GetFieldInfo().FieldType;

            if (!s_CachedPropertyDrawers.TryGetValue(propertyType, out PropertyDrawer propertyDrawer))
            {
                Type foundDrawerType = null;
                Type foundDrawerTargetType = null;

                //$"{propertyType.Name} start".ToLog();
                foreach (var drawerType in TypeHelper.GetTypesIter(other => !other.IsAbstract && !other.IsInterface && other.GetCustomAttributes<CustomPropertyDrawer>().Any()))
                {
                    foreach (var customPropertyDrawer in drawerType.GetCustomAttributes<CustomPropertyDrawer>())
                    {
                        Type targetType = (Type)CachedPropertyTypeField.GetValue(customPropertyDrawer);
                        bool useChild = (bool)CachedPropertyUseChildField.GetValue(customPropertyDrawer);
                        //$"target:{targetType.Name} usechild:{useChild}".ToLog();
                        if (targetType.Equals(propertyType))
                        {
                            //$"target:{targetType.Name} {propertyType.Name}".ToLog();
                            foundDrawerType = drawerType;

                            break;
                        }
                        else if (useChild && (propertyType.IsSubclassOf(targetType) || targetType.IsAssignableFrom(propertyType)))
                        {
                            if (foundDrawerType != null)
                            {
                                // 만약 더 상위를 타겟으로 하고 있으면 교체
                                if (foundDrawerTargetType.IsAssignableFrom(targetType))
                                {
                                    foundDrawerType = drawerType;
                                    foundDrawerTargetType = targetType;
                                }

                                continue;
                            }

                            foundDrawerType = drawerType;
                            foundDrawerTargetType = targetType;
                        }
                    }
                }

                if (foundDrawerType != null)
                {
                    propertyDrawer = (PropertyDrawer)Activator.CreateInstance(foundDrawerType);
                }
                s_CachedPropertyDrawers.Add(propertyType, propertyDrawer);
            }

            SetupPropertyDrawer(propertyDrawer, t);
            return propertyDrawer;
        }
        private static void SetupPropertyDrawer(PropertyDrawer propertyDrawer, SerializedProperty property)
        {
            if (propertyDrawer == null) return;

            FieldInfo fieldInfoField = TypeHelper.TypeOf<PropertyDrawer>.GetFieldInfo("m_FieldInfo");
            fieldInfoField.SetValue(propertyDrawer, property.GetFieldInfo());
        }

        #endregion

        public static FieldInfo GetFieldInfo(this SerializedProperty prop)
        {
            if (prop == null) return null;

            string path = prop.propertyPath.Replace(".Array.data[", "[");
            Type t = prop.serializedObject.targetObject.GetType();
            FieldInfo currentField = null;
            string[] elements = path.Split('.');

            foreach (string element in elements)
            {
                Type currentType = currentField == null ? t : currentField.FieldType;
                if (currentType.IsArray) currentType = currentType.GetElementType();

                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    currentField = TypeHelper.GetFieldInfoRecursive(currentType, elementName);
                    if (currentField == null)
                    {
                        throw new Exception($"from ({currentType.Name}) {elementName}");
                    }
                }
                else
                {
                    currentField = TypeHelper.GetFieldInfoRecursive(currentType, element);
                    if (currentField == null)
                    {
                        throw new Exception($"from ({currentType.Name}) {element}");
                    }
                }
            }

            return currentField;
        }

        public static object GetTargetObject(this SerializedProperty t)
        {
            return PropertyDrawerHelper.GetTargetObjectOfProperty(t);
        }
        public static void SetDefaultValue(this SerializedProperty t)
        {
            switch (t.propertyType)
            {
                case SerializedPropertyType.Integer:
                    t.intValue = 0;
                    break;
                case SerializedPropertyType.Boolean:
                    t.boolValue = false;
                    break;
                case SerializedPropertyType.Float:
                    t.floatValue = 0;
                    break;
                case SerializedPropertyType.String:
                    t.stringValue = String.Empty;
                    break;
                case SerializedPropertyType.Color:
                    t.colorValue = Color.white;
                    break;
                case SerializedPropertyType.ObjectReference:
                    t.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.Enum:
                    t.enumValueIndex = 0;
                    break;
                case SerializedPropertyType.Vector2:
                    t.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    t.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Vector4:
                    t.vector4Value = Vector4.zero;
                    break;
                case SerializedPropertyType.Rect:
                    t.rectValue = default(Rect);
                    break;
                case SerializedPropertyType.AnimationCurve:
                    t.animationCurveValue = new AnimationCurve();
                    break;
                case SerializedPropertyType.Bounds:
                    t.boundsValue = default(Bounds);
                    break;
                case SerializedPropertyType.Gradient:
                    t.colorValue = Color.white;
                    break;
                case SerializedPropertyType.Quaternion:
                    t.quaternionValue = Quaternion.identity;
                    break;
                case SerializedPropertyType.Vector2Int:
                    t.vector2IntValue = Vector2Int.zero;
                    break;
                case SerializedPropertyType.Vector3Int:
                    t.vector3IntValue = Vector3Int.zero;
                    break;
                case SerializedPropertyType.RectInt:
                    t.rectIntValue = default(RectInt);
                    break;
                case SerializedPropertyType.BoundsInt:
                    t.boundsIntValue = default(BoundsInt);
                    break;
                case SerializedPropertyType.ManagedReference:
                    t.managedReferenceValue = Activator.CreateInstance(t.GetFieldInfo().FieldType);
                    break;
                case SerializedPropertyType.Generic:
                    //FieldInfo field = t.GetFieldInfo();
                    //object defaultObj = Activator.CreateInstance(field.FieldType.GetElementType());

                    ////var parent = t.GetParent().GetTargetObject();
                    //field.SetValue(parent, Activator.CreateInstance(field.FieldType.GetElementType()));
                    //break;

                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Character:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.ExposedReference:
                default:
                    throw new NotImplementedException($"{t.propertyType}");
            }
        }
    }
}

#endif
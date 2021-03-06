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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;

namespace Point.Collections
{
    /// <summary>
    /// XML 으로 런타임 중 사용되는 값을 저장할 수 있게 도와줍니다.
    /// </summary>
    public sealed class XmlSettings
    {
        private static readonly string s_GlobalConfigPath = Path.Combine(PointPath.DataPath,  "config");
        private static readonly Dictionary<Type, FieldInfo[]> s_CachedSettingFields = new Dictionary<Type, FieldInfo[]>();

        private static readonly List<Type> s_StaticLoadedTypes = new List<Type>();

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            s_StaticLoadedTypes.Clear();
            var iter = TypeHelper
                .GetTypesIter(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetCustomAttribute<XmlSettingsAttribute>() != null);
            foreach (var item in iter)
            {
                FieldInfo[] settingFields = GetSettingFields(item);
                XElement element = GetElement(item, item.GetCustomAttribute<XmlSettingsAttribute>());

                bool hasValue = false;
                for (int i = 0; i < settingFields.Length; i++)
                {
                    if (!settingFields[i].IsStatic) continue;

                    FieldInfo field = settingFields[i];
                    LoadValue(element, field, null);

                    hasValue |= true;
                }

                if (!hasValue) continue;

                s_StaticLoadedTypes.Add(item);
            }

            Application.quitting -= OnApplicationExit;
            Application.quitting += OnApplicationExit;
        }
        private static void OnApplicationExit()
        {
            foreach (Type item in s_StaticLoadedTypes)
            {
                FieldInfo[] settingFields = GetSettingFields(item);
                XElement element = GetElement(item, item.GetCustomAttribute<XmlSettingsAttribute>());

                foreach (FieldInfo field in settingFields)
                {
                    if (!field.IsStatic) continue;

                    SaveValue(element, field, null);
                }
            }
        }

        private static XDocument LoadDocumentFromDisk()
        {
            XDocument doc;
            if (File.Exists(s_GlobalConfigPath))
            {
                using (var rdr = new StreamReader(s_GlobalConfigPath))
                {
                    string xml = rdr.ReadToEnd();
                    doc = XDocument.Parse(xml);
                }
            }
            else
            {
                doc = new XDocument(
                    new XElement("Root")
                    );
            }
            return doc;
        }
        private static XDocument LoadDocumentFromPref(string key)
        {
            string xmlString = PlayerPrefs.GetString(key);
            XDocument doc;
            if (!string.IsNullOrEmpty(xmlString)) doc = XDocument.Parse(xmlString);
            else
            {
                doc = new XDocument(
                    new XElement(key)
                    );
            }
            return doc;
        }

        private static void SaveDocument(XDocument doc, bool saveToDisk)
        {
            if (!saveToDisk)
            {
                PlayerPrefs.SetString(doc.Root.Name.ToString(), doc.Document.ToString());
                return;
            }

            string xml = doc.Document.ToString();
            Debug.Log(xml);

            using (var wr = new StreamWriter(s_GlobalConfigPath, false))
            {
                wr.Write(xml);
            }
        }
        private static void SaveDocument(XElement doc, bool saveToDisk)
        {
            if (!saveToDisk)
            {
                PlayerPrefs.SetString(doc.Name.ToString(), doc.Document.ToString());
                return;
            }

            string xml = doc.Document.ToString();
            //Debug.Log(xml);

            using (var wr = new StreamWriter(s_GlobalConfigPath, false))
            {
                wr.Write(xml);
            }
        }

        private static XElement GetElement(Type t, string propertyName, bool saveToDisk)
        {
            string key;
            if (propertyName.IsNullOrEmpty()) key = TypeHelper.ToString(t);
            else key = propertyName;

            XDocument doc = saveToDisk ? LoadDocumentFromDisk() : LoadDocumentFromPref(key);
            XElement objRoot;
            if (saveToDisk)
            {
                objRoot = doc.Root.Element(key);
                if (objRoot == null)
                {
                    objRoot = new XElement(key);
                    doc.Root.Add(objRoot);
                }
            }
            else
            {
                objRoot = doc.Element(key);
                if (objRoot == null)
                {
                    objRoot = new XElement(key);
                    doc.Add(objRoot);
                }
            }

            return objRoot;
        }
        private static XElement GetElement(object obj, XmlSettingsAttribute settings = null)
        {
            Type t = obj.GetType();
            if (settings == null)
            {
                settings = t.GetCustomAttribute<XmlSettingsAttribute>();
            }

            return GetElement(t, settings.PropertyName, settings.SaveToDisk);
        }
        private static FieldInfo[] GetSettingFields(Type t)
        {
            if (s_CachedSettingFields.TryGetValue(t, out FieldInfo[] fields)) return fields;

            fields = t
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(target =>
                {
                    return target.GetCustomAttribute<XmlFieldAttribute>() != null;
                })
                .ToArray();
            s_CachedSettingFields.Add(t, fields);

            return fields;
        }

        private static void SaveValue(XElement objRoot, FieldInfo fieldInfo, object obj)
        {
            if (!ValidateFieldType(fieldInfo))
            {
                Debug.Log($"not valid {fieldInfo.Name}");
                return;
            }

            XmlFieldAttribute fieldAtt = fieldInfo.GetCustomAttribute<XmlFieldAttribute>();

            string elementName;
            if (fieldAtt == null)
            {
                elementName = fieldInfo.Name;
            }
            else
            {
                elementName = fieldAtt.PropertyName.IsNullOrEmpty() ? fieldInfo.Name : fieldAtt.PropertyName;
            }
            
            XElement element = objRoot.Element(elementName);
            if (element == null)
            {
                objRoot.Add(
                    new XElement(
                        XmlParser.ConvertToXml(elementName, fieldInfo.GetValue(obj))
                        //elementName,
                        //fieldInfo.GetValue(obj).ToString()
                        )
                    );
                return;
            }

            XElement xml = XmlParser.ConvertToXml(elementName, fieldInfo.GetValue(obj));
            if (xml.HasElements)
            {
                foreach (var item in xml.Elements())
                {
                    if (element.Element(item.Name) != null)
                    {
                        element.Element(item.Name).Value = item.Value;
                    }
                    else element.Add(item);

                    //$"{element.Name}: {item.Name}:{item.Value}".ToLog();
                }
            }
            else
            {
                element.Value = xml.Value;
                //$"{element.Name}: {element.Value}".ToLog();
            }
        }
        private static void LoadValue(XElement objRoot, FieldInfo fieldInfo, object obj)
        {
            if (!ValidateFieldType(fieldInfo))
            {
                Debug.Log($"not valid {fieldInfo.Name}");
                return;
            }

            XmlFieldAttribute fieldAtt = fieldInfo.GetCustomAttribute<XmlFieldAttribute>();

            string elementName;
            if (fieldAtt == null)
            {
                elementName = fieldInfo.Name;
            }
            else
            {
                elementName = fieldAtt.PropertyName.IsNullOrEmpty() ? fieldInfo.Name : fieldAtt.PropertyName;
            }
            
            XElement element = objRoot.Element(elementName);
            if (element == null)
            {
                objRoot.Add(
                    XmlParser.ConvertToXml(elementName, fieldInfo.GetValue(obj))
                    );
                Debug.Log($"not exist {elementName} adding");
                return;
            }

            object value = XmlParser.ConvertToObject(fieldInfo.FieldType, element);
            //if (element.HasElements)
            //{
            //    foreach (var item in element.Elements())
            //    {
            //        $"{fieldInfo.Name}: {item.Name}:{item.Value}".ToLog();
            //    }
            //}
            //else
            //{
            //    $"{fieldInfo.Name}: {element.Value}".ToLog();
            //}

            fieldInfo.SetValue(obj, value);
        }

        /// <summary>
        /// <paramref name="obj"/> 의 설정 값을 저장합니다.
        /// </summary>
        /// <remarks>
        /// <paramref name="obj"/> 는 <see cref="XmlSettingsAttribute"/> 를 가지고 있어야합니다.
        /// </remarks>
        /// <param name="obj"></param>
        public static void SaveSettings(object obj)
        {
            Type t = obj.GetType();
            XmlSettingsAttribute att = t.GetCustomAttribute<XmlSettingsAttribute>();
            if (att == null) return;

            XElement objRoot = GetElement(obj, att);

            IEnumerable<FieldInfo> fieldsIter = GetSettingFields(t);
            foreach (FieldInfo fieldInfo in fieldsIter)
            {
                SaveValue(objRoot, fieldInfo, obj);
            }

            //Debug.Log($"save doc for {obj.GetType().Name}");
            SaveDocument(objRoot, att.SaveToDisk);
        }
        public static void SaveSettings(object obj, string propertyName, bool saveToDisk)
        {
            Type t = obj.GetType();
            XElement objRoot = GetElement(t, propertyName, saveToDisk);

            IEnumerable<FieldInfo> fieldsIter = GetSettingFields(t);
            foreach (FieldInfo fieldInfo in fieldsIter)
            {
                SaveValue(objRoot, fieldInfo, obj);
            }

            //Debug.Log($"save doc for {obj.GetType().Name}");
            SaveDocument(objRoot, saveToDisk);
        }
        /// <summary>
        /// <paramref name="obj"/> 의 설정 값을 불러옵니다.
        /// </summary>
        /// <remarks>
        /// <paramref name="obj"/> 는 <see cref="XmlSettingsAttribute"/> 를 가지고 있어야합니다.
        /// </remarks>
        /// <param name="obj"></param>
        public static void LoadSettings(object obj)
        {
            Type t = obj.GetType();
            XmlSettingsAttribute att = t.GetCustomAttribute<XmlSettingsAttribute>();
            if (att == null) return;

            XElement objRoot = GetElement(obj, att);

            IEnumerable<FieldInfo> fieldsIter = GetSettingFields(t);
            foreach (FieldInfo fieldInfo in fieldsIter)
            {
                LoadValue(objRoot, fieldInfo, obj);
            }

            //Debug.Log(doc.ToString());
            SaveDocument(objRoot, att.SaveToDisk);
        }
        public static void LoadSettings(object obj, string propertyName, bool saveToDisk)
        {
            Type t = obj.GetType();
            XElement objRoot = GetElement(t, propertyName, saveToDisk);

            IEnumerable<FieldInfo> fieldsIter = GetSettingFields(t);
            foreach (FieldInfo fieldInfo in fieldsIter)
            {
                LoadValue(objRoot, fieldInfo, obj);
            }

            //Debug.Log(doc.ToString());
            SaveDocument(objRoot, saveToDisk);
        }
        public static T LoadValueFromDisk<T>(string key, string name, T defaultValue = default(T))
        {
            XDocument doc = LoadDocumentFromDisk();
            XElement element = doc.Element(key);
            bool isModified = false;

            if (element == null)
            {
                element = new XElement(key);
                doc.Add(element);
                isModified = true;
            }
            XElement value = element.Element(name);
            if (value == null)
            {
                //value = new XElement(name, defaultValue.ToString());
                value = XmlParser.ConvertToXml(name, defaultValue);
                element.Add(value);
                isModified = true;

                Debug.Log($"value not found for {key}:{name}. using default {defaultValue}");
            }

            if (isModified)
            {
                SaveDocument(doc, true);
            }

            T result = (T)XmlParser.ConvertToObject(TypeHelper.TypeOf<T>.Type, value);
            //Debug.Log($"loaded result {result}");
            return result;
        }
        public static T LoadValueFromPref<T>(string key, string name, T defaultValue = default(T))
        {
            XDocument doc = LoadDocumentFromPref(key);
            XElement element = doc.Element(key);
            bool isModified = false;

            if (element == null)
            {
                element = new XElement(key);
                doc.Add(element);
                isModified = true;
            }
            XElement value = element.Element(name);
            if (value == null)
            {
                //value = new XElement(name, defaultValue.ToString());
                value = XmlParser.ConvertToXml(name, defaultValue);
                element.Add(value);
                isModified = true;

                Debug.Log($"value not found for {key}:{name}. using default {defaultValue}");
            }

            if (isModified)
            {
                SaveDocument(doc, false);
            }

            T result = (T)XmlParser.ConvertToObject(TypeHelper.TypeOf<T>.Type, value);
            //Debug.Log($"loaded result {result}");
            return result;
        }

        // https://www.delftstack.com/howto/csharp/serialize-object-to-xml-in-csharp/#:~:text=The%20XmlSerializer%20class%20converts%20class,an%20XML%20file%20or%20string.
        private static bool ValidateFieldType(FieldInfo fieldInfo)
        {
            Type t = fieldInfo.FieldType;
            if (TypeHelper.TypeOf<bool>.Type.Equals(t) ||
                TypeHelper.TypeOf<float>.Type.Equals(t) ||
                TypeHelper.TypeOf<int>.Type.Equals(t) ||
                TypeHelper.TypeOf<double>.Type.Equals(t) ||
                TypeHelper.TypeOf<string>.Type.Equals(t) ||
                TypeHelper.InheritsFrom<IDictionary>(t)
                )
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// XML 타입을 선언합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class XmlSettingsAttribute : Attribute
    {
        /// <summary>
        /// 사용자가 지정한 이름으로 최상단 XML 구조를 만듭니다. 
        /// <see langword="null"/> 이거나 아무것도 없으면 타입의 이름을 대신 사용합니다.
        /// </summary>
        public string PropertyName;
        /// <summary>
        /// <see langword="false"/> 일 경우, <see cref="PlayerPrefs"/> 를 사용하여 레지스트리에 저장합니다. 
        /// <see langword="true"/> 일 경우에는 로컬에 저장합니다.
        /// </summary>
        public bool SaveToDisk;
    }
    /// <summary>
    /// XML 필드를 선언합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class XmlFieldAttribute : Attribute
    {
        /// <summary>
        /// 사용자가 지정한 이름으로 XML 구조를 만듭니다. 
        /// <see langword="null"/> 이거나 아무것도 없으면 필드의 이름을 대신 사용합니다.
        /// </summary>
        public string PropertyName;
    }
}

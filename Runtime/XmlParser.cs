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
using System.Xml.Linq;

namespace Point.Collections
{
    public sealed class XmlParser
    {
        public static XElement CreateRoot(string name)
        {
            return new XElement(name);
        }
        /// <summary>
        /// <paramref name="obj"/> 를 <paramref name="elementName"/> 이름의 XML 필드로 바꾸어 반환합니다.
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static XElement ConvertToXml(string elementName, object obj)
        {
            if (obj is IDictionary dictionary)
            {
                XElement root = new XElement(elementName);
                var iter = dictionary.GetEnumerator();
                while (iter.MoveNext())
                {
                    XElement item = new XElement(iter.Key.ToString(), iter.Value.ToString());
                    root.Add(item);
                }

                return root;
            }

            return new XElement(elementName, obj.ToString());
        }

        /// <summary>
        /// <see cref="ConvertToXml(string, object)"/> 을 통해 변환된 XML 을 
        /// 다시 오브젝트(<typeparamref name="T"/>) 로 변환하여 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T ConvertToObject<T>(XElement xml) => (T)ConvertToObject(TypeHelper.TypeOf<T>.Type, xml);
        /// <summary>
        /// <see cref="ConvertToXml(string, object)"/> 을 통해 변환된 XML 을 
        /// 다시 오브젝트로 변환하여 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object ConvertToObject(Type type, XElement xml)
        {
            if (TypeHelper.InheritsFrom(type, TypeHelper.TypeOf<IDictionary>.Type))
            {
                IDictionary dictionary = (IDictionary)Activator.CreateInstance(type);
                Type
                    keyType = type.GenericTypeArguments[0],
                    valueType = type.GenericTypeArguments[1];

                foreach (var item in xml.Elements())
                {
                    object
                        key = Convert.ChangeType(item.Name.ToString(), keyType),
                        value = Convert.ChangeType(item.Value, valueType);

                    dictionary.Add(key, value);
                }

                return dictionary;
            }

            return Convert.ChangeType(xml.Value, type);
        }
    }
}

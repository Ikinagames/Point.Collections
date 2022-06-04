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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Point.Collections
{
    public static class ExtensionMethods
    {
        public static TypeInfo ToTypeInfo(this Type type) => TypeHelper.ToTypeInfo(type);

        #region IConvertible

        public static int ToInt32<T>(this T t) where T : struct, IConvertible => t.ToInt32(System.Globalization.CultureInfo.InvariantCulture);
        public static double ToDouble<T>(this T t) where T : struct, IConvertible => t.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
        public static float ToSingle<T>(this T t) where T : struct, IConvertible => t.ToSingle(System.Globalization.CultureInfo.InvariantCulture);
        public static long ToInt64<T>(this T t) where T : struct, IConvertible => t.ToInt64(System.Globalization.CultureInfo.InvariantCulture);
        public static string ToString<T>(this T t) where T : struct, IConvertible => t.ToString(System.Globalization.CultureInfo.InvariantCulture);

        #endregion

        #region String

        private const char c_StringLineSeperator = '\n';
        private static string[] s_StringLineSpliter = new[] { "\r\n", "\r", "\n" };
        private static char[] s_StringLineSpliterChar = new[] { '\r', '\n' };

        /// <summary>
        /// 이 스트링이 null 혹은 비었는지 반환합니다.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string t)
        {
            return string.IsNullOrEmpty(t);
        }
        /// <summary>
        /// 문자열 마지막에 Return iteral 을 추가하여 반환합니다.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ReturnAtLast(this string t)
        {
            return t + c_StringLineSeperator;
        }
        /// <summary>
        /// 문자열 처음에 Return iteral 을 추가하여 반환합니다.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ReturnAtFirst(this string t)
        {
            return c_StringLineSeperator + t;
        }

        public static int GetLineCount(this string t)
        {
            return t.GetLines().Length;
        }
        public static string[] GetLines(this string t)
        {
            return t.Split(s_StringLineSpliter, StringSplitOptions.None);
        }
        public static string RemoveLines(this string t, int lineIndex, int count)
        {
            var lines = t.GetLines().ToList();
            lines.RemoveRange(lineIndex, count);

            string sum = string.Empty;
            for (int i = 0; i < lines.Count; i++)
            {
                sum += lines[i];

                if (i + 1 < lines.Count) sum += c_StringLineSeperator;
            }

            return sum;
        }

        #endregion

        #region Monobehaviour

        public static T GetOrAddComponent<T>(this GameObject t)
            where T : UnityEngine.Component
        {
            T p = t.GetComponent<T>();
            if (p == null) p = t.AddComponent<T>();

            return p;
        }
        public static T GetOrAddComponent<T>(this Transform t) where T : UnityEngine.Component => t.gameObject.GetOrAddComponent<T>();

        public static TComponent[] GetComponentsInChildrenOnly<TComponent>(this UnityEngine.Component t) 
        {
            Transform tr = t.transform;
            int count = tr.childCount;
            List<TComponent> components = new List<TComponent>();
            for (int i = 0; i < count; i++)
            {
                components.AddRange(tr.GetChild(i).GetComponentsInChildren<TComponent>());
            }

            return components.ToArray();
        }

        #endregion
    }
}

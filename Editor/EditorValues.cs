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
#if UNITY_2019 || !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using System;
using UnityEditor;

namespace Point.Collections.Editor
{
    public static class EditorValues
    {
        private const string
            c_KeyFormat = "Point_{0}_{1}";

        private static string GetConcreteKey(object obj, string key)
        {
            return string.Format(c_KeyFormat, obj.GetType().Name, key);
        }
        private static string GetConcreteKey(Type t, string key)
        {
            return string.Format(c_KeyFormat, t.Name, key);
        }

        #region Boolen

        public static bool GetBool<T>(string key, bool defaultValue)
        {
            string text = GetConcreteKey(TypeHelper.TypeOf<T>.Type, key);
            return EditorPrefs.GetBool(text, defaultValue);
        }
        public static bool GetBool(object obj, string key, bool defaultValue)
        {
            string text = GetConcreteKey(obj, key);
            return EditorPrefs.GetBool(text, defaultValue);
        }
        public static void SetBool<T>(string key, bool value)
        {
            string text = GetConcreteKey(TypeHelper.TypeOf<T>.Type, key);
            EditorPrefs.SetBool(text, value);
        }
        public static void SetBool(object obj, string key, bool value)
        {
            string text = GetConcreteKey(obj, key);
            EditorPrefs.SetBool(text, value);
        }

        #endregion
    }
}

#endif
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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
#if UNITY_BURST
using Unity.Burst;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Native;
using Point.Collections.IO.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Point.Collections
{
    public sealed class CollectionUtility : CLRSingleTone<CollectionUtility>
    {
#if UNITYENGINE && UNITY_MATHEMATICS
        private Unity.Mathematics.Random m_Random;
#else
        private System.Random m_Random;
#endif
        private JsonSerializerSettings m_JsonSettings;

        public struct EngineTypeGuid
        {
            //public static Guid GameObject => 
        }

        public static JsonSerializerSettings JsonSerializerSettings => Instance.m_JsonSettings;

        public static void Initialize()
        {
            CollectionUtility ins = Instance;
        }
        protected override void OnInitialize()
        {
#if POINT_COLLECTIONS_NATIVE || !UNITY_MATHEMATICS
#if POINT_COLLECTIONS_NATIVE
            NativeDebug.Initialize();
#endif
            m_Random = new System.Random();
#else
            Instance.m_Random = new Unity.Mathematics.Random();
            Instance.m_Random.InitState();
#endif

            m_JsonSettings = new JsonSerializerSettings();
            m_JsonSettings.Converters = new List<JsonConverter>()
            {
                Float3JsonConverter.Static,
                Float2JsonConverter.Static,
                QuaternionJsonConvereter.Static
            };
            Type[] customConverters = TypeHelper.GetTypes(other => !other.IsAbstract && TypeHelper.TypeOf<JsonConverter>.Type.IsAssignableFrom(other));
            for (int i = 0; i < customConverters.Length; i++)
            {
                m_JsonSettings.Converters.Add((JsonConverter)Activator.CreateInstance(customConverters[i]));
            }

            JsonConvert.DefaultSettings = GetJsonSerializerSettings;
        }
        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            return JsonSerializerSettings;
        }

        public static short CreateHashCode2()
        {
#if UNITYENGINE && UNITY_MATHEMATICS
            return unchecked((short)Instance.m_Random.NextInt(short.MinValue, short.MaxValue));
#else
            return unchecked((short)Instance.m_Random.Next(short.MinValue, short.MaxValue));
#endif
        }
        public static int CreateHashCode() 
        {
#if UNITYENGINE && UNITY_MATHEMATICS
            return Instance.m_Random.NextInt(int.MinValue, int.MaxValue); 
#else
            return Instance.m_Random.Next(int.MinValue, int.MaxValue);
#endif
        }
    }
}

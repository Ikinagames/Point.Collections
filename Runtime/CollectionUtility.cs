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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using System;
using Unity.Burst;
using Point.Collections.Native;
using Newtonsoft.Json;
using System.Collections.Generic;
using Point.Collections.IO.Json;
using System.Reflection;

namespace Point.Collections
{
    public sealed class CollectionUtility : CLRSingleTone<CollectionUtility>
    {
        private Unity.Mathematics.Random m_Random;
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
#if POINT_COLLECTIONS_NATIVE
            NativeDebug.Initialize();
#endif
            Instance.m_Random = new Unity.Mathematics.Random();
            Instance.m_Random.InitState();

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

        public static short CreateHashCode2() => unchecked((short)Instance.m_Random.NextInt(short.MinValue, short.MaxValue));
        public static int CreateHashCode() => Instance.m_Random.NextInt(int.MinValue, int.MaxValue);
    }
}
